using System;
using System.Text;
using System.Windows.Forms;

namespace GrapeCity.ActiveReports.Samples.IListBinding
{
	class Program
	{
		[STAThread]
		public static void Main()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Application.EnableVisualStyles();
#if NET6_0_OR_GREATER
			Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif
			Application.Run(new BindIListToDataGridSample());
		}
	}
}
