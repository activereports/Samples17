using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Export.Pdf.Section;

namespace CustomPreview;

public class RpxPdfPreviewMiddleware
{
	public RpxPdfPreviewMiddleware(RequestDelegate next) { }

	public async Task InvokeAsync(HttpContext context, IWebHostEnvironment webHostEnvironment)
	{
		var filename = context.Request.Path.Value?.Replace("/pdf", "");
		if (string.IsNullOrEmpty(filename))
		{
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync("Invalid filename");
			return;
		}
		
		var rpt = new SectionReport();
		var filePath = webHostEnvironment.ContentRootPath[..^1] + filename;
		var reportsPath = Path.GetDirectoryName(filePath);
		rpt.ResourceLocator = new DefaultResourceLocator(new Uri(reportsPath + @"\"));
		var xtr = new System.Xml.XmlTextReader(filePath);
		rpt.LoadLayout(xtr);
		xtr.Close();
		try
		{
			rpt.Run(false);
		} 
		catch (ReportException eRunReport)
		{
			// Failure running report, just report the error to the user.
			context.Response.Clear();
			
			context.Response.ContentType = "text/html";
			context.Response.StatusCode = 500;
			await context.Response.WriteAsync(Resources.Resources.Error + eRunReport);
			return;
		}

		// Tell the browser this is a PDF document so it will use an appropriate viewer.
		// If the report has been exported in a different format, the content-type will 
		// need to be changed as noted in the following table:
		//	ExportType  ContentType
		//	PDF	   "application/pdf"  (needs to be in lowercase)
		//	RTF	   "application/rtf"
		//	TIFF	  "image/tiff"	   (will open in separate viewer instead of browser)
		//	HTML	  "message/rfc822"   (only applies to compressed HTML pages that includes images)
		//	Excel	 "application/vnd.ms-excel"
		//	Excel	 "application/excel" (either of these types should work) 
		//	Text	  "text/plain"  
		context.Response.ContentType = "application/pdf";
		context.Response.Headers.Add("content-disposition", "inline; filename=MyPDF.PDF");

		// Create the PDF export object.
		var pdf = new PdfExport();
		// Create a new memory stream that will hold the pdf output.
		var memStream = new MemoryStream();
		// Export the report to PDF.
		pdf.Export(rpt.Document, memStream);
		await context.Response.Body.WriteAsync(memStream.ToArray());
	}
}