using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace ServerAPI.Features.Auth;

public record LoginRequest(string Email, string Password);

public class Login : Endpoint<LoginRequest>
{
    private readonly UserManager<IdentityUser> _userManager;
    
    private readonly SignInManager<IdentityUser> _signInManager;
    
    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
        
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var result = await _signInManager.PasswordSignInAsync(user, req.Password, true, false);

        if (!result.Succeeded)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.UserData, user.Id),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    }
}
