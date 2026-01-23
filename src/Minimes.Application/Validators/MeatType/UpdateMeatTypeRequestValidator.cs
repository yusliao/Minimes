using FluentValidation;
using Minimes.Application.DTOs.MeatType;

namespace Minimes.Application.Validators.MeatType;

/// <summary>
/// 更新肉类类型请求验证器
/// </summary>
public class UpdateMeatTypeRequestValidator : AbstractValidator<UpdateMeatTypeRequest>
{
    public UpdateMeatTypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("类型名称不能为空")
            .MaximumLength(100).WithMessage("类型名称长度不能超过100个字符");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("显示顺序必须大于等于0");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("描述说明长度不能超过500个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
