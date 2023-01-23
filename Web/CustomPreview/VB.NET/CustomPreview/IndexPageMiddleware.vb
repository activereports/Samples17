Imports System.IO
Imports System.Text
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class IndexPageMiddleware
	
	Private ReadOnly Exts As String() = { ".rdlx", ".rpx" }
	
	Public Sub New (rd As RequestDelegate)
	End Sub
	
	Public Async Function InvokeAsync(context as HttpContext, hostingEnv as IWebHostEnvironment) As Task
		Dim sb = new StringBuilder()
		sb.Append("<!DOCTYPE html><html lang=""en""><head><meta charset=""UTF-8""><title>ActiveReports Custom Preview</title><link rel=""stylesheet""type=""text/css"" href=""style.css""><link rel=""shortcut icon"" href=""favicon.ico""/></head><body><div id=""pagetop"">
		<div id=""pagetitlebanner""><h1><a href=""Index"">ActiveReports Custom Preview</a></h1></div></div><div id=""pagebody"">
		<h2>ActiveReports Custom Preview Options</h2><!-- Raw Exporting --><p><br>The PDF and HTML exports allow developers to manually control exporting by
		writing a little code in your favorite ASP.NET language.</p><table><tr><th>Filename</th><th>HTML</th><th>PDF</th></tr>")
		
		Dim filePaths = Directory.GetFiles(Path.Combine(hostingEnv.ContentRootPath, "Reports")).
			    Where(function(x) Exts.Any(Function(c) x.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)))
		
		For Each filePath in filePaths
			Dim file = Path.GetFileName(filePath)
			sb.Append($"<tr><td>{file}</td><td><a href=""{String.Concat("Reports/", file)}"">HTML</a></td>
			<td><a href=""{String.Concat("Reports/", file, "/pdf")}"">PDF</a></td></tr>")
		Next

		sb.Append("</table><p style=""FONT-SIZE: smaller; VERTICAL-ALIGN: sub; FONT-STYLE: italic"">
		See ActiveReports Professional Edition for additional web reporting options under ASP.NET.</p></div></body>")
		
		Await context.Response.WriteAsync(sb.ToString())
	End Function
End Class