using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Order;

public record PurchaseRequest
{
    public int CustomerId { get; init; }
    public int ProductId { get; init; }
}

public class GetPurchases: Endpoint<PurchaseRequest, List<Purchase>>
{
    private readonly DataBase _context;

    public GetPurchases (DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/purchases");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
    }

    public override async Task HandleAsync(PurchaseRequest req, CancellationToken ct)
    {
        try
        {
            Console.WriteLine(req.ProductId);
            List<ProductPurchaseUser> orders;
            if (req.ProductId == 0)
            {
                var adminResponse = await _context.Purchases.ToListAsync(ct);
                await SendAsync(adminResponse, 200, ct);
                orders = new List<ProductPurchaseUser>();
            }
            else if (req.CustomerId == 0 || req.CustomerId == null)
            {
                Console.WriteLine("Producto");
                orders = await _context.ProductPurchaseUsers.Where(o => o.ProductId == req.ProductId).ToListAsync(ct);
            }
            else
            {
                Console.WriteLine("Usuario");
                orders = await _context.ProductPurchaseUsers.Where(o => o.UserId == req.CustomerId).ToListAsync();
                Console.WriteLine(orders.Count);
            }
            
            List<Purchase> response = new List<Purchase>();
            HashSet<int> set = new HashSet<int>();

            
            if (orders.Count == 0)
            {
                await SendAsync(response, 404, ct);
            }
            
            for (int i = 0; i < orders.Count; i++)
            {
                if (!set.Contains(orders[i].PurchaseId))
                {
                    var order = await _context.Purchases.FirstOrDefaultAsync(o => o.Id == orders[i].PurchaseId, ct);
                    response.Add(order);
                    set.Add(order.Id);
                }
            }
            
            await SendAsync(response, 200, ct);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}