using FastEndpoints;
using FluentValidation;
using Namotion.Reflection;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features;

public class CommentRequest
{
    public Comment Comment { get; set; }
    public int Id { get; set; }  // 'Id' sigue la convención PascalCase
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
        Post("/comment/{id}");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
        Validator<CommentValidator>();
    }

    public override async Task HandleAsync(CommentRequest req, CancellationToken ct)
    {
        var comment = req.Comment;
        comment.Id = 0;  // Asegúrate de que el ID sea 0 para que se genere automáticamente
        comment.ProductId = req.Id;  // Establece ProductId desde el id de la URL

        _context.Comments.Add(comment); 
        await _context.SaveChangesAsync(ct);
        await SendAsync(new SavedComment("Comment saved"), 200, ct);
    }
}

internal class CommentValidator : Validator<CommentRequest>
{
    public CommentValidator()
    {
        RuleFor(c => c.Comment.Id)
            .Equal(0)
            .WithMessage("ID should not be provided and will be generated automatically.");

        RuleFor(c => c.Comment.User)
            .NotEmpty()
            .WithMessage("User name required for the comment");

        RuleFor(c => c.Comment.Text)
            .NotEmpty()
            .WithMessage("Text required for the comment");

        RuleFor(c => c.Comment.Date)
            .NotEmpty()
            .WithMessage("Date required for the comment");
        // Puedes quitar la comparación de la fecha, a menos que sea necesaria
    }
}