using System;
using System.Text;
using System.Windows.Forms;

namespace GrapeCity.ActiveReports.Sample.Inheritance
{
	/// <summary>
	/// A description of the overview of the ViewerForm.
	/// </summary>
	public partial class ViewerForm : System.Windows.Forms.Form
	{
		public ViewerForm()
		{
			//
			//Required for Windows Form Designer support
			//
			InitializeComponent();
		 }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
#if NET6_0_OR_GREATER
			Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif
			Application.Run(new ViewerForm());
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			rptInheritChild rpt = new rptInheritChild();
			arvMain.LoadDocument(rpt);
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			rptDesignChild rpt = new rptDesignChild();
			arvMain.LoadDocument(rpt);
		}
	}
}
