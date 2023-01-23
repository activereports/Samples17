namespace CustomPreview;

public static class MapReportingExtensions
{
	private static readonly string[] _rdlx = { ".rdlx", ".rdlxWeb" };
	private static readonly string[] _rpx = { ".rpx", "rpxWeb" };
	
	public static void MapReporting(this WebApplication app)
	{
		app.MapWhen(ctx => _rdlx.Any(x => ctx.Request.Path.ToString().EndsWith(x, StringComparison.InvariantCultureIgnoreCase)),
			x => x.UseMiddleware<RdlxHandlerMiddleware>());
		
		app.MapWhen(ctx => _rpx.Any(x => ctx.Request.Path.ToString().EndsWith(x, StringComparison.InvariantCultureIgnoreCase)),
			x => x.UseMiddleware<RpxHandlerMiddleware>());

		app.MapWhen(ctx => ctx.Request.Path.ToString().EndsWith(".rdlx/pdf", StringComparison.InvariantCultureIgnoreCase),
			x => x.UseMiddleware<RdlxPdfPreviewMiddleware>());
		
		app.MapWhen(ctx => ctx.Request.Path.ToString().EndsWith(".rpx/pdf", StringComparison.InvariantCultureIgnoreCase),
			x => x.UseMiddleware<RpxPdfPreviewMiddleware>());
		
	}
}