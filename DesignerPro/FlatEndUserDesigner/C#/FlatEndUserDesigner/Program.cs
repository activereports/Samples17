using GrapeCity.ActiveReports.Design.Advanced;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GrapeCity.ActiveReports.Samples.FlatUserDesigner
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
			var designerForm = new DesignerForm
			{
				ExportViewerFactory = new ExportViewerFactory(),
				SessionSettingsStorage = new SessionSettingsStorage()
			};

#if !NETCOREAPP3_1_OR_GREATER
			SetProcessDpiAwareness(_Process_DPI_Awareness.Process_DPI_Unaware);
#endif

			Application.Run(designerForm);
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
