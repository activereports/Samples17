using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataEndPoint.Models;

static IEdmModel GetEdmModel()
{
	var builder = new ODataConventionModelBuilder();
	builder.EntitySet<Customer>("Customers");
	builder.EntitySet<Movie>("Movies");
	return builder.GetEdmModel();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddControllers()
	.AddOData(o => o.AddRouteComponents(GetEdmModel(), new DefaultODataBatchHandler()).Filter().Select().Expand());

var app = builder.Build();

app.MapControllers();

app.Run();