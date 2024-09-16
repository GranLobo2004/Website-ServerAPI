using ServerAPI.Entities;
using FastEndpoints;
using FluentValidation;

namespace ServerAPI.Features;

internal record ProductRequest (Product product);

internal record ProductResponse (string message);

internal sealed class CheckProduct:Endpoint<ProductRequest, ProductResponse>
{
    public override void Configure()
    {
        Post("/product");
        AllowAnonymous();
        Validator<ProductValidator>();
    }

    public override async Task HandleAsync(ProductRequest req, CancellationToken ct)
    {
        await SendAsync(new ProductResponse ("Producto registrado correctamente"), 200, ct);
    }
}

internal class ProductValidator : Validator<ProductRequest>
{
    public ProductValidator()
    {
        RuleFor(p => p.product.Name)
            .NotEmpty()
            .NotNull();
        RuleFor(p => p.product.Description)
            .NotEmpty()
            .NotNull();
        RuleFor(p => p.product.Price)
            .NotEmpty()
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}