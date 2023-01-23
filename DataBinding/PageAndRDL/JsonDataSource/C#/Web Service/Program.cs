using WebService;
using WebService.Properties;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddAuthentication(nameof(BasicAuthentication))
	.AddScheme<BasicAuthenticationSchemeOptions, BasicAuthentication>(nameof(BasicAuthentication), null);

builder.Services.AddControllers()
	.AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });

var app = builder.Build();

app.Map("/", (ctx) => ctx.Response.WriteAsync(Resources.bodyOfMessage));

app.UseAuthorization();

app.MapControllers();

app.Run();