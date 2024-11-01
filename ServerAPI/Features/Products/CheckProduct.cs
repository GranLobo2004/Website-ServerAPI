using ServerAPI.Entities;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using Microsoft.Extensions.Logging;

namespace ServerAPI.Features;

internal record ProductRequest(Product Product);

public partial record ProductResponse(bool Variable, string Message, int ProductId);

internal sealed class CheckProduct : Endpoint<ProductRequest, ProductResponse>
{
    private readonly DataBase _context;
    private readonly ILogger<CheckProduct> _logger;

    public CheckProduct(DataBase context, ILogger<CheckProduct> logger)
    {
        _context = context;
        _logger = logger;
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
        // Realizar validaciones adicionales si es necesario
        if (product == null)
        {
            await SendAsync(new ProductResponse(false, "Invalid product data", 0), 400, ct);
            return;
        }

        try
        {
            product.Id = 0;
            product.Rating = 0; // Asegurarse de que el rating sea 0
            product.NRatings = 0;
            _context.Products.Add(product);
            
            await _context.SaveChangesAsync(ct);
            var Product = _context.Products.OrderByDescending(p => p.Id).FirstOrDefault();
            await SendAsync(new ProductResponse(true, "Producto registrado correctamente", Product.Id), 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product.");
            await SendAsync(new ProductResponse(false, "Internal server error", 0), 500, ct);
        }
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