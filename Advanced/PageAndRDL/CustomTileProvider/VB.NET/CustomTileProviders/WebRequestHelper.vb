Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices

Friend Module WebRequestHelper
    ''' <summary>
    ''' Load raw data into MemoryStream from specified Url.
    ''' </summary>
    ''' <paramname="url">Data source Url</param>
    ''' <paramname="timeoutMilliseconds">Timeout in milliseconds. If -1 the default timeout will  used.</param>
    ''' <paramname="success">Success callback.</param>
    ''' <paramname="error">Error callback.</param>
    ''' <paramname="userAgent">User-Agent for request.</param>
    ''' <returns></returns>
    Public Sub DownloadDataAsync(ByVal url As String, ByVal timeoutMilliseconds As Integer, ByVal success As Action(Of MemoryStream, String), ByVal [error] As Action(Of Exception), ByVal Optional userAgent As String = Nothing)
    Dim request = WebRequest.CreateHttp(url)

    If Not String.IsNullOrEmpty(userAgent) Then request.UserAgent = userAgent

    If timeoutMilliseconds > 0 Then
        request.Timeout = timeoutMilliseconds
    End If

    request.BeginGetResponse(Sub(ar)
                                 Try
                                     Dim response = request.EndGetResponse(ar)

                                     'Copy data from buffer (It must be done, otherwise the buffer overflow and we stop to receive responses).
                                     Dim stream = New MemoryStream()
                                     Dim responseStream = response.GetResponseStream()
                                     If responseStream IsNot Nothing Then
                                         responseStream.CopyTo(stream)
                                         success(stream, response.ContentType)
                                     Else
                                         [error](New NullReferenceException(NameOf(responseStream)))
                                     End If
                                 Catch exception As Exception
                                     [error](exception)
                                 End Try
                             End Sub, Nothing)

End Sub
End Module

