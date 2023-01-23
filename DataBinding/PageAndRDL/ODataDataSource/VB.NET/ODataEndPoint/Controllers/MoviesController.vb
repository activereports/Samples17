Imports Microsoft.Data.Sqlite
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.OData.Routing.Controllers
Imports Microsoft.Extensions.Configuration
Imports ODataEndPoint.Models

Namespace Controllers
	Public Class MoviesController
		Inherits ODataController
		
		Dim ReadOnly _configuration As IConfiguration
		Dim ReadOnly _env As IWebHostEnvironment

		Public Sub New(configuration As IConfiguration, env As IWebHostEnvironment)
			_configuration = configuration
			_env = env
		End Sub
		
		Public Function [Get]() As IList(Of Movie)
			Dim movies = New List(Of Movie)()
			
			Dim connStr = _configuration.GetSection("ConnectionStrings")("Reels").Replace("$appPath$", _env.ContentRootPath)
			Dim conn As New SqliteConnection(connStr)
			conn.Open()
			Dim cmd As New SqliteCommand("SELECT Movie.MovieID, Movie.Title, Movie.MPAA, Movie.YearReleased FROM Movie ORDER BY Movie.YearReleased", conn)
			Dim dataReader As SqliteDatareader = cmd.ExecuteReader()
			While dataReader.Read()
				movies.Add(New Movie() With {
					          .Id = CType(dataReader.GetValue(0), Integer),
					          .Title = dataReader.GetValue(1).ToString(),
					          .MPAA = dataReader.GetValue(2).ToString(),
					          .YearReleased = CType(dataReader.GetValue(3), Integer)
					          })
			End While

			conn.Close()
			Return movies
		End Function
	End Class
End NameSpace