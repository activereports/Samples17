Imports System.Collections.Specialized
Imports System.Globalization
Imports GrapeCity.ActiveReports.Extensibility.Rendering.Components.Map
Imports GrapeCity.ActiveReports.Extensibility.Rendering
Imports System.Net
Imports System.IO
Imports System.Xml

''' <summary>
''' Represents service which provides map tile images from Map Quest (http://www.mapquest.com/).
''' </summary>
Public NotInheritable Class MapQuestTileProvider
    Inherits BaseTileProvider
    Implements IMapTileProvider
    ''' <summary>
    ''' Provider settings:
    ''' ApiKey - The key to access API
    ''' Timeout - Response timout
    ''' Language - API language
    ''' UseSecureConnection.IsVisible - False
    ''' </summary>
    Private _Settings As NameValueCollection
    Private Const UrlTemplate As String = "https://www.mapquestapi.com/staticmap/v5/map?key={0}&center={1},{2}&zoom={3}&size=256,256&type={4}&format=png&scalebar=false"

    Public Property Settings As NameValueCollection Implements IMapTileProvider.Settings
        Get
            Return _Settings
        End Get
        Private Set(ByVal value As NameValueCollection)
            _Settings = value
        End Set
    End Property

    Private ReadOnly Property IMapTileProvider_Copyright As String Implements IMapTileProvider.Copyright
        Get
            Return Copyright
        End Get
    End Property

    Public Sub New()
        Settings = New NameValueCollection()
        Settings.[Set]("UseSecureConnection.IsVisible", "False")
        Settings.[Set]("Styles", String.Join(";", [Enum].GetNames(GetType(MapTypes))))
    End Sub

    Public Sub GetTile(ByVal key As MapTileKey, ByVal success As Action(Of IMapTile), ByVal [error] As Action(Of Exception)) Implements IMapTileProvider.GetTile
        Dim parameters = GetParameters()

        ValidateApiKey(parameters, Sub()
                                       Dim url = MapQuestTileProvider.GetTileUrl(key, parameters)

                                       WebRequestHelper.DownloadDataAsync(url, parameters.Timeout, Sub(stream, contentType) success(New MapTile(key, New ImageInfo(stream, contentType))), [error])
                                   End Sub, [error])
    End Sub

    Private Shared Function GetTileUrl(ByVal key As MapTileKey, ByVal parameters As Parameters) As String
        Dim p = key.ToWorldPos()
        Dim url = String.Format(CultureInfo.InvariantCulture.NumberFormat, UrlTemplate, parameters.Key, p.Y, p.X, key.LevelOfDetail, parameters.MapType.ToString().ToLower())

        If Not String.IsNullOrEmpty(parameters.Language) Then url += "&language=" & parameters.Language
        Return url
    End Function

    Private Sub ValidateApiKey(ByVal parameters As Parameters, ByVal success As Action, ByVal [error] As Action(Of Exception))
        Dim request = WebRequest.Create("http://www.mapquestapi.com/geocoding/v1/reverse?location=0,0&outFormat=xml&key=" & parameters.Key)
        If parameters.Timeout > 0 Then
            request.Timeout = parameters.Timeout
        End If

        request.BeginGetResponse(Sub(ar)
                                     Try
                                         Dim response = request.GetResponse()

                                         'Copy data from buffer (It must be done, otherwise the buffer overflow and we stop to get repsonses).
                                         Using reader = New StreamReader(response.GetResponseStream())
                                             Dim result = reader.ReadToEnd()
                                             Dim doc = New XmlDocument()
                                             doc.LoadXml(result)
                                             Dim infoNode = doc.SelectSingleNode("response/info")
                                             If infoNode IsNot Nothing AndAlso infoNode("statusCode") IsNot Nothing Then
                                                 Dim statusCode = infoNode("statusCode").InnerText
                                                 If statusCode Is "403" Then
                                                     [error](New MapQuestServiceMapsKeyError())
                                                     Return
                                                 End If
                                             End If
                                         End Using

                                         success()
                                     Catch exception As Exception
                                         Dim webEx = TryCast(exception, WebException)
                                         If webEx IsNot Nothing Then
                                             If webEx.Status.Equals(WebExceptionStatus.ProtocolError) Then
                                                 Dim response = TryCast(webEx.Response, HttpWebResponse)
                                                 If response IsNot Nothing AndAlso response.StatusCode.Equals(HttpStatusCode.Forbidden) Then
                                                     [error](New MapQuestServiceMapsKeyError())
                                                     Return
                                                 End If
                                             End If
                                         End If

                                         [error](exception)
                                     End Try
                                 End Sub, Nothing)
    End Sub


#Region "Parameters"
    Private Function GetParameters() As Parameters
        Dim parameters = New Parameters With {
.Key = If(Me.Settings("ApiKey"), "Fmjtd%7Cluur21ua2l%2C2x%3Do5-90t5h6"),
.Language = Me.Settings("Language"),
.Timeout = If(Not String.IsNullOrEmpty(Me.Settings("Timeout")), Integer.Parse(Me.Settings("Timeout")), -1)
}

        Select Case Me.Settings("Style")
            Case "Road", "Map"
                parameters.MapType = MapTypes.Map
            Case "Aerial", "Sat"
                parameters.MapType = MapTypes.Sat
            Case "Hybrid", "Hyb"
                parameters.MapType = MapTypes.Hyb
        End Select

        Return parameters
    End Function

    Friend Class Parameters
        Public Key As String
        Public MapType As MapTypes
        Public Language As String
        Public Timeout As Integer
    End Class

    '[DoNotObfuscateType]
    Friend Enum MapTypes
        Map
        Sat
        Hyb
    End Enum
#End Region
End Class

Friend Class MapQuestServiceMapsKeyError
    Inherits Exception
    Public Sub New()
        MyBase.New(String.Format(GrapeCity.ActiveReports.Samples.CustomTileProviders.My.Resources.Resources.ResourceManager.GetString("MapQuestMapsKeyIsInvalid")))
    End Sub
End Class
