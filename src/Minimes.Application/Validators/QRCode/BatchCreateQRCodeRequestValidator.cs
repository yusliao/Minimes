using FluentValidation;
using Minimes.Application.DTOs.QRCode;

namespace Minimes.Application.Validators.QRCode;

/// <summary>
/// 批量创建二维码请求验证器
/// </summary>
public class BatchCreateQRCodeRequestValidator : AbstractValidator<BatchCreateQRCodeRequest>
{
    public BatchCreateQRCodeRequestValidator()
    {
        RuleFor(x => x.MeatTypeId)
            .GreaterThan(0).WithMessage("肉类类型ID必须大于0");

        RuleFor(x => x.StartNumber)
            .GreaterThanOrEqualTo(1).WithMessage("起始编号必须大于等于1");

        RuleFor(x => x.EndNumber)
            .GreaterThanOrEqualTo(1).WithMessage("结束编号必须大于等于1")
            .GreaterThanOrEqualTo(x => x.StartNumber).WithMessage("结束编号必须大于等于起始编号");

        RuleFor(x => x.PaddingLength)
            .GreaterThanOrEqualTo(1).WithMessage("补零位数必须大于等于1")
            .LessThanOrEqualTo(10).WithMessage("补零位数不能超过10");

        RuleFor(x => x.Prefix)
            .MaximumLength(10).WithMessage("前缀长度不能超过10个字符")
            .When(x => !string.IsNullOrEmpty(x.Prefix));

        RuleFor(x => x.BatchNumber)
            .MaximumLength(50).WithMessage("批次号长度不能超过50个字符")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));
    }
}
