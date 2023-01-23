Imports System.Collections.ObjectModel
Imports Microsoft.Data.Sqlite
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.OData.Query
Imports Microsoft.AspNetCore.OData.Routing.Controllers
Imports Microsoft.Extensions.Configuration
Imports ODataEndPoint.Models

Namespace Controllers

	Public Class CustomersController
		Inherits ODataController

		ReadOnly _configuration As IConfiguration
		ReadOnly _env As IWebHostEnvironment

		Public Sub New(configuration As IConfiguration, env As IWebHostEnvironment)
			_configuration = configuration
			_env = env
		End Sub

		<EnableQuery>
		Public Function [Get]() As IActionResult
			Dim connStr = _configuration.GetSection("ConnectionStrings")("Nwind").Replace("$appPath$", _env.ContentRootPath)
			Dim conn As New SqliteConnection(connStr)
			conn.Open()
			Dim customers As New Collection(Of Customer)()
			Dim cmd As New SqliteCommand("select customers.CustomerID, customers.CompanyName, customers.ContactName, customers.Address from customers", conn)
			Dim dataReader As SqliteDataReader = cmd.ExecuteReader()
			While dataReader.Read()
				customers.Add(New Customer() With {
								 .CustomerID = dataReader.GetValue(0).ToString(),
								 .CompanyName = dataReader.GetValue(1).ToString(),
								 .ContactName = dataReader.GetValue(2).ToString(),
								 .Address = dataReader.GetValue(3).ToString()
								 })
			End While

			conn.Close()
			Return Ok(customers.AsQueryable())
		End Function
	End Class
End NameSpace