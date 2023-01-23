Imports System.IO
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Export.Pdf.Page
Imports GrapeCity.ActiveReports.Rendering.IO
Imports GrapeCity.ActiveReports.Rendering.Tools
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class RdlxPdfPreviewMiddleware
	Public Sub New (rd As RequestDelegate)
	End Sub
	
	Public Async Function InvokeAsync(context as HttpContext, webHostEnvironment as IWebHostEnvironment) As Task
		Dim filename = context.Request.Path.Value?.Replace("/pdf", "")
		If String.IsNullOrEmpty(filename)
			context.Response.StatusCode = 500
			Await context.Response.WriteAsync("Invalid filename")
			Return
		End If
		
		Dim filePath =  webHostEnvironment.ContentRootPath.Substring(0, webHostEnvironment.ContentRootPath.Length - 1) + filename
		Dim reportException As String = Nothing
		Dim streamProvider = New MemoryStreamProvider()
		Try
			Using reader = new StreamReader(webHostEnvironment.ContentRootPath + filename)
				Using report = new PageReport(reader)
					report.ResourceLocator = New DefaultResourceLocator(New Uri(Path.GetDirectoryName(filePath) + "\"))
					Dim pdfRe = new PdfRenderingExtension()
					report.Document.Render(pdfRe, streamProvider)
				End Using
			End Using
		Catch eRunReport As ReportException
			reportException = eRunReport.ToString()
		End Try
		
		If Not String.IsNullOrEmpty(reportException)
			context.Response.StatusCode = 500
			Await context.Response.WriteAsync(My.Resources.Resources.RunningError + reportException)
			Return
		End If
		
		Await context.Response.Body.WriteAsync(streamProvider.GetPrimaryStream().OpenStream().ToArray())
		
	End Function
End Class