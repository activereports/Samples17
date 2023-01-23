Imports System.Security.Claims
Imports System.Text
Imports System.Text.Encodings.Web
Imports Microsoft.AspNetCore.Authentication
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Options

Public Class BasicAuthentication
	Inherits AuthenticationHandler(Of BasicAuthenticationSchemeOptions)
	
	Sub New(options As IOptionsMonitor(Of BasicAuthenticationSchemeOptions), logger As ILoggerFactory, encoder As UrlEncoder, clock As ISystemClock)
		MyBase.New(options, logger, encoder, clock)
	End Sub
	
	Protected Overrides Function HandleAuthenticateAsync() As Task(Of AuthenticateResult)
		Dim authHeader = Request.Headers("Authorization").ToString()
		
		If String.IsNullOrWhiteSpace(authHeader)
			Return Task.FromResult(AuthenticateResult.Fail("No Authorization Header"))
		End If
		
		If Not authHeader.StartsWith("Basic ", StringComparison.InvariantCultureIgnoreCase)
			Return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"))
		End If
		
		Dim token = authHeader.Substring("Basic ".Length)
		Dim credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(":")
		Dim claims As Claim()
		If Not IsAuthenticated(credentials, claims)
			Return Task.FromResult(AuthenticateResult.Fail("Invalid login or password"))
		End If
		
		Dim identity = new ClaimsIdentity(claims, "Basic")
		Dim claimsPrincipal = new ClaimsPrincipal(identity)
		Return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)))
	End Function
	
	Protected Overrides Function HandleChallengeAsync(properties As AuthenticationProperties) As Task
		Response.Headers.Add("WWW-Authenticate", $"Basic realm={Request.Host.Value + Request.Path}")
		Return MyBase.HandleChallengeAsync(properties)
	End Function
	
	Private Function IsAuthenticated(credentials As String(), ByRef claims As Claim()) As Boolean
		If credentials(0).Equals("admin") And credentials(1).Equals("1")
			claims = New Claim() { New Claim(ClaimTypes.Name, credentials(0)), new Claim(ClaimTypes.Role, "Admin") }
			Return True
		End If
		Return False
	End Function
	
End Class

Public Class BasicAuthenticationSchemeOptions
	Inherits AuthenticationSchemeOptions
End Class