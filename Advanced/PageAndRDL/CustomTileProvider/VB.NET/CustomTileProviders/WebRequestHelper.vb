Imports System.IO
Imports System.Net.Http

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
        Using client As New HttpClient()

            If Not String.IsNullOrEmpty(userAgent) Then client.DefaultRequestHeaders.Add("User-Agent", userAgent)

            If timeoutMilliseconds > 0 Then
                client.Timeout = New TimeSpan(0, 0, 0, 0, timeoutMilliseconds)
            End If

            Dim task = client.GetAsync(url).ContinueWith(Sub(responseTask)
                                                             Try
                                                                 Dim response As HttpResponseMessage = responseTask.Result

                                                                 If Not response.IsSuccessStatusCode Then
                                                                     Dim errorTask = response.Content.ReadAsStringAsync()
                                                                     errorTask.Wait()

                                                                     Throw New Exception(errorTask.Result)
                                                                 End If

                                                                 Dim readTask = response.Content.ReadAsStreamAsync()
                                                                 readTask.Wait()

                                                                 'Copy data from buffer (It must be done, otherwise the buffer overflow and we stop to receive responses).
                                                                 Dim stream = New MemoryStream()
                                                                 Using responseStream As Stream = readTask.Result
                                                                     If responseStream IsNot Nothing Then
                                                                         responseStream.CopyTo(stream)
                                                                         success(stream, response.Content.Headers.ContentType.MediaType)
                                                                     Else
                                                                         [error](New NullReferenceException(NameOf(responseStream)))
                                                                     End If
                                                                 End Using
                                                             Catch exception As Exception
                                                                 [error](exception)
                                                             End Try
                                                         End Sub)
            task.Wait()
        End Using
    End Sub
End Module

