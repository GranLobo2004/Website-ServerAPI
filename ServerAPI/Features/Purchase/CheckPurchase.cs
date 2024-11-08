using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;


namespace ServerAPI.Features.Order;

public record CheckOrderRequest
{
    public int OrderId { get; init; }
    public decimal Total { get; init; }
    public List<int> ProductIds { get; init; }
    public int CustomerId { get; init; }
    public List<int> Quantities { get; init; }
}

public record CheckOrderResponse(string Message);

public class CheckPurchase: Endpoint<CheckOrderRequest, CheckOrderResponse>
{
    private readonly DataBase _context;

    public CheckPurchase (DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/purchase");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
        Validator<OrderValidator>();
    }

    public override async Task HandleAsync(CheckOrderRequest req, CancellationToken ct)
    {
        try
        {
            Console.WriteLine(req.Quantities.Count + " quantities");
            Entities.Purchase purchase = new Entities.Purchase
            {
                Id = 0,
                Total = req.Total,
                State = "Pending Review",
                Date = DateTime.Now,
            };
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.CustomerId);
            if (user == null)
            {
                await SendAsync(new CheckOrderResponse("Customer not found"), 404, ct);
            }
            _context.Purchases.Add(purchase);
            _context.SaveChanges();
            purchase = await _context.Purchases.OrderByDescending(o => o.Id).FirstOrDefaultAsync();
            for (int i = 0; i < req.ProductIds.Count; i++)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == req.ProductIds[i]);
                if (product == null)
                {
                    _context.Purchases.Remove(purchase);
                    await SendAsync(new CheckOrderResponse("Product not found"), 404, ct);
                }
                else if (req.Quantities[i] <= 0)
                {
                    _context.Purchases.Remove(purchase);
                    await SendAsync(new CheckOrderResponse("Quantities must be more than 0"), 400, ct);
                }
                else
                {
                    ProductPurchaseUser productPurchaseUser = new ProductPurchaseUser()
                    {
                        PurchaseId = purchase.Id,
                        UserId = user.Id,
                        ProductId = product.Id,
                        Quantity = req.Quantities[i],
                    };
                    _context.ProductPurchaseUsers.Add(productPurchaseUser);
                }
                _context.SaveChanges();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
internal class OrderValidator : Validator<CheckOrderRequest>
{
    public OrderValidator()
    {
        RuleFor(o => o.OrderId)
            .Equal(0)
            .WithMessage("Order ID must be 0");

        RuleFor(o => o.Total)
            .GreaterThan(0)
            .WithMessage("Total cannot be less than 0");
        
        RuleFor(o => o.ProductIds)
            .NotEmpty()
            .NotNull()
            .WithMessage("ProductId is required and must not be 0");

        RuleFor(o => o.ProductIds.Count)
            .GreaterThan(0)
            .WithMessage("Products can't be empty");
        
        RuleFor(o => o.CustomerId)
            .NotEmpty()
            .NotNull()
            .WithMessage("Customer ID is required");
        
        RuleFor(o => o.Quantities)
            .NotEmpty()
            .NotNull()
            .WithMessage("Quantities is required");
        
        RuleFor(o => o.Quantities.Count)
            .GreaterThan(0);
    }
}