Imports System.ComponentModel.DataAnnotations

Namespace Models

	Public Class Movie
		<Key>
		Public Property ID() As Integer
		Public Property Title() As String
		Public Property MPAA() As String
		Public Property YearReleased() As Integer
	End Class
	
End NameSpace