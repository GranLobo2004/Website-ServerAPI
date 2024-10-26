using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features.Updates;

internal sealed class UpdateProduct : Endpoint<ProductRequest, ProductResponse>
{
    private readonly DataBase _context;

    public UpdateProduct(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/update/product");
        AllowAnonymous();
        Validator<UpdateProductValidator>();
    }

    public override async Task HandleAsync(ProductRequest req, CancellationToken ct)
    {
        var product = req.Product;
        // Realizar validaciones adicionales si es necesario

        try
        { 
            var savedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id, ct);
            if (savedProduct == null)
            {
                await SendAsync(new ProductResponse(false, "Product does not exist in database", 0), 400, ct);

            }
            savedProduct.Characteristics = product.Characteristics;
            savedProduct.Name = product.Name;
            savedProduct.Price = product.Price;
            savedProduct.Description = product.Description;
            savedProduct.Tags = product.Tags;
            _context.Products.Update(savedProduct);
            await _context.SaveChangesAsync(ct);
            await SendAsync(new ProductResponse(true, "Product updated", savedProduct.Id), 200, ct);
        }
        catch (Exception ex)
        {
            await SendAsync(new ProductResponse(false, "Internal server error", 0), 500, ct);
        }
    }
}

internal class UpdateProductValidator : Validator<ProductRequest>
{
    public UpdateProductValidator()
    {
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
        RuleFor(p => p.Product.Characteristics)
            .NotEmpty()
            .WithMessage("Characteristic required for the product")
            .NotNull()
            .WithMessage("Characteristic required");
        RuleFor(p => p.Product.Price)
            .NotEmpty()
            .WithMessage("Price required")
            .NotNull()
            .WithMessage("Price required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be 0 or negative");
    }
}