Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.DependencyInjection

Module Program
	Sub Main(args As String())
		BuildWebApplication(args).Run()
	End Sub
	
	Private Function BuildWebApplication(args as String()) As WebApplication
		Dim builder = WebApplication.CreateBuilder()
		
		builder.Services.
			AddAuthentication(NameOf(BasicAuthentication)).
			AddScheme (Of BasicAuthenticationSchemeOptions, BasicAuthentication)(NameOf(BasicAuthentication), Nothing)
		
		builder.Services.
			AddControllers().
			AddJsonOptions(Sub (options) options.JsonSerializerOptions.PropertyNamingPolicy = Nothing)
		
		Dim app = builder.Build()
		
		app.Map("/", Function(ctx) ctx.Response.WriteAsync(My.Resources.Resources.bodyOfMessage))
		
		app.UseAuthorization()
		
		app.MapControllers()
		
		Return app
	End Function
End Module
