using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebService;

public class BasicAuthentication : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
	public BasicAuthentication(IOptionsMonitor<BasicAuthenticationSchemeOptions> options, ILoggerFactory logger,
		UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
	
	protected override Task HandleChallengeAsync(AuthenticationProperties properties)
	{
		Response.Headers.Add("WWW-Authenticate", $"Basic realm={Request.Host.Value + Request.Path}");
		return base.HandleChallengeAsync(properties);
	}
	
	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{ 
		var authHeader = Request.Headers["Authorization"].ToString();
		
		if (string.IsNullOrWhiteSpace(authHeader))
			return Task.FromResult(AuthenticateResult.Fail("No Authorization Header"));
		
		if (!authHeader.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase))
			return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));

		var token = authHeader["Basic ".Length..].Trim();
		var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

		if (!IsAuthenticated(credentials, out var claims))
			return Task.FromResult(AuthenticateResult.Fail("Invalid login or password"));
		
		var identity = new ClaimsIdentity(claims, "Basic");
		var claimsPrincipal = new ClaimsPrincipal(identity);
		return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
		
	}
	private bool IsAuthenticated(string[] credentials, out Claim[]? claims)
	{
		if (credentials[0].Equals("admin") && credentials[1].Equals("1"))
		{
			claims = new[] { new Claim(ClaimTypes.Name, credentials[0]), new Claim(ClaimTypes.Role, "Admin") };
			return true;
		}
		claims = null;
		return false;
	}
}
	public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions {  }