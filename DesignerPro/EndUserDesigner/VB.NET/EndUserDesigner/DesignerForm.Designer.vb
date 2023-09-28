<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DesignerForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer


    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DesignerForm))
        Me.toolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.splitContainerToolbox = New System.Windows.Forms.SplitContainer()
        Me.splitContainerDesigner = New System.Windows.Forms.SplitContainer()
        Me.splitContainerGroupEditor = New System.Windows.Forms.SplitContainer()
        Me.arPropertyGrid = New System.Windows.Forms.PropertyGrid()
        Me.GroupEditorContainer = New System.Windows.Forms.Panel()
        Me.GroupEditorSeparator = New System.Windows.Forms.Panel()
        Me.GroupEditorToggleButton = New System.Windows.Forms.PictureBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.splitContainerProperties = New System.Windows.Forms.SplitContainer()
        Me.tabControl1 = New System.Windows.Forms.TabControl()
        Me.reportExplorerTabPage = New System.Windows.Forms.TabPage()
        Me.layersTabPage = New System.Windows.Forms.TabPage()
        Me.statusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.openDialog = New System.Windows.Forms.OpenFileDialog()
        Me.saveDialog = New System.Windows.Forms.SaveFileDialog()
        Me.GroupPanelVisibility = New System.Windows.Forms.ToolTip(Me.components)
        Me.splitContainerReportsLibrary = New System.Windows.Forms.SplitContainer()
        Me.arToolbox = New GrapeCity.ActiveReports.Design.Toolbox.Toolbox()
        Me.reportsLibrary = New GrapeCity.ActiveReports.Design.ReportsLibrary.ReportsLibrary()
        Me.arDesigner = New GrapeCity.ActiveReports.Design.Designer()
        Me.groupEditor = New GrapeCity.ActiveReports.Design.GroupEditor.GroupEditor()
        Me.reportExplorer = New GrapeCity.ActiveReports.Design.ReportExplorer.ReportExplorer()
        Me.layerList = New GrapeCity.ActiveReports.Design.LayerList()
        Me.toolStripContainer1.ContentPanel.SuspendLayout()
        Me.toolStripContainer1.SuspendLayout()
        Me.splitContainerToolbox.Panel1.SuspendLayout()
        Me.splitContainerToolbox.Panel2.SuspendLayout()
        Me.splitContainerToolbox.SuspendLayout()
        Me.splitContainerDesigner.Panel1.SuspendLayout()
        Me.splitContainerDesigner.Panel2.SuspendLayout()
        Me.splitContainerDesigner.SuspendLayout()
        Me.splitContainerGroupEditor.Panel1.SuspendLayout()
        Me.splitContainerGroupEditor.Panel2.SuspendLayout()
        Me.splitContainerGroupEditor.SuspendLayout()
        Me.GroupEditorContainer.SuspendLayout()
        Me.GroupEditorSeparator.SuspendLayout()
        CType((Me.GroupEditorToggleButton), System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainerProperties.Panel1.SuspendLayout()
        Me.splitContainerProperties.Panel2.SuspendLayout()
        Me.splitContainerProperties.SuspendLayout()
        Me.tabControl1.SuspendLayout()
        Me.reportExplorerTabPage.SuspendLayout()
        Me.layersTabPage.SuspendLayout()
        Me.statusStrip1.SuspendLayout()
        Me.splitContainerReportsLibrary.Panel1.SuspendLayout()
        Me.splitContainerReportsLibrary.Panel2.SuspendLayout()
        Me.splitContainerReportsLibrary.SuspendLayout()
        CType((Me.arToolbox), System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        Me.toolStripContainer1.ContentPanel.Controls.Add(Me.splitContainerToolbox)
        resources.ApplyResources(Me.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel")
        resources.ApplyResources(Me.toolStripContainer1, "toolStripContainer1")
        Me.toolStripContainer1.Name = "toolStripContainer1"
        resources.ApplyResources(Me.splitContainerToolbox, "splitContainerToolbox")
        Me.splitContainerToolbox.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitContainerToolbox.Name = "splitContainerToolbox"
        Me.splitContainerToolbox.Panel1.Controls.Add(Me.splitContainerReportsLibrary)
        Me.splitContainerToolbox.Panel2.Controls.Add(Me.splitContainerDesigner)
        resources.ApplyResources(Me.splitContainerDesigner, "splitContainerDesigner")
        Me.splitContainerDesigner.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.splitContainerDesigner.Name = "splitContainerDesigner"
        Me.splitContainerDesigner.Panel1.Controls.Add(Me.splitContainerGroupEditor)
        Me.splitContainerDesigner.Panel2.Controls.Add(Me.splitContainerProperties)
        resources.ApplyResources(Me.splitContainerGroupEditor, "splitContainerGroupEditor")
        Me.splitContainerGroupEditor.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.splitContainerGroupEditor.Name = "splitContainerGroupEditor"
        Me.splitContainerGroupEditor.Panel1.Controls.Add(Me.arDesigner)
        Me.splitContainerGroupEditor.Panel2.Controls.Add(Me.GroupEditorContainer)
        resources.ApplyResources(Me.splitContainerGroupEditor.Panel2, "splitContainerGroupEditor.Panel2")
        resources.ApplyResources(Me.arPropertyGrid, "arPropertyGrid")
        Me.arPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar
        Me.arPropertyGrid.Name = "arPropertyGrid"
        Me.GroupEditorContainer.BackColor = System.Drawing.Color.FromArgb((CInt(((CByte((160)))))), (CInt(((CByte((160)))))), (CInt(((CByte((160)))))))
        Me.GroupEditorContainer.Controls.Add(Me.groupEditor)
        Me.GroupEditorContainer.Controls.Add(Me.GroupEditorSeparator)
        resources.ApplyResources(Me.GroupEditorContainer, "GroupEditorContainer")
        Me.GroupEditorContainer.Name = "GroupEditorContainer"
        Me.GroupEditorSeparator.BackColor = System.Drawing.Color.Gainsboro
        Me.GroupEditorSeparator.Controls.Add(Me.GroupEditorToggleButton)
        Me.GroupEditorSeparator.Controls.Add(Me.label1)
        resources.ApplyResources(Me.GroupEditorSeparator, "GroupEditorSeparator")
        Me.GroupEditorSeparator.Name = "GroupEditorSeparator"
        resources.ApplyResources(Me.GroupEditorToggleButton, "GroupEditorToggleButton")
        Me.GroupEditorToggleButton.Name = "GroupEditorToggleButton"
        Me.GroupEditorToggleButton.TabStop = False
        resources.ApplyResources(Me.label1, "label1")
        Me.label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.label1.Name = "label1"
        Me.label1.UseCompatibleTextRendering = True
        resources.ApplyResources(Me.splitContainerProperties, "splitContainerProperties")
        Me.splitContainerProperties.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitContainerProperties.Name = "splitContainerProperties"
        Me.splitContainerProperties.Panel1.Controls.Add(Me.tabControl1)
        Me.splitContainerProperties.Panel2.Controls.Add(Me.arPropertyGrid)
        Me.tabControl1.Controls.Add(Me.reportExplorerTabPage)
        Me.tabControl1.Controls.Add(Me.layersTabPage)
        resources.ApplyResources(Me.tabControl1, "tabControl1")
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.reportExplorerTabPage.Controls.Add(Me.reportExplorer)
        resources.ApplyResources(Me.reportExplorerTabPage, "reportExplorerTabPage")
        Me.reportExplorerTabPage.Name = "reportExplorerTabPage"
        Me.layersTabPage.Controls.Add(Me.layerList)
        resources.ApplyResources(Me.layersTabPage, "layersTabPage")
        Me.layersTabPage.Name = "layersTabPage"
        Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabel1})
        resources.ApplyResources(Me.statusStrip1, "statusStrip1")
        Me.statusStrip1.Name = "statusStrip1"
        Me.toolStripStatusLabel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
        resources.ApplyResources(Me.toolStripStatusLabel1, "toolStripStatusLabel1")
        Me.toolStripStatusLabel1.Spring = True
        resources.ApplyResources(Me.openDialog, "openDialog")
        resources.ApplyResources(Me.splitContainerReportsLibrary, "splitContainerReportsLibrary")
        Me.splitContainerReportsLibrary.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.splitContainerReportsLibrary.Name = "splitContainerReportsLibrary"
        Me.splitContainerReportsLibrary.Panel1.Controls.Add(Me.arToolbox)
        Me.splitContainerReportsLibrary.Panel2.Controls.Add(Me.reportsLibrary)
        Me.arToolbox.DesignerHost = Nothing
        resources.ApplyResources(Me.arToolbox, "arToolbox")
        Me.arToolbox.Name = "arToolbox"
        resources.ApplyResources(Me.reportsLibrary, "reportsLibrary")
        Me.reportsLibrary.Name = "reportsLibrary"
        Me.reportsLibrary.ReportDesigner = Nothing
        resources.ApplyResources(Me.arDesigner, "arDesigner")
        Me.arDesigner.IsDirty = False
        Me.arDesigner.LockControls = False
        Me.arDesigner.Name = "arDesigner"
        Me.arDesigner.PromptUser = False
        Me.arDesigner.PropertyGrid = Me.arPropertyGrid
        Me.arDesigner.ReportTabsVisible = True
        Me.arDesigner.ShowDataSourceIcon = True
        Me.arDesigner.Toolbox = Nothing
        Me.arDesigner.ToolBoxItem = Nothing
        Me.groupEditor.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.groupEditor, "groupEditor")
        Me.groupEditor.Name = "groupEditor"
        Me.groupEditor.ReportDesigner = Nothing
        resources.ApplyResources(Me.reportExplorer, "reportExplorer")
        Me.reportExplorer.Name = "reportExplorer"
        Me.reportExplorer.ReportDesigner = Nothing
        resources.ApplyResources(Me.layerList, "layerList")
        Me.layerList.Name = "layerList"
        Me.layerList.ReportDesigner = Nothing
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.toolStripContainer1)
        Me.Controls.Add(Me.statusStrip1)
        Me.Name = "DesignerForm"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.toolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.toolStripContainer1.ResumeLayout(False)
        Me.toolStripContainer1.PerformLayout()
        Me.splitContainerToolbox.Panel1.ResumeLayout(False)
        Me.splitContainerToolbox.Panel2.ResumeLayout(False)
        Me.splitContainerToolbox.ResumeLayout(False)
        Me.splitContainerDesigner.Panel1.ResumeLayout(False)
        Me.splitContainerDesigner.Panel2.ResumeLayout(False)
        Me.splitContainerDesigner.ResumeLayout(False)
        Me.splitContainerGroupEditor.Panel1.ResumeLayout(False)
        Me.splitContainerGroupEditor.Panel2.ResumeLayout(False)
        Me.splitContainerGroupEditor.ResumeLayout(False)
        Me.GroupEditorContainer.ResumeLayout(False)
        Me.GroupEditorSeparator.ResumeLayout(False)
        CType((Me.GroupEditorToggleButton), System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainerProperties.Panel1.ResumeLayout(False)
        Me.splitContainerProperties.Panel2.ResumeLayout(False)
        Me.splitContainerProperties.ResumeLayout(False)
        Me.tabControl1.ResumeLayout(False)
        Me.reportExplorerTabPage.ResumeLayout(False)
        Me.layersTabPage.ResumeLayout(False)
        Me.statusStrip1.ResumeLayout(False)
        Me.statusStrip1.PerformLayout()
        Me.splitContainerReportsLibrary.Panel1.ResumeLayout(False)
        Me.splitContainerReportsLibrary.Panel2.ResumeLayout(False)
        Me.splitContainerReportsLibrary.ResumeLayout(False)
        CType((Me.arToolbox), System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents arPropertyGrid As System.Windows.Forms.PropertyGrid
    Private WithEvents reportExplorer As GrapeCity.ActiveReports.Design.ReportExplorer.ReportExplorer
    Private WithEvents statusStrip1 As System.Windows.Forms.StatusStrip
    Private WithEvents toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents splitContainerProperties As SplitContainer
    Private WithEvents splitContainerToolbox As System.Windows.Forms.SplitContainer
    Private WithEvents arToolbox As GrapeCity.ActiveReports.Design.Toolbox.Toolbox
    Private WithEvents splitContainerDesigner As System.Windows.Forms.SplitContainer
    Private WithEvents toolStripContainer1 As System.Windows.Forms.ToolStripContainer
    Private WithEvents openDialog As System.Windows.Forms.OpenFileDialog
    Private WithEvents saveDialog As System.Windows.Forms.SaveFileDialog
    Private WithEvents tabControl1 As TabControl
    Private WithEvents reportExplorerTabPage As TabPage
    Private WithEvents layersTabPage As TabPage
    Private WithEvents layerList As GrapeCity.ActiveReports.Design.LayerList
    Private WithEvents splitContainerGroupEditor As SplitContainer
    Private WithEvents arDesigner As Design.Designer
    Private WithEvents GroupEditorContainer As Panel
    Private WithEvents groupEditor As GrapeCity.ActiveReports.Design.GroupEditor.GroupEditor
    Private WithEvents GroupEditorSeparator As Panel
    Private WithEvents GroupEditorToggleButton As PictureBox
    Private WithEvents label1 As Label
    Private WithEvents GroupPanelVisibility As ToolTip
    Private WithEvents splitContainerReportsLibrary As SplitContainer
    Private WithEvents reportsLibrary As Design.ReportsLibrary.ReportsLibrary
End Class
