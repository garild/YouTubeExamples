using FluentValidation;

namespace Products.Api.Endpoints.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(150).WithMessage("Product name cannot exceed 150 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than 0.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("Product SKU is required.")
            .MaximumLength(50).WithMessage("Product SKU cannot exceed 50 characters.");
    }
}
