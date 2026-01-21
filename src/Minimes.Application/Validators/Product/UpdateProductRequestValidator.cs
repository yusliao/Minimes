using FluentValidation;
using Minimes.Application.DTOs.Product;

namespace Minimes.Application.Validators.Product;

/// <summary>
/// 更新商品请求验证器
/// </summary>
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("商品ID必须大于0");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("条形码不能为空")
            .MaximumLength(50).WithMessage("条形码长度不能超过50个字符")
            .Matches(@"^[0-9a-zA-Z-]+$").WithMessage("条形码只能包含数字、字母和连字符");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("商品名称不能为空")
            .MaximumLength(100).WithMessage("商品名称长度不能超过100个字符");

        RuleFor(x => x.Specification)
            .MaximumLength(200).WithMessage("规格说明长度不能超过200个字符")
            .When(x => !string.IsNullOrEmpty(x.Specification));

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("计量单位不能为空")
            .MaximumLength(20).WithMessage("计量单位长度不能超过20个字符");

        RuleFor(x => x.ReferencePrice)
            .GreaterThan(0).WithMessage("参考价格必须大于0")
            .When(x => x.ReferencePrice.HasValue)
            .PrecisionScale(10, 2, true).WithMessage("参考价格最多只能有2位小数");
    }
}
