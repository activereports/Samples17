Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.Json

' Provides the data used in the sample.
Friend NotInheritable Class DataLayer
	Public Shared Function CreateData() As String
		Const sourceUrl As String = "http://localhost:10395/customers/GetJson"
		Dim responseText As String = Nothing

		Using httpClient = New HttpClient()
			httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Basic " & Convert.ToBase64String(Encoding.[Default].GetBytes("admin:1")))
			httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
			Dim request = New HttpRequestMessage(HttpMethod.Get, sourceUrl)
			Dim response = httpClient.SendAsync(request).Result
			Dim json = response.Content.ReadAsStringAsync().Result
			Return json
		End Using
	End Function
End Class
