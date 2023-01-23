Imports System.IO
Imports System.Xml
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Export.Pdf.Section
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class RpxPdfPreviewMiddleware
	Public Sub New (rd As RequestDelegate)
	End Sub
	
	Public Async Function InvokeAsync(context as HttpContext, webHostEnvironment as IWebHostEnvironment) As Task
		Dim filename = context.Request.Path.Value?.Replace("/pdf", "")
		If String.IsNullOrEmpty(filename)
			context.Response.StatusCode = 500
			Await context.Response.WriteAsync("Invalid filename")
			Return
		End If
		
		Dim rpt As New SectionReport()
		Dim filePath = webHostEnvironment.ContentRootPath.Substring(0, webHostEnvironment.ContentRootPath.Length - 1) + filename
        Dim reportsPath As String = Path.GetDirectoryName(filePath)
        rpt.ResourceLocator = New DefaultResourceLocator(New Uri(reportsPath + "/"))
        Dim xtr As New XmlTextReader(filePath)
        rpt.LoadLayout(xtr)
        xtr.Close()
        
        Dim reportException As String = Nothing
        Try
            rpt.Run(False)
        Catch eRunReport As ReportException
            ' Failure running report, just report the error to the user.
            reportException = eRunReport.ToString()
        End Try
        ' VB doesn't support Await inside Try Catch block
        If Not String.IsNullOrEmpty(reportException)
            context.Response.Clear()
	        context.Response.StatusCode = 500
            Await context.Response.WriteAsync(My.Resources.Resources.RunningError + reportException)
            Return
        End If
        
        ' Tell the browser this is a PDF document so it will use an appropriate viewer.
        '  If the report has been exported in a different format, the content-type will 
        ' need to be changed as noted in the following table:
        'ExportType		(ContentType)
        '	PDF		   "application/pdf"  (needs to be in lowercase)
        '	RTF		  ("application/rtf")
        '	TIFF		  "image/tiff"	   (will open in separate viewer instead of browser)
        '	HTML		  "message/rfc822"   (only applies to compressed HTML pages that includes images)
        '	Excel		("application/vnd.ms-excel")
        '	Excel		 "application/excel" (either of these types should work) 
        '	Text		  ("text/plain")
        context.Response.ContentType = "application/pdf"

        context.Response.Headers.Add("content-disposition", "inline; filename=MyPDF.PDF")

        ' Create the PDF export object.
        Dim pdf As New PdfExport()
        ' Create a new memory stream that will hold the pdf output.
        Dim memStream As New System.IO.MemoryStream()
        ' Export the report to PDF.
        pdf.Export(rpt.Document, memStream)
        '  Write the PDF stream out.
		await context.Response.Body.WriteAsync(memStream.ToArray())
	End Function
End Class