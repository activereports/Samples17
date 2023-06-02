using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using GrapeCity.ActiveReports.Export.Pdf.Section.Signing;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using System.Resources;
using GrapeCity.ActiveReports.Document;

namespace GrapeCity.ActiveReports.Sample.DigitalSignaturePro
{
	public partial class PDFDigitalSignature : Form
	{
		private PageDocument _pageDocument;
		public PDFDigitalSignature()
		{
			InitializeComponent();
		}

		private void PDFDigitalSignature_Load(object sender, EventArgs e)
		{
			//Set the default for in the 'Signature Format' combo box.
			cmbVisibilityType.SelectedIndex = 3;
			var pageReport = new PageReport();
			_pageDocument = pageReport.Document;
			pageReport.Load(new FileInfo(@"..\..\..\..\..\Report\Catalog.rdlx"));
			arvMain.LoadDocument(_pageDocument);
		}

		private void pdfExportButton_Click(object sender, EventArgs e)
		{
			var pdfRE = new Export.Pdf.Page.PdfRenderingExtension();
			var settings = new Export.Pdf.Page.Settings();

			SaveFileDialog sfd = new SaveFileDialog();
			Cursor tmpCursor = Cursor;
			// Display the save dialog.
			
			sfd.Title = this.Text; //Title 
			sfd.FileName = "DigitalSignature.pdf"; // Name of the file for initial display
			sfd.Filter = "PDF|*.pdf"; // Filter
			if (sfd.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			try
			{
				// Change the cursor.
				Cursor = Cursors.WaitCursor;
				Application.DoEvents();

				// PAdES format
				settings.SignatureFormat = SignatureFormat.ETSI_CAdES_detached;
				settings.SignatureDigestAlgorithm = SignatureDigestAlgorithm.SHA256;

				// Sets the type of signature.
				settings.SignatureVisibilityType = (VisibilityType)cmbVisibilityType.SelectedIndex;

				// Set the signature display area.
				settings.SignatureStampBounds = new RectangleF(0.05F, 0.05F, 4.0F, 0.93F);

				// Sets the character position of the signature text
				settings.SignatureStampTextAlignment = Alignment.Left;

				settings.SignatureStampFontName = "Courier New";
				settings.SignatureStampFontSize = 10;

				// Set the rectangle in which the text is placed in the area that displays the signature.
				//  The coordinate specified in this property starts with the top left point, relative to the rectangular signature.
				settings.SignatureStampTextRectangle = new RectangleF(1.2F, 0.0F, 2.8F, 0.93F);

				// Set the signature image.
				settings.SignatureStampImageFileName = Path.GetFullPath(@"..\..\..\Image\gc.bmp");

				// Set the display position of the signature image.
				settings.SignatureStampImageAlignment = Alignment.Center;

				// Set the rectangle image so that it is placed in the area that displays the signature.
				// The coordinate specified in this property starts with the top left point, relative to the rectangular signature.
				settings.SignatureStampImageRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 0.93F);

				// Sets the password for the certificate and digital signature.
				// For X509Certificate2 class, etc. Please refer to the site of Microsoft.
				settings.SignatureCertificateFileName = Path.GetFullPath(@"..\..\..\certificate.pfx");
				settings.SignatureCertificatePassword = "test";
				// 
				if (chkTimeStamp.Checked)
				{
					settings.SignatureTimeStamp = new TimeStamp("https://tsa.wotrus.com", "", "");
				}

				// Sets the time stamp
				settings.SignatureSignDate = DateTime.Now.ToString(CultureInfo.InvariantCulture); // Signing time
				settings.SignatureDistinguishedNameVisible  = false; // Display whether or not the distinguished name
				settings.SignatureContact = new SignatureField<string>("activereports.support@grapecity.com", true); // Contact

				// Reason
				settings.SignatureReason = "I agree that is it a sample.";

				// Location
				settings.SignatureLocation = "Pittsburgh";

				var outputDirectory = new DirectoryInfo(Path.GetDirectoryName(sfd.FileName));
				var outputProvider = new Rendering.IO.FileStreamProvider(outputDirectory, sfd.FileName);
				outputProvider.OverwriteOutputFile = true;
				
				// Export the file
				_pageDocument.Render(pdfRE, outputProvider, settings);

				// Start the output file (Open)
				Process.Start(new ProcessStartInfo()
				{
					CreateNoWindow = true,
					UseShellExecute = true,
					Verb = "open",
					FileName = sfd.FileName
				});

				// Display the notification message.
				MessageBox.Show(Resource.FinishExportMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (PdfSigningException)
			{
				File.Delete(sfd.FileName);
				MessageBox.Show(Resource.LimitMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				// Replace the cursor
				Cursor = tmpCursor;
				Application.DoEvents();

				// End processing
				sfd.Dispose();
			}
		}
	}
}
