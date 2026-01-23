using FluentValidation;
using Minimes.Application.DTOs.QRCode;

namespace Minimes.Application.Validators.QRCode;

/// <summary>
/// 更新二维码请求验证器
/// </summary>
public class UpdateQRCodeRequestValidator : AbstractValidator<UpdateQRCodeRequest>
{
    public UpdateQRCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("用户编号不能为空")
            .MaximumLength(50).WithMessage("用户编号长度不能超过50个字符");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("备注说明长度不能超过500个字符")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}
