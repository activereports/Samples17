Imports System
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Data
Imports System.Xml
Imports System.IO
Imports System.Text
Imports System.Data.SQLite

'BoundDataSample - Illustrates how to bind data in ActiveReports.
Public Class MainForm
	Inherits System.Windows.Forms.Form
	ReadOnly _settingForNoHeaderFixed As String = My.Resources.NoHeaderFixed
	ReadOnly _settingForHeaderExistsFixed As String = My.Resources.HeaderExistsFixed
	Public Sub New()
		MyBase.New()
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
		'Required for Windows Form Designer support
		'
		InitializeComponent()

		'Add any initialization after the InitializeComponent() call
		'
	End Sub

	Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If Not (components Is Nothing) Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub

	Private Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		'Clear drop down lists
		'
		cbCompanyName.Items.Clear()
	End Sub 'MainForm_Load

	'Clear the viewer out when switching tabs.
	Private Sub tabDataBinding_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabDataBinding.SelectedIndexChanged
		'Clear existing pages
		arvMain.Document = New Document.SectionDocument()
	End Sub 'tabDataBinding_SelectedIndexChanged


	'Populate the DropDown list with company names from the Northwind access database.
	Private Sub cbCompanyName_DropDown(ByVal sender As Object, ByVal e As EventArgs) Handles cbCompanyName.DropDown
		'Populate the combo box if no items exist.
		If cbCompanyName.Items.Count = 0 Then

			Cursor = Cursors.WaitCursor

			'Set up database connection.
			Dim nwindConn As New SQLiteConnection()
			nwindConn.ConnectionString = My.Resources.ConnectionString
			'SQL Select statement used to get the Company Names
			Dim selectCMD As New SQLiteCommand("SELECT DISTINCT Customers_CompanyName from Invoices", nwindConn)

			nwindConn.Open()
			Dim companyNamesDR As SQLiteDataReader = selectCMD.ExecuteReader()

			'While the reader has data add a new Company Name to the list.
			While companyNamesDR.Read()
				cbCompanyName.Items.Add(companyNamesDR(0).ToString())
			End While
			nwindConn.Close()

			'Set selection to first item in the list.
			cbCompanyName.SelectedIndex = 0

			Cursor = Cursors.Arrow
		End If
	End Sub 'cbCompanyName_DropDown

	' Illustrates using a DataSet as a data source.
	' 
	Private Sub btnDataSet_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDataSet.Click
		Cursor = Cursors.WaitCursor

		'Dataset to hold data.
		'
		Dim invoiceData As New DataSet()
		invoiceData.Locale = CultureInfo.InvariantCulture

		'Database connection populated from the sample Northwind access database
		'
		Dim nwindConn As New SQLiteConnection()
		nwindConn.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		Dim selectCMD As New SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn)
		selectCMD.CommandTimeout = 30

		'Data adapter used to run the select command
		'
		Dim invoicesDA As New SQLiteDataAdapter()
		invoicesDA.SelectCommand = selectCMD

		'Fill the DataSet.
		'
		invoicesDA.Fill(invoiceData, "Invoices")
		nwindConn.Close()

		'Create the report and assign the data source.
		'
		Dim rpt As New SectionReport()
		rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
		rpt.DataSource = invoiceData
		rpt.DataMember = invoiceData.Tables(0).TableName

		'Run and view the report.
		'
		arvMain.LoadDocument(rpt)

		Cursor = Cursors.Arrow
	End Sub 'btnDataSet_Click

	' Illustrates using a DataTable as a data source
	' 

	Private Sub btnDataTable_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDataTable.Click
		Cursor = Cursors.WaitCursor

		'DataTable to hold the data.
		'
		Dim invoiceData As New DataTable("Invoices")
		invoiceData.Locale = CultureInfo.InvariantCulture

		'Database connection populated from the sample Northwind access database
		'
		Dim nwindConn As New SQLiteConnection()
		nwindConn.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		Dim selectCMD As New SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn)
		selectCMD.CommandTimeout = 30

		'Data adapter used to run the select command
		'
		Dim invoicesDA As New SQLiteDataAdapter()
		invoicesDA.SelectCommand = selectCMD

		'Fill the DataTable
		'
		invoicesDA.Fill(invoiceData)
		nwindConn.Close()

		'Create the report and assign the data source
		'
		Dim rpt As New SectionReport()
		rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
		rpt.DataSource = invoiceData
		'Run and view the report
		'
		arvMain.LoadDocument(rpt)

		Cursor = Cursors.Arrow
	End Sub 'btnDataTable_Click

	'Illustrates using a DataView as a data source
	'
	Private Sub btnDataView_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDataView.Click
		Cursor = Cursors.WaitCursor

		'Verify that a value Company Name is selected
		'
		If cbCompanyName.SelectedItem Is Nothing Then
			MessageBox.Show(My.Resources.SelectCompanyName)

			Cursor = Cursors.Arrow
			Return
		End If

		'DataTable to hold the full data
		'
		Dim invoiceData As New DataTable("Invoices")
		invoiceData.Locale = CultureInfo.InvariantCulture

		'Database connection populated from the sample Northwind access database
		'
		Dim nwindConn As New SQLiteConnection()
		nwindConn.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		Dim selectCMD As New SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn)
		selectCMD.CommandTimeout = 30

		'Data adapter used to run the select command
		'
		Dim invoicesDA As New SQLiteDataAdapter()
		invoicesDA.SelectCommand = selectCMD

		'Fill the DataTable
		'
		invoicesDA.Fill(invoiceData)
		nwindConn.Close()

		'Create DataView and assign the selected CompanyName RowFilter
		'  
		Dim invoiceDataView As New DataView(invoiceData)
		invoiceDataView.RowFilter = "Customers_CompanyName='" + Convert.ToString(cbCompanyName.SelectedItem).Replace("'", "''") + "'"

		'Create the report and assign the data source
		'
		Dim rpt As New SectionReport()
		rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
		rpt.DataSource = invoiceDataView
		'Run and view the report
		'
		arvMain.LoadDocument(rpt)

		Cursor = Cursors.Arrow
	End Sub 'btnDataView_Click

	'Illustrates using a DataReader as a data source.
	'
	Private Sub btnDataReader_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDataReader.Click
		Cursor = Cursors.WaitCursor

		'Database connection populated from the sample Northwind access database
		'
		Dim nwindConn As New SQLiteConnection()
		nwindConn.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		Dim selectCMD As New SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn)
		selectCMD.CommandTimeout = 30

		'DataReader used to read the data
		'
		Dim invoiceDataReader As SQLiteDataReader

		'Open the database connection and Execute the reader
		'
		nwindConn.Open()
		invoiceDataReader = selectCMD.ExecuteReader()

		'Create the report and assign the data source
		'
		Dim rpt As New SectionReport()
		rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
		rpt.DataSource = invoiceDataReader
		'Run and view the report
		'
		arvMain.Document = rpt.Document
		rpt.Run(False)

		'Close the database connection
		'
		nwindConn.Close()

		Cursor = Cursors.Arrow
	End Sub 'btnDataReader_Click

	'Illustrates using a GrapeCity db object as a data source
	'
	Private Sub btnSqliteDb_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSqliteDb.Click
		Cursor = Cursors.WaitCursor

		'db Data Source object to use.
		'

		Dim db As New Data.DbDataSource(SQLiteFactory.Instance)

		' Assign the database connection string from the sample Northwind access database
		'
		db.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		db.SQL = "SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID"

		'Create the report and assign the data source
		'
		Dim rpt As New SectionReport()
		rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
		rpt.DataSource = db
		'Run and view the report
		'
		arvMain.LoadDocument(rpt)

		Cursor = Cursors.Arrow
	End Sub 'btnSqliteDb_Click

	'Generates a DataSet and saves it as an XML data file.
	'
	'
	Private Sub btnGenerateXML_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGenerateXML.Click
		'DataSet used to hold the Data.
		'
		Dim invoiceData As New DataSet("Northwind")
		invoiceData.Locale = CultureInfo.InvariantCulture

		'Database connection populated from the sample Northwind access database
		'
		Dim nwindConn As New SQLiteConnection()
		nwindConn.ConnectionString = My.Resources.ConnectionString

		'Run the SQL command.
		'
		Dim selectCMD As New SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn)
		selectCMD.CommandTimeout = 30

		'Data adapter used to run the select command
		'
		Dim invoicesDA As New SQLiteDataAdapter()
		invoicesDA.SelectCommand = selectCMD

		'Fill the DataSet
		'
		invoicesDA.FillSchema(invoiceData, SchemaType.Source, "Invoices")
		invoiceData.Tables("Invoices").Columns("OrderDate").DataType = System.Type.GetType("System.String")
		invoiceData.Tables("Invoices").Columns("ShippedDate").DataType = System.Type.GetType("System.String")
		invoicesDA.Fill(invoiceData, "Invoices")

		'Initalize the Save Dialog Box
		'

		dlgSave.Title = My.Resources.SaveDataAs

		dlgSave.FileName = "Invoices.xml"
		dlgSave.Filter = My.Resources.Filter
		If (dlgSave.ShowDialog() = DialogResult.OK) Then
			btnXML.Enabled = False
			Dim x As Integer
			'If valid name is returned, save out the DataSet to the specified filename
			'
			If dlgSave.FileName.Length <> 0 Then
				'Format all date fields in the XML for the report
				'
				For x = 0 To invoiceData.Tables("Invoices").Rows.Count - 1
					invoiceData.Tables("Invoices").Rows(x)("OrderDate") = Convert.ToDateTime(invoiceData.Tables("Invoices").Rows(x)("OrderDate")).ToShortDateString()
					If Not invoiceData.Tables("Invoices").Rows(x)("ShippedDate").GetType() Is Type.GetType("System.DBNull") Then
						invoiceData.Tables("Invoices").Rows(x)("ShippedDate") = Convert.ToDateTime(invoiceData.Tables("Invoices").Rows(x)("ShippedDate")).ToShortDateString()
					End If
				Next

				invoiceData.WriteXml(dlgSave.FileName)
			End If

			btnXML.Enabled = True
		End If
	End Sub

	'Illustrates using a GrapeCity XML object as a data source.
	'
	Private Sub btnXML_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnXML.Click
		'Initialize the Open Dialog Box
		'

		dlgOpen.Title = My.Resources.OpenDataFile

		dlgOpen.FileName = dlgSave.FileName
		dlgOpen.Filter = My.Resources.Filter

		If dlgOpen.ShowDialog() = Windows.Forms.DialogResult.OK Then

			'If valid name is returned, open the data and run the report
			'
			If dlgOpen.FileName.Length <> 0 Then
				Cursor = Cursors.WaitCursor

				'XML Data Source object to use
				'
				Dim xml As New Data.XMLDataSource() '  Dim _XML As New GrapeCity.ActiveReports.DataSources.XMLDataSource()

				'Assign the FileName for the selected XML data file
				'
				xml.FileURL = dlgOpen.FileName
				'Assign the Recordset Pattern
				'
				xml.RecordsetPattern = "//Northwind/Invoices"

				'Create the report and assign the data source
				'
				Dim rpt As New SectionReport()
				rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", My.Resources.ReportName)))
				rpt.DataSource = xml
				'Run and view the report
				'
				arvMain.LoadDocument(rpt)

				Cursor = Cursors.Arrow
			End If
		End If

	End Sub

	'Illustrates using a GrapeCity CSV object as a data source.
	Private Sub btnCSV_Click(sender As Object, e As EventArgs) Handles btnCSV.Click

		Const settingForNoHeaderDelimited As String = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued"

		Cursor = Cursors.WaitCursor
		'CSV Data Source object to use.
		Dim csv As New Data.CsvDataSource()

		'Dataset encoding.
		Dim encoding As String = My.Resources.CSVEncoding

		'Configure connection string by selected dataset type
		Dim connectionString As String = String.Empty

		'Delimited Data: No header, column separator is comma
		If rbtnNoHeaderComma.Checked Then
			connectionString = (Convert.ToString((Convert.ToString("Path=") & My.Resources.CSVDataSetPathComma) + ";" + "Encoding=") & encoding) + ";" + "TextQualifier="";" + "ColumnsSeparator=,;" + "RowsSeparator=\r\n;" + "Columns=" + settingForNoHeaderDelimited
			'Delimited Data: Header exists, column separator is Tab
		ElseIf rbtnHeaderTab.Checked Then

			connectionString = (Convert.ToString((Convert.ToString("Path=") & My.Resources.CSVDataSetPathHeaderTab) + ";" + "Encoding=") & encoding) + ";" + "TextQualifier="";" + "ColumnsSeparator=" & vbTab & ";" + "RowsSeparator=\r\n;" + "HasHeaders=True"
			'Fixed width Data: Header exists
		ElseIf rbtnHeader.Checked Then
			connectionString = (Convert.ToString((Convert.ToString("Path=") & My.Resources.CSVDataSetPathHeaderFixed) + ";" + "Encoding=") & encoding) + ";" +
			"Columns=" + _settingForHeaderExistsFixed + ";" +
			"HasHeaders=True"
			'Fixed width Data: No header
		ElseIf rbtnNoHeader.Checked Then
			connectionString = (Convert.ToString((Convert.ToString("Path=") & My.Resources.CSVDataSetPathFixed) + ";" + "Encoding=") & encoding) + ";" + "Columns=" + _settingForNoHeaderFixed
		End If
		'Applying specified connection string to data source
		csv.ConnectionString = connectionString

        'Create the report and assign the data source.
        Dim productList As New ProductList With {
            .ResourceLocator = New DefaultResourceLocator(New Uri(Path.GetDirectoryName(Application.ExecutablePath) + "\")),
            .DataSource = csv
        }

        'Run and view the report
        arvMain.LoadDocument(productList)

		Cursor = Cursors.Arrow

	End Sub
End Class
