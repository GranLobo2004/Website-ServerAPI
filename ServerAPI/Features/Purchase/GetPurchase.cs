using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Order;

public record OrderResponse
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public List<Product> Products { get; set; }
    public string State { get; set; }
    public DateTime Date { get; set; }
    public List<int> Quantity { get; set; }
}

public class GetPurchase : EndpointWithoutRequest<OrderResponse>
{
    private readonly DataBase _context;

    public GetPurchase(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/purchase/{id}");  // Asegúrate de que la ruta sea correcta
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var id = Route<int>("id");
            
            // Recuperamos la compra junto con los productos y las cantidades asociadas
            Purchase purchase = await _context.Purchases.FirstOrDefaultAsync(o => o.Id == id, ct);

            // Si no se encuentra la compra, se retorna un 404
            if (purchase == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }
            
            List<ProductPurchaseUser> productPurchaseUsers = await _context.ProductPurchaseUsers.Where(o => o.PurchaseId == purchase.Id).ToListAsync(ct);
            List<Product> products = new List<Product>();
            List<int> quantities = new List<int>();
            
            for (int i = 0; i < productPurchaseUsers.Count; i++)
            {
                Product product = _context.Products.FirstOrDefault(p=>p.Id == productPurchaseUsers[i].ProductId);
                products.Add(product);
                quantities.Add(productPurchaseUsers[i].Quantity);
            }
            
            var response = new OrderResponse()
            {
                Id = purchase.Id,
                Total = purchase.Total,
                State = purchase.State,
                Date = purchase.Date,
                Products = products, // Lista de productos vacía
                Quantity = quantities, // Lista de cantidades vacía
            };

            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            // Captura de excepciones
            Console.WriteLine(ex.Message);
            await SendErrorsAsync();
        }
    }
}
