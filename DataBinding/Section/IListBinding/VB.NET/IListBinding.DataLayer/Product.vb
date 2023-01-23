'<summary>
' Summary description for Product.
'</summary>
Public Class Product
	Friend Sub New()
	End Sub 'New

	'******
	'* The properties listed below define a product in the datalayer. 
	'******

	Public Property ProductID As Integer

	Public Property ProductName As String

	Public Property SupplierID As Integer

	Public Property CategoryID As Integer

	Public Property QuantityPerUnit As String

	Public Property UnitPrice As Decimal

	Public Property UnitsInStock As Integer

	Public Property UnitsOnOrder As Integer

	Public Property ReorderLevel As Integer

	Public Property Discontinued As Boolean
End Class 'Product
