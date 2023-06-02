using System.Xml;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Export.Html;
using GrapeCity.ActiveReports.Export.Html.Section;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace CustomPreview;

public class RpxHandlerMiddleware
{
	private const string HandlerExtension = ".rpx";
	private const string HandlerCacheExtension = ".rpxWeb";
	
	public RpxHandlerMiddleware(RequestDelegate next) { }
	
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
		var rootpath = webHostEnvironment.ContentRootPath.EndsWith("\\") ? 
			webHostEnvironment.ContentRootPath[..^1] : webHostEnvironment.ContentRootPath;

        var rpxFile = rootpath + context.Request.Path.Value;
		var htmlHandler = new HtmlOutputHandler(cache, Path.GetFileNameWithoutExtension(rpxFile));
		context.Response.ContentType = "text/html";
		
		try
		{
			using var report = new SectionReport();
			using var reader = XmlReader.Create(rpxFile);
			report.ResourceLocator = new DefaultResourceLocator(new Uri(Path.GetDirectoryName(rpxFile) + @"\"));
			report.LoadLayout(reader);
			report.Run(false);
			using (var html = new HtmlExport { IncludeHtmlHeader = true })
				html.Export(report.Document, htmlHandler, "");
			report.Document.Dispose();
		}
		catch (ReportException eRunReport)
		{
			// Failure running report, just report the error to the user.
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync(Resources.Resources.Error + eRunReport);
			return;
		}

		context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue { NoCache = true };
		await context.Response.Body.WriteAsync(htmlHandler.MainPageData);
	}
	
	private IMemoryCache? GetCache(HttpContext context) => context.RequestServices.GetService<IMemoryCache>();
	
	private sealed class HtmlOutputHandler : IOutputHtml
	{
		private readonly IMemoryCache _cache;
		private readonly string _name;

		public HtmlOutputHandler(IMemoryCache cache, string name)
		{
			_cache = cache;
			_name = name;
		}

		public string OutputHtmlData(HtmlOutputInfoArgs info)
		{
			var stream = info.OutputStream;

			var dataLen = (int)stream.Length;
			if (dataLen <= 0)
				return string.Empty;

			var bytesData = new byte[dataLen];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(bytesData, 0, dataLen);

			if (info.OutputKind == HtmlOutputKind.HtmlPage)
			{
				MainPageData = bytesData;
				return _name;
			}

			var keyName = Guid.NewGuid().ToString() + HandlerCacheExtension;
			// 30 seconds should be enough to load all images
			_cache.Set(keyName, bytesData, new MemoryCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 0, 30) });
			return keyName;
		}

		public void Finish()
		{
		}

		public byte[] MainPageData { get; private set; } = null!;
	}
}
