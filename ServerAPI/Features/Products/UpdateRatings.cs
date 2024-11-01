using ServerAPI.Data;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ServerAPI.Features.Ratings;

public class NewRatingRequest
{
    public int ProductId { get; set; }
    public int Rating { get; set; }
}

public class NewRatingResponse(string message);

public class UpdateRatings: Endpoint<NewRatingRequest, NewRatingResponse>
{
    private readonly DataBase _context;

    public UpdateRatings(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/update/ratings");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
        Validator<RatingValidator>();
    }
    public override async Task HandleAsync(NewRatingRequest req, CancellationToken ct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p=> p.Id == req.ProductId,ct);
        var total = product.Rating / 10 * (product.NRatings * 10) + req.Rating;
        product.NRatings += 1;
        product.Rating = total / (product.NRatings * 10) * 10 ;
        try
        {
            await _context.SaveChangesAsync(ct);
            await SendAsync(new NewRatingResponse("Rating updated"), 200, ct);
        }
        catch (Exception ex)
        {
            // Manejo de excepciones adecuado (log, etc.)
            await SendAsync(new NewRatingResponse("Error updating rating"), 500, ct);
        }
    }
}
internal class RatingValidator : Validator<NewRatingRequest>
{
    public RatingValidator()
    {
        RuleFor(r=>r.ProductId)
            .NotEmpty()
            .WithMessage("Product Id is required")
            .NotEqual(0)
            .WithMessage("ID should not be provided and will be generated automatically.");

        RuleFor(r => r.Rating)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Rating must be between 0 and 10.")
            .LessThanOrEqualTo(10)
            .WithMessage("Rating must be between 0 and 10.");
    }
}