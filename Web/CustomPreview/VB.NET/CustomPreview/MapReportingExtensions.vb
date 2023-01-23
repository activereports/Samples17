Imports System.Runtime.CompilerServices
Imports Microsoft.AspNetCore.Builder

Public Module MapReportingExtensions
    
    Private ReadOnly Rdlx as String() = { ".rdlx", "rdlxWeb" }
    Private ReadOnly Rpx as String() = { ".rpx", ".rpxWeb" } 
    
    <Extension()>
    Public Sub MapReporting(app as WebApplication)
	  
        app.MapWhen(Function(ctx) Rdlx.Any(Function(x) ctx.Request.Path.ToString().EndsWith(x, StringComparison.InvariantCultureIgnoreCase)),
                    Sub(x) x.UseMiddleware(Of RdlxHandlerMiddleware))
        app.MapWhen(Function(ctx) Rpx.Any(Function(x) ctx.Request.Path.ToString().EndsWith(x, StringComparison.InvariantCultureIgnoreCase)),
                    Sub(x) x.UseMiddleware(Of RpxHandlerMiddleware))
	    
	    app.MapWhen(Function(ctx) ctx.Request.Path.ToString().EndsWith(".rdlx/pdf", StringComparison.InvariantCultureIgnoreCase),
	                Sub(x) x.UseMiddleware(Of RdlxPdfPreviewMiddleware))
	    
	    app.MapWhen(Function(ctx) ctx.Request.Path.ToString().EndsWith(".rpx/pdf", StringComparison.InvariantCultureIgnoreCase),
	                Sub(x) x.UseMiddleware(Of RpxPdfPreviewMiddleware))
	    
    End Sub
End Module