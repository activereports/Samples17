﻿Imports System.IO
Imports System.Text

Public Class MainForm
	
	Sub New()
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
		InitializeComponent()
	End Sub

	' Loads and shows the report.
	Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
		Dim rptPath As New FileInfo("..\..\..\testReport.rdlx")
		Dim definition As New PageReport(rptPath)
		AddHandler definition.Document.LocateDataSource, AddressOf OnLocateDataSource

		ReportPreview1.ReportViewer.LoadDocument(definition.Document)
	End Sub

	' The handler of <see cref="PageDocument.LocateDataSource"/> that returns appropriate data for a report.
	Private Sub OnLocateDataSource(ByVal sender As Object, ByVal args As LocateDataSourceEventArgs)
		Dim data As New Object
		Dim dataSourceName As String = args.DataSet.Name
		If (dataSourceName = "DataSet1") Then
			data = DataLayer.CreateData()
		End If
		args.Data = data
	End Sub

End Class
