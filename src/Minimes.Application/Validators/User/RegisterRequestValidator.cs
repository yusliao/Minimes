using FluentValidation;
using Minimes.Application.DTOs.User;

namespace Minimes.Application.Validators.User;

/// <summary>
/// 用户注册请求验证器
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .Length(4, 50).WithMessage("用户名长度必须在4-50个字符之间")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线");

        // 内部系统，简化密码规则：4-6位即可
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .Length(4, 6).WithMessage("密码长度必须在4-6个字符之间");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("确认密码不能为空")
            .Equal(x => x.Password).WithMessage("确认密码与密码不一致");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("姓名不能为空")
            .Length(2, 50).WithMessage("姓名长度必须在2-50个字符之间");
    }
}
