using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using GrapeCity.ActiveReports.Design;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using GrapeCity.ActiveReports.Calendar.Properties;

namespace GrapeCity.ActiveReports.Calendar
{
	public partial class DesignerForm : Form
	{
		static readonly string FolderPath = "";
		private string _reportName;

		static DesignerForm()
		{
			XDocument loaded = XDocument.Load("TestDesignerPro.config");
			FolderPath = loaded.Descendants("FolderPath").Select(t => t.Value.ToString()).ToList()[0];
		}

		public DesignerForm()
		{
			InitializeComponent();
		}

		// Handle Tree View collapse.
		private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			e.Node.ImageIndex = 0;
			e.Node.SelectedImageIndex = 0;
		}

		//Handle expansion of Tree View
		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			e.Node.ImageIndex = 1;
			e.Node.SelectedImageIndex = 1;
		}

		//Handle click on Tree Node
		private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if ((e.Node.Text.ToLower().Contains(".rdlx")) || (e.Node.Text.ToLower().Contains(".rpx")))
			{
				e.Node.ImageIndex = 2;
				treeView.SelectedNode = e.Node;
				FileInfo reportFile = new FileInfo(e.Node.Tag.ToString());
				_reportName = reportFile.FullName;
				ActiveReports.Design.Advanced.DesignerForm df = new ActiveReports.Design.Advanced.DesignerForm();
				df.LoadReport(_reportName);
				df.ExportViewerFactory = new ExportViewerFactory();
				df.SessionSettingsStorage = new SessionSettingsStorage();	
				df.Show();
			}
			else
			{
				if (e.Node.Parent != null)
				{
					if (e.Node.Parent.Parent != null)
					{
						MessageBox.Show(Properties.Resources.InvalidFileText);
					}
				}
			}
		}

		private void DesignerForm_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(FolderPath))
			{
				var rootDirectoryInfo = new DirectoryInfo(FolderPath);
				treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
			}
			FolderLocalization();
		}

		// Traverse Samples Folder and create tree
		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeNode(Properties.Resources.ReportsNode);
			foreach (var file in directoryInfo.GetFiles("*.rdlx"))
			{
				TreeNode reportFileNode = new TreeNode(file.Name);
				reportFileNode.ImageIndex = 2;
				reportFileNode.SelectedImageIndex = 2;
				reportFileNode.ForeColor = Color.Brown;
				reportFileNode.Tag = file.FullName;
				directoryNode.Nodes.Add(reportFileNode);
			}
			return directoryNode;
		}

		private void FolderLocalization()
		{
			Hashtable strReplace = new Hashtable();
			StreamReader reader = new StreamReader(new FileStream(@"TestDesignerPro.config", FileMode.Open, FileAccess.Read, FileShare.Read));
			XmlDocument doc = new XmlDocument();
			string xmlIn = reader.ReadToEnd();
			reader.Close();
			doc.LoadXml(xmlIn);
			foreach (XmlNode child in doc.ChildNodes[1].ChildNodes)
				if (child.Name.Equals("Localization"))
					foreach (XmlNode node in child.ChildNodes)
						if (node.Name.Equals("ReplaceName"))
							strReplace.Add
							(
								node.Attributes["OriginalName"].Value,
								node.Attributes["ReplaceWith"].Value
							);

			for (int i = 0; i < treeView.Nodes.Count; i++)
			{
				foreach (DictionaryEntry entry in strReplace)
				{
					if (treeView.Nodes[i].Text.Equals(entry.Key.ToString()))
					{
						treeView.Nodes[i].Text = entry.Value.ToString();
					}
				}
			}
		}
	}
}
