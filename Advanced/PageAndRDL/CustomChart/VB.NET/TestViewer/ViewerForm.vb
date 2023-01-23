Imports System.Text

Public Class ViewerForm

	Public Sub New()
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
		' This call is required by the designer.
		InitializeComponent()
	End Sub
	
	Protected Overrides Sub OnLoad(e As EventArgs)
		MyBase.OnLoad(e)
		viewer1.LoadDocument("..\..\..\..\..\Report\Radar.rdlx")
	End Sub
End Class
