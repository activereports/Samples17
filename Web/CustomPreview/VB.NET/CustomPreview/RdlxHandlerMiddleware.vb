Imports System.IO
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Core.Export.Html.Page
Imports GrapeCity.ActiveReports.Export.Html.Page
Imports GrapeCity.ActiveReports.Extensibility.Rendering
Imports GrapeCity.ActiveReports.Extensibility.Rendering.IO
Imports GrapeCity.ActiveReports.Rendering.IO
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Net.Http.Headers

Public Class RdlxHandlerMiddleware
    
    Private Const HandlerExtension = ".rdlx"
    Private Const HandlerCacheExtension = ".rdlxWeb"
    
    Public Sub New (rd As RequestDelegate)
    End Sub
    
    Public Async Function InvokeAsync(context as HttpContext, hostingEnv as IWebHostEnvironment) As Task
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
        
        context.Response.ContentType = "text/html"
        Dim reportException As String = Nothing
        Dim streams = New HtmlStreamProvider()
        Try
            Using report = New PageReport(New FileInfo(hostingEnv.ContentRootPath + context.Request.Path))
                Dim html = New HtmlRenderingExtension()
                Dim settings = DirectCast(html.GetSupportedSettings(), Settings)
                settings.StyleStream = False
                settings.OutputTOC = False
                settings.Mode = RenderMode.Paginated
                report.Document.Render(html, streams, DirectCast(settings, ISettings).GetSettings())
            End Using
            
            For Each secondaryStreamInfo As StreamInfo In streams.GetSecondaryStreams()
                Dim secondaryStream = DirectCast(secondaryStreamInfo.OpenStream(), MemoryStream)
                ' 30 seconds should be enough to load all images
                cache.Set(secondaryStreamInfo.Uri.OriginalString, secondaryStream.ToArray(),
                          New MemoryCacheEntryOptions With { .SlidingExpiration = New TimeSpan(0, 0, 30) })
            Next
            
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
        
        Dim primaryStream = DirectCast(streams.GetPrimaryStream().OpenStream(), MemoryStream)
	   
	    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue With { .NoCache = true }
        Await context.Response.Body.WriteAsync(primaryStream.ToArray())
    End Function
    
    Private Function GetCache(context as HttpContext) as IMemoryCache
        Return context.RequestServices.GetService(Of IMemoryCache)
    End Function
    
    Private NotInheritable Class HtmlStreamProvider
        Inherits MemoryStreamProvider
        Protected Overrides Function GetStreamUri(extension As String) As Uri
            Dim uri As Uri = MyBase.GetStreamUri(extension)
            Return New Uri(uri.OriginalString + HandlerCacheExtension, UriKind.Relative)
        End Function
    End Class
End Class