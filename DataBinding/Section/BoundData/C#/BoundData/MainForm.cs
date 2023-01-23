using System;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;
using System.Data.SQLite;

namespace GrapeCity.ActiveReports.Samples.BoundData
{
	/// <summary>
	/// BoundDataSample - Illustrates how to bind data in ActiveReports.
	/// </summary>
	public partial class MainForm : System.Windows.Forms.Form
	{
		readonly string _settingForNoHeaderFixed;
		readonly string _settingForHeaderExistsFixed;
		public MainForm()
		{
			_settingForNoHeaderFixed = Properties.Resources.NoHeaderFixed;
			_settingForHeaderExistsFixed = Properties.Resources.HeaderExistsFixed;
			// Required for Windows Form Designer support
			InitializeComponent();
		}

		/// <summary>
		/// The main entry point for the application.>
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if NET6_0_OR_GREATER
			Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif
			Application.Run(new MainForm());
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			//Clear drop down lists.
			cbCompanyName.Items.Clear();
		}

		/// <summary>
		/// tabDataBinding_SelectedIndexChanged - Clear the viewer out when switching tabs.
		/// </summary>
		/// 
		private void tabDataBinding_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Clear existing pages
			arvMain.Document = new Document.SectionDocument();
		}

		/// <summary>
		/// Populate the DropDown list with company names from the Northwind access database.
		/// </summary>
		private void cbCompanyName_DropDown(object sender, EventArgs e)
		{
			//Populate the combo box if no items exist.
			if (cbCompanyName.Items.Count == 0)
			{
				Cursor = Cursors.WaitCursor;

				//Set up database connection.
				var nwindConn = new SQLiteConnection();
				nwindConn.ConnectionString = Properties.Resources.ConnectionString;
				//SQL Select statement used to get the Company Names
				var selectCMD = new SQLiteCommand("SELECT DISTINCT Customers_CompanyName from Invoices", nwindConn);
				nwindConn.Open();	
				var companyNamesDR = selectCMD.ExecuteReader();
				 //While the reader has data add a new Company Name to the list.
				while (companyNamesDR.Read())
				{
					cbCompanyName.Items.Add(companyNamesDR[0].ToString());
				}
				nwindConn.Close();
				//Set selection to first item in the list.
				cbCompanyName.SelectedIndex = 0;
				Cursor = Cursors.Arrow;
			}		
		}

		/// <summary>
		/// Illustrates using a DataSet as a data source.
		/// 
		/// </summary>
		private void btnDataSet_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			//Dataset to hold data.
			//
			DataSet invoiceData = new DataSet();
			invoiceData.Locale = CultureInfo.InvariantCulture;

			//Database connection populated from the sample Northwind access database
			//
			var nwindConn = new SQLiteConnection();			
			nwindConn.ConnectionString = Properties.Resources.ConnectionString;			
			//Run the SQL command.
			//
			var selectCMD = new SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn);
			selectCMD.CommandTimeout = 30;

			//Data adapter used to run the select command
			//
			var invoicesDA = new SQLiteDataAdapter();
			invoicesDA.SelectCommand = selectCMD;

			//Fill the DataSet.
			//
			invoicesDA.Fill(invoiceData, "Invoices");
			nwindConn.Close();

			//Create the report and assign the data source.
			//
			var rpt = new ActiveReports.SectionReport();
			rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\",Properties.Resources.ReportName)));
			rpt.DataSource = invoiceData;
			rpt.DataMember = invoiceData.Tables[0].TableName;
			//Run and view the report.
			//
			arvMain.LoadDocument(rpt);

			Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Illustrates using a DataTable as a data source
		/// 
		/// </summary>
		private void btnDataTable_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			//DataTable to hold the data.
			//
			DataTable invoiceData = new DataTable("Invoices");
			invoiceData.Locale = CultureInfo.InvariantCulture;

			//Database connection populated from the sample Northwind access database
			//
			var nwindConn = new SQLiteConnection();			
			nwindConn.ConnectionString = Properties.Resources.ConnectionString;			

			//Run the SQL command.
			//
			var selectCMD = new SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn);
			selectCMD.CommandTimeout = 30;

			//Data adapter used to run the select command
			//
			var invoicesDA = new SQLiteDataAdapter();
			invoicesDA.SelectCommand = selectCMD;

			//Fill the DataSet.
			//
			invoicesDA.Fill(invoiceData);
			nwindConn.Close();

			//Create the report and assign the data source.
			//
			var rpt = new ActiveReports.SectionReport();
			rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\", Properties.Resources.ReportName)));
			rpt.DataSource = invoiceData;
			//Run and view the report.
			//
			arvMain.LoadDocument(rpt);		

			Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Illustrates using a DataView as a data source
		/// 
		/// </summary>
		private void btnDataView_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			//Verify that a value Company Name is selected
			//
			if (cbCompanyName.SelectedItem == null)
			{
				MessageBox.Show(Properties.Resources.SelectCompanyName);
				Cursor = Cursors.Arrow;
				return;
			}

			//DataTable to hold the data.
			//
			DataTable invoiceData = new DataTable("Invoices");
			invoiceData.Locale = CultureInfo.InvariantCulture;

			//Database connection populated from the sample Northwind access database
			//
			var nwindConn = new SQLiteConnection();			
			nwindConn.ConnectionString = Properties.Resources.ConnectionString;			

			//Run the SQL command.
			//
			var selectCMD = new SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn);
			selectCMD.CommandTimeout = 30;

			//Data adapter used to run the select command
			//
			var invoicesDA = new SQLiteDataAdapter();
			invoicesDA.SelectCommand = selectCMD;

			//Fill the DataSet.
			//
			invoicesDA.Fill(invoiceData);
			nwindConn.Close();

			//Create a DataView and assign the selected CompanyName RowFilter.
			//  
			DataView invoiceDataView = new DataView(invoiceData);
			invoiceDataView.RowFilter = "Customers_CompanyName='" + Convert.ToString(cbCompanyName.SelectedItem).Replace("'", "''") + "'";

			//Create the report and assign the data source
			//
			var rpt = new ActiveReports.SectionReport();
			rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\",Properties.Resources.ReportName)));
			rpt.DataSource = invoiceDataView;
			//Run and view the report
			//
			arvMain.LoadDocument(rpt);		

			Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Illustrates using a DataReader as a data source.
		/// 
		/// </summary>
		private void btnDataReader_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			//Database connection populated from the sample Northwind access database
			//
			var nwindConn = new SQLiteConnection();
			nwindConn.ConnectionString = Properties.Resources.ConnectionString;

			//Run the SQL command.
			//
			var selectCMD = new SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn);
			selectCMD.CommandTimeout = 30;

			//Open the database connection and execute the reader
			//
			nwindConn.Open();
			var invoiceDataReader = selectCMD.ExecuteReader();

			//Create the report and assign the data source.
			//
			var rpt = new ActiveReports.SectionReport();
			rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\",Properties.Resources.ReportName)));
			rpt.DataSource = invoiceDataReader;
			//Run and view the report
			//
			arvMain.Document = rpt.Document;
						rpt.Run(false);
			
			//Close the database connection
			//
			nwindConn.Close();

			Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// Illustrates using a GrapeCity db object as a data source
		/// 
		/// </summary>
		private void btnSqliteDb_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			//db Data Source object to use.
			//
			Data.DbDataSource db = new Data.DbDataSource(SQLiteFactory.Instance);
			
			//Assign the database connection string from the sample Northwind access database
			//
			db.ConnectionString = Properties.Resources.ConnectionString;
			
			//Run the SQL command.
			//
			db.SQL = "SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID";

			//Create the report and assign the data source
			//
			var rpt = new ActiveReports.SectionReport();
			rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\",Properties.Resources.ReportName)));
			rpt.DataSource = db;
			//Run and view the report
			//
			arvMain.LoadDocument(rpt);

			Cursor = Cursors.Arrow;
		}
		
		/// <summary>
		/// Generates a DataSet and saves it as an XML data file.
		/// 
		/// </summary>
		private void btnGenerateXML_Click(object sender, EventArgs e)
		{
			//DataSet used to hold the Data.
			//
			DataSet invoiceData = new DataSet("Northwind");
			invoiceData.Locale = CultureInfo.InvariantCulture;

			//Database connection populated from the sample Northwind access database
			//
			var nwindConn = new SQLiteConnection();
			nwindConn.ConnectionString = Properties.Resources.ConnectionString;			

			//SQL Select command to run against the database
			//
			var selectCMD = new SQLiteCommand("SELECT * FROM Invoices ORDER BY Customers_CompanyName, OrderID", nwindConn);
			selectCMD.CommandTimeout = 30;

			//Data adapter used to run the select command
			//
			var invoicesDA = new SQLiteDataAdapter();
			invoicesDA.SelectCommand = selectCMD;

			//Fill the DataSet
			//
			invoicesDA.FillSchema(invoiceData, SchemaType.Source, "Invoices");
			invoiceData.Tables["Invoices"].Columns["OrderDate"].DataType = Type.GetType("System.String");
			invoiceData.Tables["Invoices"].Columns["ShippedDate"].DataType = Type.GetType("System.String");
			invoicesDA.Fill(invoiceData, "Invoices");

			//Initalize the Save Dialog Box
			//
			dlgSave.Title =  Properties.Resources.SaveDataAs;
			dlgSave.FileName = "Invoices.xml";
			dlgSave.Filter = Properties.Resources.Filter;
			if (dlgSave.ShowDialog() == DialogResult.OK)
			{
				btnXML.Enabled = false;
				//If valid name is returned, save out the DataSet to the specified filename
				//
				if (dlgSave.FileName.Length != 0)
				{
					//Format all date fields in the XML for the report
					//
					for (int x = 0; x < invoiceData.Tables["Invoices"].Rows.Count; x++)
					{
						invoiceData.Tables["Invoices"].Rows[x]["OrderDate"] = Convert.ToDateTime(invoiceData.Tables["Invoices"].Rows[x]["OrderDate"]).ToShortDateString();
						if (invoiceData.Tables["Invoices"].Rows[x]["ShippedDate"].GetType() != Type.GetType("System.DBNull"))
						{
							invoiceData.Tables["Invoices"].Rows[x]["ShippedDate"] = Convert.ToDateTime(invoiceData.Tables["Invoices"].Rows[x]["ShippedDate"]).ToShortDateString();
						}
					}
					invoiceData.WriteXml(dlgSave.FileName);
				}
				btnXML.Enabled = true;
			}
		}

		/// <summary>
		/// Illustrates using a GrapeCity XML object as a data source.
		/// 
		/// </summary>
		private void btnXML_Click(object sender, EventArgs e)
		{
			//Initialize the Open Dialog Box
			//
			
			dlgOpen.Title = Properties.Resources.OpenDataFile;
			dlgOpen.FileName = dlgSave.FileName;
			dlgOpen.Filter = Properties.Resources.Filter;

			if (dlgOpen.ShowDialog() == DialogResult.OK)
			{
				//If valid name is returned, open the data and run the report
				//
				if (dlgOpen.FileName.Length != 0)
				{
					Cursor = Cursors.WaitCursor;

					//XML Data Source object to use
					Data.XMLDataSource xml = new Data.XMLDataSource();

					//Assign the FileName for the selected XML data file
					xml.FileURL = dlgOpen.FileName;
					//Assign the Recordset Pattern
					xml.RecordsetPattern = @"//Northwind/Invoices";

					//Create the report and assign the data source
					var rpt = new ActiveReports.SectionReport();
					rpt.LoadLayout(XmlReader.Create(System.IO.Path.Combine("..\\..\\..\\",Properties.Resources.ReportName)));
					rpt.DataSource = xml;
					//Run and view the report
					arvMain.LoadDocument(rpt);

					Cursor = Cursors.Arrow;
				}
			}
		}

		private void btnCSV_Click(object sender, EventArgs e)
		{
			const string settingForNoHeaderDelimited = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued";

			Cursor = Cursors.WaitCursor;
			//CSV Data Source object to use.
			Data.CsvDataSource csv = new Data.CsvDataSource();


			//Dataset encoding.
			string encoding = Properties.Resources.CSVEncoding;

			//Configure connection string by selected dataset type
			string connectionString = string.Empty;

			//Delimited Data: No header, column separator is comma
			if (rbtnNoHeaderComma.Checked)
			{
				connectionString = @"Path=" + Properties.Resources.CSVDataSetPathComma + ";" +
								   "Encoding=" + encoding + ";" +
								   "TextQualifier=\";" +
								   "ColumnsSeparator=,;" +
								   "RowsSeparator=\\r\\n;" +
								   "Columns=" + settingForNoHeaderDelimited;
			}
			//Delimited Data: Header exists, column separator is Tab
			else if (rbtnHeaderTab.Checked)
			{
				connectionString = @"Path=" + Properties.Resources.CSVDataSetPathHeaderTab + ";" +
								   "Encoding=" + encoding + ";" +
								   "TextQualifier=\";" +
								   "ColumnsSeparator=\t;" +
								   "RowsSeparator=\\r\\n;" +
								   "HasHeaders=True";

			}
			//Fixed width Data: Header exists
			else if (rbtnHeader.Checked)
			{
				connectionString = @"Path=" + Properties.Resources.CSVDataSetPathHeaderFixed + ";" +
								   "Encoding=" + encoding + ";" +
								   "Columns=" + _settingForHeaderExistsFixed + ";" +
								   "HasHeaders=True";
			}
			//Fixed width Data: No header
			else if (rbtnNoHeader.Checked)
			{
				connectionString = @"Path=" + Properties.Resources.CSVDataSetPathFixed + ";" +
								   "Encoding=" + encoding + ";" +
								   "Columns=" + _settingForNoHeaderFixed;
			}
			//Applying specified connection string to data source
			csv.ConnectionString = connectionString;

			//Create the report and assign the data source.
			ProductList productList = new ProductList
			{
				ResourceLocator = new DefaultResourceLocator(new Uri(Path.GetDirectoryName(Application.ExecutablePath) + "\\")),
				DataSource = csv
			};

			//Run and view the report
			arvMain.LoadDocument(productList);

			Cursor = Cursors.Arrow;
		}
	}
}
