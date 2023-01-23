Imports System
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.OData
Imports Microsoft.AspNetCore.OData.Batch
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.OData.Edm
Imports Microsoft.OData.ModelBuilder
Imports ODataEndPoint.Models

Module Program
	Sub Main(args As String())
		BuildWebApplication(args).Run()
	End Sub
	
	Private Function BuildWebApplication(args As String()) As WebApplication
		Dim builder = WebApplication.CreateBuilder()
		
		builder.Services.
			AddControllers().
			AddOData(sub(options) options.AddRouteComponents(GetEdmModel(), new DefaultODataBatchHandler()).Filter().Select().Expand())
		
		Dim app = builder.Build()
		
		app.MapControllers()
		
		Return app
	End Function
	
	Private Function GetEdmModel() As IEdmModel
		Dim builder = new ODataConventionModelBuilder()
		builder.EntitySet(Of Customer)("Customers")
		builder.EntitySet(Of Movie)("Movies")
		Return builder.GetEdmModel()
	End Function
	
End Module
