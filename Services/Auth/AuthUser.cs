using System.Security.Claims;

namespace dotnet_rpg.Services.Auth;

public abstract class AuthUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public int GetAuthUserId()
    {
        var claims = _httpContextAccessor.HttpContext!.User;
        // we set the user if to NameIdentifier claimType
        // claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        var nameIdentifier = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return int.Parse(nameIdentifier);
    }
}