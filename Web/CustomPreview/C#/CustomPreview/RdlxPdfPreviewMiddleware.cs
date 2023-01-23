using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Document;
using GrapeCity.ActiveReports.Export.Pdf.Page;
using GrapeCity.ActiveReports.Rendering.IO;
using GrapeCity.ActiveReports.Rendering.Tools;

namespace CustomPreview;

public class RdlxPdfPreviewMiddleware
{
	public RdlxPdfPreviewMiddleware(RequestDelegate next) { }

	public async Task InvokeAsync(HttpContext context, IWebHostEnvironment webHostEnvironment)
	{
		var filename = context.Request.Path.Value?.Replace("/pdf", "", StringComparison.InvariantCultureIgnoreCase);
		if (string.IsNullOrEmpty(filename))
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync("Invalid filename");
			return;
		}

		var filePath = webHostEnvironment.ContentRootPath[..^1] + filename;
		var streamProvider = new MemoryStreamProvider();
		try
		{
			using var reader = new StreamReader(filePath);
			using var report = new PageReport(reader);
			report.ResourceLocator = new DefaultResourceLocator(new Uri(Path.GetDirectoryName(filePath) + @"\"));
			var pdfRe = new PdfRenderingExtension();

			report.Document.Render(pdfRe, streamProvider);
		}
		catch (ReportException eRunReport)
		{
			// Failure running report, just report the error to the user.
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync(Resources.Resources.Error + eRunReport);
			return;
		}
		
		await context.Response.Body.WriteAsync(streamProvider.GetPrimaryStream().OpenStream().ToArray());
	}
}