using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Features.Users;

namespace ServerAPI.Features;
using FastEndpoints;

public record DeleteCommentRequest
{
    public int Id { get; set; }
    public int ProductId { get; set; }
}

public record DeleteCommentResponse(string Message);

public class DeleteComment:Endpoint<DeleteCommentRequest, DeleteCommentResponse>
{
    private readonly DataBase _context;

    public DeleteComment(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/comment/delete");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteCommentRequest req, CancellationToken ct)
    {
        try
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(p => p.Id == req.Id && p.ProductId == req.ProductId, ct);
            if (comment == null)
            {
                await SendAsync(new DeleteCommentResponse("Comment not found."), 404, ct);
            }
            else
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
                await SendAsync(new DeleteCommentResponse("Comment deleted."), 200, ct);
            }
        }
        catch (Exception ex)
        {
            await SendAsync(new DeleteCommentResponse(ex.Message), 400, ct);
        }
    }
}