Imports System.IO
Imports System.Xml
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Core.Rendering.Tools
Imports GrapeCity.ActiveReports.Export.Html
Imports GrapeCity.ActiveReports.Export.Html.Section
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Net.Http.Headers

Public Class RpxHandlerMiddleware
    
    Private Const HandlerExtension = ".rpx"
    Private Const HandlerCacheExtension = ".rpxWeb"
    
    Public Sub New (rd As RequestDelegate)
    End Sub
    
    Public Async Function InvokeAsync(context as HttpContext, webHostEnvironment as IWebHostEnvironment) As Task
        Dim cache = GetCache(context)
        If Not context.Request.Path.ToString().EndsWith(HandlerExtension)
            If Not context.Request.Path.ToString().EndsWith(HandlerCacheExtension)
                Return
            End If
            
            ' return image
            Dim keyName = Path.GetFileName(context.Request.Path.ToUriComponent())
            Dim cacheItem as Byte()
            If cache.TryGetValue(keyName, cacheItem)
	            ' For SVG images the automatic detection of Response.ContentType sets the wrong type,
	            ' so we have to set the content type manually 
	            If keyName.EndsWith(".svg" + HandlerCacheExtension)
		            context.Response.ContentType = "image/svg+xml"
	            End If
                Await context.Response.Body.WriteAsync(cacheItem)
            End If
            Return
        End If
        
        Dim rpxFile As String = webHostEnvironment.ContentRootPath.Substring(0, webHostEnvironment.ContentRootPath.Length - 1) + context.Request.Path.Value
        Dim htmlHandler = New HtmlOutputHandler(cache, Path.GetFileNameWithoutExtension(rpxFile))
        Dim reportException As String = Nothing
        
        context.Response.ContentType = "text/html"
        Try
            Using report = New SectionReport()
                Using reader As XmlReader = XmlReader.Create(rpxFile)
                    report.ResourceLocator = New DefaultResourceLocator(New Uri(Path.GetDirectoryName(rpxFile) + "\"))
                    report.LoadLayout(reader)
                    report.Run(False)
                    Using html = New HtmlExport With { .IncludeHtmlHeader = True }
                        html.Export(report.Document, htmlHandler, "")
                    End Using
                    report.Document.Dispose()
                End Using
            End Using
        Catch eRunReport As ReportException
            ' Failure running report, just report the error to the user.
            reportException = eRunReport.ToString()
        End Try
        ' VB doesn't support Await inside Try Catch block
        If Not String.IsNullOrEmpty(reportException)
	        context.Response.StatusCode = 500
            Await context.Response.WriteAsync(My.Resources.Resources.RunningError + reportException)
            Return
        End If
    
	    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue With { .NoCache = true }
        Await context.Response.Body.WriteAsync(htmlHandler.MainPageData)
    End Function
    
    Private Function GetCache(context as HttpContext) as IMemoryCache
        Return context.RequestServices.GetService(Of IMemoryCache)
    End Function
    
    Private NotInheritable Class HtmlOutputHandler
        Implements IOutputHtml
        Private ReadOnly _cache As IMemoryCache
        Private ReadOnly _name As String
        Private _mainPageData As Byte()

        Public Sub New(cache As IMemoryCache, name As String)
            _cache = cache
            _name = name
        End Sub

        Public Function OutputHtmlData(info As HtmlOutputInfoArgs) As String Implements IOutputHtml.OutputHtmlData
            Dim stream As Stream = info.OutputStream

            Dim dataLen = CInt(stream.Length)
            If dataLen <= 0 Then
                Return String.Empty
            End If

            Dim bytesData = New Byte(dataLen - 1) {}
            stream.Seek(0, SeekOrigin.Begin)
            stream.Read(bytesData, 0, dataLen)

            If info.OutputKind = HtmlOutputKind.HtmlPage Then
                MainPageData = bytesData
                Return _name
            End If

            Dim keyName As String = Guid.NewGuid().ToString() & HandlerCacheExtension
            ' 30 seconds should be enough to load all images
            _cache.Set(keyName, bytesData, New MemoryCacheEntryOptions With { .SlidingExpiration = New TimeSpan(0, 0, 30) })
            Return keyName
        End Function

        Public Sub Finish() Implements IOutputHtml.Finish
        End Sub

        Public Property MainPageData As Byte()
            Get
                Return _mainPageData
            End Get
            Private Set(value As Byte())
                _mainPageData = value
            End Set
        End Property
    End Class
End Class