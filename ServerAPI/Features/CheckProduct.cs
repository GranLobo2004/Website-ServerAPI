using ServerAPI.Entities;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features;

internal record ProductRequest(Product Product);

internal record ProductResponse(bool Variable, string Message);

internal sealed class CheckProduct : Endpoint<ProductRequest, ProductResponse>
{
    private readonly DataBase _context;

    public CheckProduct(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/product");
        AllowAnonymous();
        Validator<ProductValidator>();
    }

    public override async Task HandleAsync(ProductRequest req, CancellationToken ct)
    {
        var product = req.Product;
        product.Id = 0;

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);
        await SendAsync(new ProductResponse(true, "Producto registrado correctamente"), 200, ct);
    }
}

internal class ProductValidator : Validator<ProductRequest>
{
    public ProductValidator()
    {
        RuleFor(p => p.Product.Id)
            .Equal(0)
            .WithMessage("ID should not be provided and will be generated automatically.");

        RuleFor(p => p.Product.Name)
            .NotEmpty()
            .WithMessage("Name required for the product")
            .NotNull()
            .WithMessage("Name required");

        RuleFor(p => p.Product.Description)
            .NotEmpty()
            .WithMessage("Description required for the product")
            .NotNull()
            .WithMessage("Description required");

        RuleFor(p => p.Product.Price)
            .NotEmpty()
            .WithMessage("Price required")
            .NotNull()
            .WithMessage("Price required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be 0 or negative");
    }
}