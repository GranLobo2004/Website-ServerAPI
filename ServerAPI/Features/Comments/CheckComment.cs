using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features;

public class CommentRequest
{
    public string Username { get; set; }
    public string content { get; set; }
    public int productId { get; set; }
}

public partial record SavedComment(string Message);

public class CheckComment : Endpoint<CommentRequest, SavedComment>
{
    private readonly DataBase _context;

    public CheckComment(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/comment");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
        Validator<CommentValidator>();
    }

    public override async Task HandleAsync(CommentRequest req, CancellationToken ct)
    {
        Console.WriteLine("Saving comment...");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user == null)
        {
            await SendAsync(new SavedComment("User not found or Username invalid"), 404, ct);
        }

        var comments = await _context.Comments.Where(c => c.ProductId == req.productId).ToListAsync();
        
        var comment = new Comment()
        {
            Id = comments.Count + 1,
            Text = req.content,
            Date = DateTime.Today,
            ProductId = req.productId,
            Username = user.Username,
            UserImage = user.Image
        };
        _context.Comments.Add(comment); 
        await _context.SaveChangesAsync(ct);
        await SendAsync(new SavedComment("Comment saved"), 200, ct);
    }
}

internal class CommentValidator : Validator<CommentRequest>
{
    public CommentValidator()
    {
        RuleFor(c => c.Username)
            .NotEmpty()
            .WithMessage("User name required for the comment");

        RuleFor(c => c.content)
            .NotEmpty()
            .WithMessage("Text required for the comment");
        // Puedes quitar la comparación de la fecha, a menos que sea necesaria
        RuleFor(c => c.productId)
            .NotEmpty()
            .NotNull()
            .NotEqual(0)
            .WithMessage("ProductId is required and must not be 0");
    }
}