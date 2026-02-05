using FluentValidation;
using Minimes.Application.DTOs.Employee;

namespace Minimes.Application.Validators.Employee;

/// <summary>
/// 更新员工请求验证器
/// </summary>
public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("员工ID必须大于0");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("员工代码不能为空")
            .MaximumLength(50).WithMessage("员工代码长度不能超过50个字符")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("员工代码只能包含字母、数字、下划线和连字符");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("员工姓名不能为空")
            .MaximumLength(100).WithMessage("员工姓名长度不能超过100个字符");

        RuleFor(x => x.ContactPerson)
            .MaximumLength(50).WithMessage("联系人长度不能超过50个字符")
            .When(x => !string.IsNullOrEmpty(x.ContactPerson));

        RuleFor(x => x.Phone)
            .Matches(@"^1[3-9]\d{9}$|^0\d{2,3}-?\d{7,8}$").WithMessage("电话号码格式不正确")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("地址长度不能超过200个字符")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}
