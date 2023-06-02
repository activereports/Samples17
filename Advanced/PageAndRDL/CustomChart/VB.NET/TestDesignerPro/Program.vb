Imports GrapeCity.ActiveReports.Design.Advanced

Module Program
	Sub Main()
		Dim _reportName = "..\..\..\..\..\Report\Radar.rdlx"
		Dim df = New DesignerForm()
		df.LoadReport(_reportName)
		Application.Run(df)
	End Sub
End Module
