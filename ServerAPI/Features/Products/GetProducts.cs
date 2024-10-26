using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features;

public record GetProductsRequest (string Name, int MinPrice, int MaxPrice, float MinRating, int NRatings, List<string> Tags);



public class GetProducts: Endpoint<GetProductsRequest, List<GetProductResponse>>
{
    
    private readonly DataBase _context;

    public GetProducts(DataBase context)
    {
        _context = context;
    }
    
    public override void Configure()
    {
        Post("/products");
        AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
    }
    public override async Task HandleAsync(GetProductsRequest req, CancellationToken ct)
    {
        try
        {

            var products = await _context.Products.ToListAsync(ct);
            
            if (req.Name != "" || req.Tags.Count > 0  || req.MinPrice >= 0 || req.MaxPrice >= 0 || req.MinRating >=0 ||req.NRatings >= 0)
            {
                if (req.Name != "")
                {
                    products = products.Where(p => p.Name.ToLower().Contains(req.Name.ToLower())).ToList();
                }

                if (req.MinPrice >= 0)
                {
                    products = products.Where(p => p.Price >= req.MinPrice).ToList();
                }

                
                if (req.MaxPrice > 0)
                {
                    products = products.Where(p => p.Price <= req.MaxPrice).ToList();
                }

                if (req.MinRating >= 0)
                {
                    products = products.Where(p => p.Rating >= req.MinRating).ToList();
                }

                
                if (req.NRatings >= 0)
                {
                    products = products.Where(p => p.NRatings >= req.NRatings).ToList();
                }

                
                if (req.Tags.Count > 0)
                {
                    Console.WriteLine(products.Count);
                    for (int i = 0; i < req.Tags.Count; i++)
                    {
                        Console.WriteLine(req.Tags[i]);
                        products = products.Where(p => p.Tags.Contains(req.Tags[i])).ToList();
                    }
                    Console.WriteLine(products.Count);
                }

            }
            var response = products.Select(p=>new GetProductResponse
            {
                Product = p
            }).ToList();
            
            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            await SendErrorsAsync(500, ct);
        }
    }
}