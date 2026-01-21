using FluentValidation;
using Minimes.Application.DTOs.User;

namespace Minimes.Application.Validators.User;

/// <summary>
/// 用户登录请求验证器
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}
