using Minimes.Application.DTOs.QRCode;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 二维码服务实现 - 二维码管理的业务逻辑
/// </summary>
public class QRCodeService : IQRCodeService
{
    private readonly IQRCodeRepository _repository;
    private readonly IMeatTypeRepository _meatTypeRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IQRCodeGeneratorService _qrCodeGenerator;

    public QRCodeService(
        IQRCodeRepository repository,
        IMeatTypeRepository meatTypeRepository,
        IEmployeeRepository employeeRepository,
        IQRCodeGeneratorService qrCodeGenerator)
    {
        _repository = repository;
        _meatTypeRepository = meatTypeRepository;
        _employeeRepository = employeeRepository;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task<QRCodeResponse> CreateAsync(CreateQRCodeRequest request)
    {
        // 检查肉类类型是否存在
        var meatType = await _meatTypeRepository.GetByIdAsync(request.MeatTypeId);
        if (meatType == null)
        {
            throw new InvalidOperationException($"肉类类型ID {request.MeatTypeId} 不存在！");
        }

        // 生成二维码内容（MeatType.Code + "-" + Code）
        var content = $"{meatType.Code}-{request.Code}";

        // 检查二维码内容是否已存在
        if (await _repository.ContentExistsAsync(content))
        {
            throw new InvalidOperationException($"二维码内容 '{content}' 已存在！");
        }

        // 生成二维码图片
        var imageBase64 = _qrCodeGenerator.GenerateQRCodeBase64(content);

        // 创建二维码实体
        var qrCode = new QRCode
        {
            Code = request.Code,
            MeatTypeId = request.MeatTypeId,
            Content = content,
            ImageBase64 = imageBase64,
            BatchNumber = request.BatchNumber,
            PrintCount = 0,
            IsActive = true,
            Remarks = request.Remarks,
            CreatedAt = DateTime.Now
        };

        var created = await _repository.AddAsync(qrCode);
        await _repository.SaveChangesAsync();

        // 加载导航属性
        created.MeatType = meatType;

        return ToResponse(created);
    }

    public async Task<List<QRCodeResponse>> BatchCreateAsync(BatchCreateQRCodeRequest request)
    {
        // 检查肉类类型是否存在
        var meatType = await _meatTypeRepository.GetByIdAsync(request.MeatTypeId);
        if (meatType == null)
        {
            throw new InvalidOperationException($"肉类类型ID {request.MeatTypeId} 不存在！");
        }

        var results = new List<QRCodeResponse>();

        // 循环生成二维码
        for (int i = request.StartNumber; i <= request.EndNumber; i++)
        {
            // 生成编号（前缀 + 补零后的数字）
            var code = $"{request.Prefix}{i.ToString().PadLeft(request.PaddingLength, '0')}";

            // 生成二维码内容
            var content = $"{meatType.Code}-{code}";

            // 检查二维码内容是否已存在
            if (await _repository.ContentExistsAsync(content))
            {
                throw new InvalidOperationException($"二维码内容 '{content}' 已存在，批量创建失败！");
            }

            // 生成二维码图片
            var imageBase64 = _qrCodeGenerator.GenerateQRCodeBase64(content);

            // 创建二维码实体
            var qrCode = new QRCode
            {
                Code = code,
                MeatTypeId = request.MeatTypeId,
                Content = content,
                ImageBase64 = imageBase64,
                BatchNumber = request.BatchNumber,
                PrintCount = 0,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var created = await _repository.AddAsync(qrCode);
            created.MeatType = meatType;
            results.Add(ToResponse(created));
        }

        await _repository.SaveChangesAsync();

        return results;
    }

    public async Task<QRCodeResponse?> GetByIdAsync(int id)
    {
        var qrCode = await _repository.GetByIdAsync(id);
        return qrCode == null ? null : await ToResponseAsync(qrCode);
    }

    public async Task<QRCodeResponse?> GetByContentAsync(string content)
    {
        var qrCode = await _repository.GetByContentAsync(content);
        return qrCode == null ? null : await ToResponseAsync(qrCode);
    }

    public async Task<List<QRCodeResponse>> GetAllAsync()
    {
        var qrCodes = await _repository.GetAllAsync();
        var orderedQRCodes = qrCodes.OrderByDescending(q => q.CreatedAt).ToList();

        // 艹，批量加载员工信息，避免N+1查询
        var employeeCodes = orderedQRCodes
            .Where(q => !string.IsNullOrEmpty(q.EmployeeCode))
            .Select(q => q.EmployeeCode!)
            .Distinct()
            .ToList();

        // 查询所有相关员工
        var employees = new Dictionary<string, string>();
        foreach (var code in employeeCodes)
        {
            var employee = await _employeeRepository.GetByCodeAsync(code);
            if (employee != null)
            {
                employees[code] = employee.Name;
            }
        }

        // 映射到Response，填充员工姓名
        return orderedQRCodes.Select(q => new QRCodeResponse
        {
            Id = q.Id,
            Code = q.Code,
            MeatTypeId = q.MeatTypeId,
            MeatTypeName = q.MeatType?.Name ?? string.Empty,
            MeatTypeCode = q.MeatType?.Code ?? string.Empty,
            EmployeeCode = q.EmployeeCode,
            EmployeeName = !string.IsNullOrEmpty(q.EmployeeCode) && employees.ContainsKey(q.EmployeeCode)
                ? employees[q.EmployeeCode]
                : null,
            Content = q.Content,
            ImageBase64 = q.ImageBase64,
            BatchNumber = q.BatchNumber,
            PrintCount = q.PrintCount,
            LastPrintedAt = q.LastPrintedAt,
            IsActive = q.IsActive,
            Remarks = q.Remarks,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        }).ToList();
    }

    public async Task<List<QRCodeResponse>> GetActiveAsync()
    {
        var qrCodes = await _repository.GetActiveAsync();
        var qrCodeList = qrCodes.ToList();

        // 艹，批量加载员工信息，避免N+1查询
        var employeeCodes = qrCodeList
            .Where(q => !string.IsNullOrEmpty(q.EmployeeCode))
            .Select(q => q.EmployeeCode!)
            .Distinct()
            .ToList();

        // 查询所有相关员工
        var employees = new Dictionary<string, string>();
        foreach (var code in employeeCodes)
        {
            var employee = await _employeeRepository.GetByCodeAsync(code);
            if (employee != null)
            {
                employees[code] = employee.Name;
            }
        }

        // 映射到Response，填充员工姓名
        return qrCodeList.Select(q => new QRCodeResponse
        {
            Id = q.Id,
            Code = q.Code,
            MeatTypeId = q.MeatTypeId,
            MeatTypeName = q.MeatType?.Name ?? string.Empty,
            MeatTypeCode = q.MeatType?.Code ?? string.Empty,
            EmployeeCode = q.EmployeeCode,
            EmployeeName = !string.IsNullOrEmpty(q.EmployeeCode) && employees.ContainsKey(q.EmployeeCode)
                ? employees[q.EmployeeCode]
                : null,
            Content = q.Content,
            ImageBase64 = q.ImageBase64,
            BatchNumber = q.BatchNumber,
            PrintCount = q.PrintCount,
            LastPrintedAt = q.LastPrintedAt,
            IsActive = q.IsActive,
            Remarks = q.Remarks,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        }).ToList();
    }

    public async Task<QRCodeResponse> UpdateAsync(int id, UpdateQRCodeRequest request)
    {
        var qrCode = await _repository.GetByIdAsync(id);
        if (qrCode == null)
        {
            throw new InvalidOperationException($"二维码ID {id} 不存在！");
        }

        // 只允许修改IsActive和Remarks
        qrCode.IsActive = request.IsActive;
        qrCode.Remarks = request.Remarks;
        qrCode.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(qrCode);
        await _repository.SaveChangesAsync();

        return ToResponse(qrCode);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var qrCode = await _repository.GetByIdAsync(id);
        if (qrCode == null)
        {
            return false;
        }

        await _repository.DeleteAsync(qrCode);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var qrCode = await _repository.GetByIdAsync(id);
        if (qrCode == null)
        {
            return false;
        }

        qrCode.IsActive = !qrCode.IsActive;
        qrCode.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(qrCode);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task RecordPrintAsync(int id)
    {
        await _repository.UpdatePrintCountAsync(id);
    }

    public async Task<QRCodeStatistics> GetStatisticsAsync()
    {
        var totalCount = await _repository.GetQRCodeCountAsync();
        var activeCount = await _repository.GetActiveQRCodeCountAsync();

        // 计算总打印次数
        var allQRCodes = await _repository.GetAllAsync();
        var totalPrintCount = allQRCodes.Sum(q => q.PrintCount);

        return new QRCodeStatistics
        {
            TotalCount = totalCount,
            ActiveCount = activeCount,
            TotalPrintCount = totalPrintCount
        };
    }

    private QRCodeResponse ToResponse(QRCode qrCode)
    {
        // 同步方法中无法使用async，如果需要员工姓名请使用ToResponseAsync
        return new QRCodeResponse
        {
            Id = qrCode.Id,
            Code = qrCode.Code,
            MeatTypeId = qrCode.MeatTypeId,
            MeatTypeName = qrCode.MeatType?.Name ?? string.Empty,
            MeatTypeCode = qrCode.MeatType?.Code ?? string.Empty,
            EmployeeCode = qrCode.EmployeeCode,
            EmployeeName = null, // 同步方法无法查询，使用ToResponseAsync获取员工姓名
            Content = qrCode.Content,
            ImageBase64 = qrCode.ImageBase64,
            BatchNumber = qrCode.BatchNumber,
            PrintCount = qrCode.PrintCount,
            LastPrintedAt = qrCode.LastPrintedAt,
            IsActive = qrCode.IsActive,
            Remarks = qrCode.Remarks,
            CreatedAt = qrCode.CreatedAt,
            UpdatedAt = qrCode.UpdatedAt
        };
    }

    private async Task<QRCodeResponse> ToResponseAsync(QRCode qrCode)
    {
        string? employeeName = null;
        if (!string.IsNullOrEmpty(qrCode.EmployeeCode))
        {
            var employee = await _employeeRepository.GetByCodeAsync(qrCode.EmployeeCode);
            employeeName = employee?.Name;
        }

        return new QRCodeResponse
        {
            Id = qrCode.Id,
            Code = qrCode.Code,
            MeatTypeId = qrCode.MeatTypeId,
            MeatTypeName = qrCode.MeatType?.Name ?? string.Empty,
            MeatTypeCode = qrCode.MeatType?.Code ?? string.Empty,
            EmployeeCode = qrCode.EmployeeCode,
            EmployeeName = employeeName,
            Content = qrCode.Content,
            ImageBase64 = qrCode.ImageBase64,
            BatchNumber = qrCode.BatchNumber,
            PrintCount = qrCode.PrintCount,
            LastPrintedAt = qrCode.LastPrintedAt,
            IsActive = qrCode.IsActive,
            Remarks = qrCode.Remarks,
            CreatedAt = qrCode.CreatedAt,
            UpdatedAt = qrCode.UpdatedAt
        };
    }
}
