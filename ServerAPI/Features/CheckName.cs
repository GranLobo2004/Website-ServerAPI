using FastEndpoints;
using FluentValidation;

namespace ServerAPI.Features;

internal record NameRequest(string Name);

internal record NameResponse(bool IsSuccess, string Message);

internal sealed class CheckName:Endpoint<NameRequest, NameResponse>
{
    public override void Configure()
    {
        Post("/name");
        AllowAnonymous();
        Validator<NameValidator>();
    }

    public override async Task HandleAsync(NameRequest req, CancellationToken ct)
    {
        await SendAsync(new NameResponse(true, "Nombre correcto"), 200, ct);
    }
}

internal class NameValidator : Validator<NameRequest>
{
    public NameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotEqual("hola");
    }
}