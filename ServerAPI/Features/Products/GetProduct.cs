namespace ServerAPI.Features
{
    using FastEndpoints;
    using Microsoft.EntityFrameworkCore;
    using ServerAPI.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetProductRequest
    {
        public int Id { get; set; }
    }

    public class CommentResponse
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
    
    public class GetProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Characteristics { get; set; }
        public string Image { get; set; }
        public List<string> Tags { get; set; }
        public List<CommentResponse> Comments { get; set; }
        public float Rating { get; set; }
        public int NRatings { get; set; }
    }

    public class GetProduct : Endpoint<GetProductRequest, GetProductResponse>
    {
        private readonly DataBase _context;

        public GetProduct(DataBase context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("/product/{id}");
            AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
        }

        public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

                if (product == null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }

                var response = new GetProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Tags = product.Tags,
                    Characteristics = product.Characteristics,
                    Description = product.Description,
                    Rating = product.Rating,
                    NRatings = product.NRatings,
                    Comments = product.Comments.Select(c => new CommentResponse
                    {
                        Id = c.Id,
                        User = c.User,
                        Text = c.Text,
                        Date = c.Date
                    }).ToList()
                };

                await SendAsync(response, 200, ct);
            }
            catch (Exception ex)
            {
                // Manejar el error y enviar una respuesta de error
                await SendErrorsAsync(500, ct);
            }
        }
    }
}
