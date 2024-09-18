using FastEndpoints;

namespace WebSite_ServerSide.Features.Prueba;

internal record HelloResponse(string Message);

internal sealed class GetName: EndpointWithoutRequest<HelloResponse>
{
  

    public override void Configure()
    {
        Get("/product");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(new HelloResponse("Hello"), 200, ct);
    }
}