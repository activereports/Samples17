using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GrapeCity.ActiveReports.Export.Pdf.Section.Signing;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using System.Resources;
using System.Diagnostics;

namespace GrapeCity.ActiveReports.Sample.DigitalSignaturePro
{
	public partial class PDFDigitalSignature : Form
	{
		public PDFDigitalSignature()
		{
			InitializeComponent();
		}

		private void PDFDigitalSignature_Load(object sender, EventArgs e)
		{
			// Set the default for in the 'Signature Format' combo box.
			cmbVisibilityType.SelectedIndex = 3;

			arvMain.LoadDocument("..//..//..//Report//Invoice.rpx");
		}

		private void pdfExportButton_Click(object sender, EventArgs e)
		{
			Export.Pdf.Section.PdfExport oPDFExport = new Export.Pdf.Section.PdfExport();
			SaveFileDialog sfd = new SaveFileDialog();
			Cursor tmpCursor = Cursor;
			string tempPath = string.Empty;
			// Display the save dialog.
			
			sfd.Title = this.Text; // Title 
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
				oPDFExport.Signature.SignatureFormat = SignatureFormat.ETSI_CAdES_detached;
				oPDFExport.Signature.SignatureDigestAlgorithm = SignatureDigestAlgorithm.SHA256;
				
				// Sets the type of signature.
				oPDFExport.Signature.VisibilityType = (VisibilityType)cmbVisibilityType.SelectedIndex;

				// Set the signature display area.
				oPDFExport.Signature.Stamp.Bounds = new RectangleF(0.05F, 0.05F, 4.0F, 0.93F);

				// Sets the character position of the signature text
				oPDFExport.Signature.Stamp.TextAlignment = Alignment.Left;

				oPDFExport.Signature.Stamp.Font = new Document.Drawing.Font("Courier New", 9.0F);

				// Set the rectangle in which the text is placed in the area that displays the signature.
				// The coordinate specified in this property starts with the top left point, relative to the rectangular signature.
				oPDFExport.Signature.Stamp.TextRectangle = new RectangleF(1.2F, 0.0F, 2.8F, 0.93F);

				// Set the signature image.
				using (var fs = new FileStream(Path.GetFullPath("..//..//..//Image//gc.bmp"), FileMode.Open, FileAccess.Read))
					oPDFExport.Signature.Stamp.Image = Document.Drawing.Image.FromStream(fs);

				// Set the display position of the signature image.
				oPDFExport.Signature.Stamp.ImageAlignment = Alignment.Center;

				// Set the rectangle image so that it is placed in the area that displays the signature.
				// The coordinate specified in this property starts with the top left point, relative to the rectangular signature.
				oPDFExport.Signature.Stamp.ImageRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 0.93f);

				// Sets the password for the certificate and digital signature.
				// For X509Certificate2 class, etc. Please refer to the site of Microsoft.
				oPDFExport.Signature.Certificate = new X509Certificate2(Path.GetFullPath("..//..//..//GrapeCity.pfx"), "password", X509KeyStorageFlags.Exportable);

				if (chkTimeStamp.Checked)
				{
					oPDFExport.Signature.TimeStamp = new TimeStamp("https://freetsa.org/tsr", "", "");
				}

				// Sets the time stamp.
				oPDFExport.Signature.SignDate = new SignatureField<DateTime>(DateTime.Now, true);		 // Signing time
				oPDFExport.Signature.DistinguishedName.Visible = false;								 // Display whether or not the distinguished name
				oPDFExport.Signature.Contact = new SignatureField<string>("activereports.support@grapecity.com", true);  // Contact
				
				oPDFExport.Signature.Reason = new SignatureField<string>("I agree that is it a sample.", true);
				// Reason
				
				oPDFExport.Signature.Location = new SignatureField<string>("Pittsburgh", true);
				
				// Export the file.
				if (File.Exists(sfd.FileName))
					File.Delete(sfd.FileName);
				oPDFExport.Export(arvMain.Document, sfd.FileName);

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
				File.Delete(tempPath);
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
				oPDFExport.Dispose();
			}
		}
	}
}
