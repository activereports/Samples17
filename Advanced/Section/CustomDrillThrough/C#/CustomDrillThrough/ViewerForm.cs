using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GrapeCity.ActiveReports.Samples.CustomDrillThrough
{
	/// <summary>
	///Summary description for frmViewMain. 
	/// 
	///Demonstrates using hyperlinks and the viewer hyperlink event to simulate  
	///drill-through from one report to another.
	/// </summary>
	public partial class ViewerForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly bool _loadMainReport;

		public ViewerForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_loadMainReport = true;
		}

		public ViewerForm(bool loadMainReport)
		{
			InitializeComponent();
			_loadMainReport = loadMainReport;
		}

		/// <summary>
		/// The main entry point for the application.
		/// 
		/// </summary>
		[STAThread]
		static void Main()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if NET6_0_OR_GREATER
			Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif
			Application.Run(new ViewerForm());
		}

		/// <summary>
		/// Handles the hyperlink sent from the viewer object.
		/// </summary>
		private void arvMain_HyperLink(object sender, GrapeCity.ActiveReports.Viewer.Win.HyperLinkEventArgs e)
		{
			e.Handled = true;
			string hyperlink = e.HyperLink.Split(':')[1];
			string report = e.HyperLink.Split(':')[0];

			if (report == "DrillThrough1")
			{
				// Click on customer ID to open the drill through for that customer.
				var rpt2 = new SectionReport();
				rpt2.LoadLayout(XmlReader.Create(Properties.Resources.DrillThrough1));
				ViewerForm frm2 = new ViewerForm(false);
				rpt2.Parameters["customerID"].Value = hyperlink;
				frm2.arvMain.LoadDocument(rpt2);
				frm2.ShowDialog(this);
			}
			else if (report == "DrillThrough2")
			{
				// Click order number to open the order details
				var rpt3 = new SectionReport();
				rpt3.LoadLayout(XmlReader.Create(Properties.Resources.DrillThrough2));
				ViewerForm frm3 = new ViewerForm(false);
				rpt3.Parameters["orderID"].Value = hyperlink;
				frm3.arvMain.LoadDocument(rpt3);
				frm3.ShowDialog(this);
			}
		}

		/// <summary>
		/// ViewerForm_Load - Handles initial form setup.
		/// </summary>
		private void ViewerForm_Load(object sender, EventArgs e)
		{
			if(_loadMainReport)
			{
				var rpt = new SectionReport();
				rpt.LoadLayout(XmlReader.Create(Properties.Resources.DrillThroughMain));
				arvMain.LoadDocument(rpt);
			}
		}
	}
}
