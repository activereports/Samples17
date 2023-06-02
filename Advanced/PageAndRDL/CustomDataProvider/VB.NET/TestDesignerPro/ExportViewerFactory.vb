Imports GrapeCity.ActiveReports.Design.Advanced
Imports GrapeCity.ActiveReports.Viewer.Win.Internal.Export

Friend Class ExportViewerFactory
	Implements IExportViewerFactory
	Public Function CreateExportViewer(viewer As Viewer.Win.Viewer) As IExportViewer Implements IExportViewerFactory.CreateExportViewer
		Return New ExportViewer(viewer)
	End Function
End Class
