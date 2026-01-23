using FluentValidation;
using Minimes.Application.DTOs.QRCode;

namespace Minimes.Application.Validators.QRCode;

/// <summary>
/// 创建二维码请求验证器
/// </summary>
public class CreateQRCodeRequestValidator : AbstractValidator<CreateQRCodeRequest>
{
    public CreateQRCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("用户编号不能为空")
            .MaximumLength(50).WithMessage("用户编号长度不能超过50个字符");

        RuleFor(x => x.MeatTypeId)
            .GreaterThan(0).WithMessage("肉类类型ID必须大于0");

        RuleFor(x => x.BatchNumber)
            .MaximumLength(50).WithMessage("批次号长度不能超过50个字符")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("备注说明长度不能超过500个字符")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}
