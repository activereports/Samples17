using GrapeCity.ActiveReports.Design.Advanced;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GrapeCity.ActiveReports.Samples.Radar
{
	static class Program
	{
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
#elif !NETCOREAPP3_1_OR_GREATER
			SetProcessDpiAwareness(_Process_DPI_Awareness.Process_DPI_Unaware);
#endif
			string _reportName = @"..\..\..\..\..\Report\Radar.rdlx";
			DesignerForm df = new DesignerForm();
			df.SessionSettingsStorage = new SessionSettingsStorage();
			df.ExportViewerFactory = new ExportViewerFactory();
			df.LoadReport(_reportName);
			Application.Run(df);
		}

#if !NETCOREAPP3_1_OR_GREATER
		[DllImport("shcore.dll")]
		static extern int SetProcessDpiAwareness(_Process_DPI_Awareness value);

		enum _Process_DPI_Awareness
		{
			Process_DPI_Unaware = 0,
			Process_System_DPI_Aware = 1,
			Process_Per_Monitor_DPI_Aware = 2
		}
#endif
	}
}
