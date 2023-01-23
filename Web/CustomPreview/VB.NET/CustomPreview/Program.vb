Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Extensions.DependencyInjection

Module Program
    Sub Main(args As String())
        BuildWebApplication(args).Run()
    End Sub

    Private Function BuildWebApplication(args as String()) As WebApplication
	    Dim builder = WebApplication.CreateBuilder()
	    builder.Services.AddSingleton(Of IMemoryCache, MemoryCache)
	    
	    Dim app = builder.Build()
	    app.UseStaticFiles()

	    app.MapReporting()
	    app.UseMiddleware(Of IndexPageMiddleware)

	    Return app

    End Function
End Module