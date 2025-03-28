using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CartonCaps;


/// <summary>
/// This middleware is used to fake a JWT token for testing purposes.
/// </summary>
public class FakeJwtUserMiddleware
{
    private readonly RequestDelegate _next;

    public FakeJwtUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token); // no signature validation

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
                if (userIdClaim != null)
                {
                    var claimsIdentity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value)
                    }, "FakeJwt");

                    context.User = new ClaimsPrincipal(claimsIdentity);
                }
            }
            catch
            {
                // TODO: Log error and handle gracefully
            }
        }

        await _next(context);
    }
}
