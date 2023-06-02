using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using GrapeCity.ActiveReports.Design;
using System.Collections;
using GrapeCity.ActiveReports.Samples.ReportsGallery.Properties;
using GrapeCity.ActiveReports.Design.Advanced;

namespace GrapeCity.ActiveReports.Samples.ReportsGallery
{
	public partial class ReportsForm : Form
	{
		private string _reportName;
		static readonly string FolderPath = "";
		static readonly List<string> ExcludeFilesList = new List<string>();
		static readonly List<string> ExcludeFoldersList = new List<string>();

		public ReportsForm()
		{
			InitializeComponent();
		}
		
		//Load Settings from Config File
		static ReportsForm()
		{
			XDocument loaded = XDocument.Load("ReportsGallery.config");
			FolderPath = loaded.Descendants("FolderPath").Select(t => t.Value.ToString()).ToList()[0];
			DirectoryInfo reportbasefolder = new DirectoryInfo(FolderPath);
			ExcludeFilesList = loaded.Descendants("ExcludeFiles").ToList()[0].Descendants("File").Select(t => reportbasefolder.FullName + "\\" + t.Value.ToString()).ToList<string>();
			ExcludeFoldersList = loaded.Descendants("ExcludeFolders").ToList()[0].Descendants("Folder").Select(t => reportbasefolder.FullName + "\\" + t.Value.ToString()).ToList<string>();
		}

		// Form Load Event
		private void ReportsForm_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(FolderPath))
			{
				ListDirectory(treeView, FolderPath);
			}
			FolderLocalization();
			treeView.Nodes[1].Expand();
			treeView.Nodes[1].Nodes[0].Expand();
			var report = new FileInfo(treeView.Nodes[1].Nodes[0].Nodes[0].Tag.ToString());
			_reportName = report.Name;
		}

		// Add nodes to tree view
		private void ListDirectory(TreeView treeView, string path)
		{
			treeView.Nodes.Clear();
			var rootDirectoryInfo = new DirectoryInfo(path);
			foreach (var directory in rootDirectoryInfo.GetDirectories())
			{
				treeView.Nodes.Add(CreateDirectoryNode(directory));
			}
		}

		// Traverse Samples Folder and create tree
		private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeNode(directoryInfo.Name);
			foreach (var directory in directoryInfo.GetDirectories())
			{
				if (!ExcludeFoldersList.Exists(t => t.ToString() == directory.FullName))
				{
					directoryNode.Nodes.Add(CreateDirectoryNode(directory));
				}
			}
			foreach (var file in directoryInfo.GetFiles("*.rpx").Concat(directoryInfo.GetFiles("*.rdlx")))
			{
				if (!ExcludeFilesList.Exists(t => t.ToString() == file.FullName))
				{
					TreeNode reportFileNode = new TreeNode(file.Name);
					reportFileNode.ImageIndex = 2;
					reportFileNode.SelectedImageIndex = 2;
					reportFileNode.ForeColor = Color.Brown;
					reportFileNode.Tag = file.FullName;
					directoryNode.Nodes.Add(reportFileNode);
				}
			}
			return directoryNode;
		}

		//Folder Localization
		private void FolderLocalization()
		{
			Hashtable strReplace = new Hashtable();
			StreamReader reader = new StreamReader(new FileStream(@"..\..\..\ReportsGallery.config", FileMode.Open, FileAccess.Read, FileShare.Read));
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
				DesignerForm df = new DesignerForm();
				df.ExportViewerFactory = new ExportViewerFactory();
				df.SessionSettingsStorage = new SessionSettingsStorage();
				
				df.LoadReport(_reportName);
				df.Show();
			}
			else
			{
				if (e.Node.Parent != null)
				{
					if(e.Node.Parent.Parent!=null)
					{
						MessageBox.Show(Properties.Resources.InvalidFileText);
					}
				}
			}
		}
	}
}
