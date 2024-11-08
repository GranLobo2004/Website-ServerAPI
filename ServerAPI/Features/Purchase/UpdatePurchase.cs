using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Order;

public record UpdatePurchaseRequest
{
    public int Id { get; init; }
    public string State { get; init; }
}

public class UpdatePurchase: Endpoint<UpdatePurchaseRequest>
{
    private readonly DataBase _context;

    public UpdatePurchase (DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/purchase/update");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
        Validator<PurchaseValidator>();
    }

    public override async Task HandleAsync(UpdatePurchaseRequest req, CancellationToken ct)
    {
        try
        {
           var Purchase = await _context.Purchases.FirstOrDefaultAsync(p=> p.Id == req.Id);
           if (Purchase == null)
           {
               await SendAsync("Purchase not found",404, ct);
           }
           Purchase.State = req.State;
           _context.Purchases.Update(Purchase);
           _context.SaveChanges();
           await SendAsync("Updated", 200, ct);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

internal class PurchaseValidator : Validator<UpdatePurchaseRequest>
{
    public PurchaseValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
            .NotNull()
            .WithMessage("Id is required");
        RuleFor(p => p.State)
            .Must(State => new[] { "On Hold", "Canceled", "Processing", "In transit", "Delivered", "Pending Review", "Returned" }.Contains(State))
            .WithMessage("Invalid State");
    }
}
