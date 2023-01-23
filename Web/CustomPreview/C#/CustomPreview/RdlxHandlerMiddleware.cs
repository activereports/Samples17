using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Core.Export.Html.Page;
using GrapeCity.ActiveReports.Export.Html.Page;
using GrapeCity.ActiveReports.Extensibility.Rendering;
using GrapeCity.ActiveReports.Rendering.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace CustomPreview;

public class RdlxHandlerMiddleware
{
	private const string HandlerExtension = ".rdlx";
	private const string HandlerCacheExtension = ".rdlxWeb";
	
	public RdlxHandlerMiddleware(RequestDelegate next) { }
	
	public async Task InvokeAsync(HttpContext context, IWebHostEnvironment webHostEnvironment)
	{
		var cache = GetCache(context)!;
		if (!context.Request.Path.ToString().EndsWith(HandlerExtension))
		{
			if (!context.Request.Path.ToString().EndsWith(HandlerCacheExtension))
				return;

			// return image
			var keyName = Path.GetFileName(context.Request.Path.ToUriComponent());
			if (cache.TryGetValue(keyName, out var cacheItem))
			{
				var cacheItemByteArray = (byte[]) cacheItem;
				// For SVG images the automatic detection of Response.ContentType sets the wrong type,
				// so we have to set the content type manually 
				if (keyName.EndsWith(".svg" + HandlerCacheExtension))
					context.Response.ContentType = "image/svg+xml";
				await context.Response.Body.WriteAsync(cacheItemByteArray);
			}
			return;
		}

		context.Response.ContentType = "text/html";
		var streams = new HtmlStreamProvider();
		try
		{
			using (var report = new PageReport(new FileInfo(webHostEnvironment.ContentRootPath + context.Request.Path)))
			{
				var html = new HtmlRenderingExtension();
				var settings = (Settings)html.GetSupportedSettings();
				settings.StyleStream = false;
				settings.OutputTOC = false;
				settings.Mode = RenderMode.Paginated;
				report.Document.Render(html, streams, ((ISettings)settings).GetSettings());
			}

			foreach (var secondaryStreamInfo in streams.GetSecondaryStreams())
			{
				var secondaryStream = (MemoryStream) secondaryStreamInfo.OpenStream();
				// 30 seconds should be enough to load all images
				cache.Set(secondaryStreamInfo.Uri.OriginalString, secondaryStream.ToArray(),
					new MemoryCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 0, 30) });
			}
		}
		catch (ReportException eRunReport)
		{
			// Failure running report, just report the error to the user.
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync(Resources.Resources.Error + eRunReport);
			return;
		}

		var primaryStream = (MemoryStream) streams.GetPrimaryStream().OpenStream();

		context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue { NoCache = true };
		await context.Response.Body.WriteAsync(primaryStream.ToArray());
	}
	
	private IMemoryCache? GetCache(HttpContext context) => context.RequestServices.GetService<IMemoryCache>();
	
	private sealed class HtmlStreamProvider : MemoryStreamProvider
	{
		protected override Uri GetStreamUri(string extension)
		{
			Uri uri = base.GetStreamUri(extension);
			return new Uri(uri.OriginalString + HandlerCacheExtension, UriKind.Relative);
		}
	}
}
