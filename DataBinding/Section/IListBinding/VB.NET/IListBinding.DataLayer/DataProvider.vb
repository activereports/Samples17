Imports System.Data.SQLite

Friend Class DataProvider

	'<summary>
	' Returns a new connection object for reading the data in the ProductCollection
	'</summary>
	Friend Shared ReadOnly Property NewConnection() As SQLiteConnection
		Get
			Dim connectString As String = My.Resources.ConnectionString
			Return New SQLiteConnection(connectString)
		End Get
	End Property

End Class 'DataProvider
