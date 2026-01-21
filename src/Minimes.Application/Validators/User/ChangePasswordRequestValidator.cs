using FluentValidation;
using Minimes.Application.DTOs.User;

namespace Minimes.Application.Validators.User;

/// <summary>
/// 修改密码请求验证器
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("用户ID必须大于0");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("旧密码不能为空");

        // 内部系统，简化密码规则：4-6位即可
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新密码不能为空")
            .Length(4, 6).WithMessage("新密码长度必须在4-6个字符之间")
            .NotEqual(x => x.OldPassword).WithMessage("新密码不能与旧密码相同");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("确认新密码不能为空")
            .Equal(x => x.NewPassword).WithMessage("确认新密码与新密码不一致");
    }
}
