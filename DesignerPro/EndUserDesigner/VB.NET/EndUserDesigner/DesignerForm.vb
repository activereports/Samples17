Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.IO
Imports System.Reflection
Imports GrapeCity.ActiveReports.Design.ReportsLibrary
Imports GrapeCity.ActiveReports.PageReportModel
Imports GrapeCity.ActiveReports.Design
Imports GrapeCity.ActiveReports.Viewer.Win.Internal.Export
Imports GrapeCity.ActiveReports.Design.Tools
Imports GrapeCity.ActiveReports.Viewer.Win
Imports Image = System.Drawing.Image
Imports Color = System.Drawing.Color
Imports GrapeCity.Viewer.Common.ViewModel

Partial Public Class DesignerForm
    Inherits Form

    Private Const ShowReportsLibraryKey As String = "ShowReportsLibrary"
    Private Const DefaultReportType As ExportForm.ReportType = ExportForm.ReportType.PageCpl
    Private _reportName As String
    Private _reportType As DesignerReportType
    Private _exportReportType As ExportForm.ReportType = DefaultReportType
    Private _isDirty As Boolean
    Private _exportMenuItem As ToolStripMenuItem
    Private _exportForm As ExportForm
    Private Const ReportPartsDirectoryKey As String = "ReportPartsDirectory"

    Public Sub New()
        InitializeComponent()
        Font = DefaultFontFactory.DefaultWinFormsFont
        arPropertyGrid.Font = DefaultFontFactory.DefaultWinFormsFont
        Icon = My.Resources.App
        arDesigner.Toolbox = arToolbox
        AddHandler arDesigner.LayoutChanged, AddressOf OnDesignerLayoutChanged
        reportExplorer.ReportDesigner = arDesigner
        layerList.ReportDesigner = arDesigner
        groupEditor.ReportDesigner = arDesigner
        reportsLibrary.ReportDesigner = arDesigner
        Dim config = GlobalServices.Instance.EnsureConfigurationManager()
        If Not String.IsNullOrEmpty(config.Settings(ReportPartsDirectoryKey)) Then arDesigner.ReportPartsDirectory = config.Settings(ReportPartsDirectoryKey)
        InitiazlizeToolstrip()
        arToolbox.SelectedCategory = My.Resources.DefaultGroup
        SetReportName(Nothing)
        Dim showResult As Boolean
        AddHandler arDesigner.ReportChanged, AddressOf OnUpdateReportName
        AddHandler arDesigner.LayoutChanged, Sub(__, args)
                                                 If args.Type = LayoutChangeType.ReportLoad OrElse args.Type = LayoutChangeType.ReportClear Then
                                                     reportsLibrary.Visible = Not (TypeOf arDesigner.Report Is SectionReport) AndAlso (Boolean.TryParse(config.Settings(ShowReportsLibraryKey), showResult) AndAlso showResult)
                                                     RefreshExportEnabled()
                                                 End If
                                             End Sub

        reportsLibrary.Visible = Not (TypeOf arDesigner.Report Is SectionReport) AndAlso (Boolean.TryParse(config.Settings(ShowReportsLibraryKey), showResult) AndAlso showResult)
        RefreshExportEnabled()
        RefreshLayersTab()
        RefreshGroupEditor()
        InitGroupEditorToggle()
        AddHandler Load, AddressOf DesignerForm_Load
        AllowDrop = True
        AddHandler DragEnter, AddressOf DesignerForm_DragEnter
        AddHandler DragDrop, AddressOf DesignerForm_DragDrop
    End Sub

    Private Sub InitiazlizeToolstrip()
        InitializeMenuStrip()
        InitializeReportStrip()
    End Sub

    Private Sub InitializeMenuStrip()
        Dim menuStrip As ToolStrip = arDesigner.CreateToolStrips(DesignerToolStrips.Menu)(0)
        menuStrip.Items.Add(CreateHelpMenu())
        menuStrip.Font = DefaultFontFactory.DefaultWinFormsFont
        CreateFileMenu(CType(menuStrip.Items(0), ToolStripDropDownItem))
        AppendToolStrips(0, {menuStrip})
    End Sub

    Private Sub InitializeReportStrip()
        AppendToolStrips(1, arDesigner.CreateToolStrips(DesignerToolStrips.Edit, DesignerToolStrips.Undo, DesignerToolStrips.Zoom))
        Dim reportStrip As ToolStrip = CreateReportToolbar()
        reportStrip.Font = Font
        AppendToolStrips(1, New List(Of ToolStrip) From {
                reportStrip
            })
        AppendToolStrips(2, arDesigner.CreateToolStrips(DesignerToolStrips.Format, DesignerToolStrips.Layout))
    End Sub

    Private Sub DesignerForm_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
        If Visible AndAlso Not CanFocus Then Return

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filename As String = (CType(e.Data.GetData(DataFormats.FileDrop), String()))(0)
            Dim fi = New FileInfo(filename)

            If (fi.Extension = ".rdlx") OrElse (fi.Extension = ".rpx") OrElse (fi.Extension = ".rdlx-master") OrElse (fi.Extension = ".rdl") Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        End If
    End Sub

    Private Sub DesignerForm_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
        Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files.Length <= 0 Then Return
        If Not ConfirmSaveChanges(Me) Then Return
        TryLoadReport(files(0))
    End Sub

    Private Sub DesignerForm_Load(ByVal sender As Object, ByVal e As EventArgs)
        _exportReportType = DefaultReportType
        Dim commandLineArgs As String() = Environment.GetCommandLineArgs()

        If commandLineArgs.Length > 1 Then
            TryLoadReport(commandLineArgs(1))
        End If
    End Sub

    Private Sub RefreshExportEnabled()
        RemoveHandler arDesigner.ActiveTabChanged, AddressOf OnEnableExport
        AddHandler arDesigner.ActiveTabChanged, AddressOf OnEnableExport
        OnEnableExport(Me, EventArgs.Empty)
    End Sub

    Private Sub OnEnableExport(ByVal sender As Object, ByVal eventArgs As EventArgs)
        _exportMenuItem.Enabled = IsEnabledExportButton()
    End Sub

    Private Function IsEnabledExportButton() As Boolean
        Return arDesigner.ActiveTab = DesignerTab.Preview
    End Function

    Private Sub OnDesignerLayoutChanged(ByVal sender As Object, ByVal e As LayoutChangedArgs)
        If e.Type = LayoutChangeType.ReportLoad OrElse e.Type = LayoutChangeType.ReportClear Then
            arToolbox.Reorder(arDesigner)
            arToolbox.EnsureCategories()
            arToolbox.Refresh()
            RefreshLayersTab()
            RefreshGroupEditor()
        End If

        If e.Type = LayoutChangeType.ReportClear Then
            _isDirty = False
            _exportReportType = DetermineReportType()
            SetReportName(Nothing)
        End If

        If e.Type = LayoutChangeType.ReportLoad Then

            If Not String.IsNullOrEmpty(_reportName) Then
                _isDirty = _exportReportType <> DetermineReportType()

                If GetIsMaster() Then
                    Dim extension = Path.GetExtension(_reportName)

                    If Not String.IsNullOrEmpty(extension) AndAlso (extension.ToLowerInvariant() = ".rdl" OrElse extension.ToLowerInvariant() = ".rdlx") Then
                        _reportName = $"{Path.GetFileNameWithoutExtension(_reportName)}.rdlx-master"
                        _isDirty = File.Exists(_reportName)
                    End If
                End If
            End If

            _exportReportType = DetermineReportType()
        End If

        BeginInvoke(New MethodInvoker(AddressOf UpdateReportName))
    End Sub

    Private Sub RefreshLayersTab()
        If arDesigner.ReportType = DesignerReportType.Section Then

            If splitContainerProperties.Panel1.Controls.Contains(tabControl1) Then
                reportExplorerTabPage.SuspendLayout()
                splitContainerProperties.Panel1.SuspendLayout()
                reportExplorerTabPage.Controls.Remove(ReportExplorer)
                splitContainerProperties.Panel1.Controls.Remove(tabControl1)
                splitContainerProperties.Panel1.Controls.Add(ReportExplorer)
                reportExplorerTabPage.ResumeLayout()
                splitContainerProperties.Panel1.ResumeLayout()
            End If
        ElseIf Not splitContainerProperties.Panel1.Controls.Contains(tabControl1) Then
            reportExplorerTabPage.SuspendLayout()
            splitContainerProperties.Panel1.SuspendLayout()
            splitContainerProperties.Panel1.Controls.Remove(ReportExplorer)
            reportExplorerTabPage.Controls.Add(ReportExplorer)
            splitContainerProperties.Panel1.Controls.Add(tabControl1)
            reportExplorerTabPage.ResumeLayout()
            splitContainerProperties.Panel1.ResumeLayout()
        End If
    End Sub

    Private _groupEditorSize As Integer

    Private Sub InitGroupEditorToggle()
        GroupEditorToggleButton.Image = My.Resources.GroupEditorHide
        AddHandler GroupEditorToggleButton.MouseEnter, Function(sender, args)
                                                           Return GroupEditorToggleButton.BackColor = Color.LightGray
                                                       End Function

        AddHandler GroupEditorToggleButton.MouseLeave, Function(sender, args)
                                                           Return GroupEditorToggleButton.BackColor = Color.Gainsboro
                                                       End Function

        AddHandler GroupEditorToggleButton.Click, Sub(sender, args)

                                                      If groupEditor.Visible Then
                                                          GroupEditorToggleButton.Image = My.Resources.GroupEditorShow
                                                          _groupEditorSize = splitContainerGroupEditor.ClientSize.Height - splitContainerGroupEditor.SplitterDistance
                                                          splitContainerGroupEditor.SplitterDistance = splitContainerGroupEditor.ClientSize.Height - GroupEditorSeparator.Height - GroupEditorContainer.Padding.Vertical - splitContainerGroupEditor.Panel2.Padding.Vertical - splitContainerGroupEditor.SplitterWidth
                                                          splitContainerGroupEditor.IsSplitterFixed = True
                                                          groupEditor.Visible = False
                                                      Else
                                                          GroupEditorToggleButton.Image = My.Resources.GroupEditorHide
                                                          splitContainerGroupEditor.SplitterDistance = CInt(If(_groupEditorSize < splitContainerGroupEditor.ClientSize.Height, splitContainerGroupEditor.ClientSize.Height - _groupEditorSize, splitContainerGroupEditor.ClientSize.Height * 2 / 3))
                                                          splitContainerGroupEditor.IsSplitterFixed = False
                                                          groupEditor.Visible = True
                                                      End If
                                                  End Sub

        AddHandler groupEditor.VisibleChanged, Sub(sender, args)
                                                   GroupPanelVisibility.SetToolTip(GroupEditorToggleButton, If(groupEditor.Visible, My.Resources.HideGroupPanelToolTip, My.Resources.ShowGroupPanelToolTip))
                                               End Sub
    End Sub

    Private Sub RefreshGroupEditor()
        splitContainerGroupEditor.Panel2Collapsed = arDesigner.ReportType = DesignerReportType.Section
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        e.Cancel = e.Cancel Or Not ConfirmSaveChanges(Me)
        MyBase.OnClosing(e)
    End Sub

    Private Sub OnUpdateReportName(ByVal sender As Object, ByVal args As EventArgs)
        SetReportName(_reportName)
    End Sub

    Private Sub UpdateReportName()
        SetReportName(_reportName)
    End Sub

    Private Sub AppendToolStrips(ByVal row As Integer, ByVal toolStrips As IList(Of ToolStrip))
        Dim panel As ToolStripPanel = toolStripContainer1.TopToolStripPanel
        Dim i As Integer = toolStrips.Count

        While System.Threading.Interlocked.Decrement(i) >= 0
            panel.Join(toolStrips(i), row)
        End While
    End Sub

    Private Function ConfirmSaveChanges(ByVal control As Control) As Boolean
        If IsDirty Then
            Dim fileName = If(_reportName IsNot Nothing, Path.GetFileName(_reportName), GetDefaultReportName(_reportType))
            Dim result As DialogResult = MessageBox.Show(control, String.Format(My.Resources.EudUnsavedChangesMessage, fileName), My.Resources.EudUnsavedChangesTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Cancel Then
                Return False
            End If

            If result = DialogResult.Yes Then
                Return PerformSave()
            End If
        End If

        Return True
    End Function

    Private Sub OnNew(ByVal sender As Object, ByVal e As EventArgs)
        If Not ConfirmSaveChanges(Me) Then Return
        arDesigner.ExecuteAction(DesignerAction.NewReport)
    End Sub

    Private Sub OnOpen(ByVal sender As Object, ByVal e As EventArgs)
        If Not ConfirmSaveChanges(Me) Then
            Return
        End If

        openDialog.FileName = String.Empty
        Dim selectedIndex As Integer = Nothing

        If TypeOf arDesigner.Report Is SectionReport Then
            If openDialog.ShowDialog(Me) = DialogResult.OK Then TryLoadReport(openDialog.FileName)
        ElseIf openDialog.ShowDialog(Me, {My.Resources.OpenAsLibrary}, selectedIndex) = DialogResult.OK Then

            If selectedIndex = 0 Then
                Dim host = TryCast((CType(arDesigner.Report, IComponent)).Site.GetService(GetType(IDesignerHost)), IDesignerHost)
                Dim reportsLibraryService = TryCast(host?.GetService(GetType(IReportsLibraryService)), IReportsLibraryService)
                reportsLibraryService?.LoadReport(openDialog.FileName)
            Else
                TryLoadReport(openDialog.FileName)
            End If
        End If
    End Sub

    Private Sub TryLoadReport(ByVal fileName As String)
        Try
            arDesigner.LoadReport(New FileInfo(fileName))
            UpdateReport(fileName)
        Catch
            MessageBox.Show(Me, String.Format(My.Resources.ReportIsNotValid, fileName), My.Resources.DesignerFormTitle, MessageBoxButtons.OK, MessageBoxIcon.[Error])

            If String.IsNullOrEmpty(_reportName) Then
                arDesigner.NewReport(_reportType)
                UpdateReport(Nothing)
                Return
            End If

            TryLoadReport(_reportName)
        End Try
    End Sub

    Private Sub UpdateReport(ByVal reportName As String)
        _isDirty = False
        _exportReportType = DetermineReportType()
        SetReportName(reportName)
    End Sub

    Private Function PerformSave() As Boolean
        If String.IsNullOrEmpty(_reportName) OrElse String.IsNullOrEmpty(Path.GetDirectoryName(_reportName)) OrElse Not File.Exists(_reportName) Then
            Return PerformSaveAs()
        End If

        arDesigner.SaveReport(New FileInfo(_reportName))
        _isDirty = False
        UpdateReportName()
        Return True
    End Function

    Private Shared Function GetSaveFilter(ByVal type As DesignerReportType, ByVal isMaster As Boolean) As String
        Select Case type
            Case DesignerReportType.Section
                Return My.Resources.SaveRpxFilter
            Case DesignerReportType.Page
                Return My.Resources.SaveRdlxFilter
            Case DesignerReportType.RdlMultiSection
                Return If(isMaster, My.Resources.SaveRdlxMasterFilter, My.Resources.SaveRdlMultiSectionFilter)
            Case DesignerReportType.RdlDashboard
                Return My.Resources.SaveRdlDashboardFilter
            Case DesignerReportType.Rdl
                Return If(isMaster, My.Resources.SaveRdlxMasterFilter, My.Resources.SaveRdlFilter)
            Case Else
                Return My.Resources.SaveRpxFilter
        End Select
    End Function

    Private Function PerformSaveAs() As Boolean
        Dim fileName = If(_reportName IsNot Nothing, Path.GetFileName(_reportName), GetDefaultReportName(_reportType))
        saveDialog.FileName = fileName
        saveDialog.Filter = GetSaveFilter(arDesigner.ReportType, GetIsMaster())

        If saveDialog.ShowDialog() = DialogResult.OK Then
            arDesigner.SaveReport(New FileInfo(saveDialog.FileName))
            _isDirty = False
            SetReportName(saveDialog.FileName)
            Return True
        End If

        Return False
    End Function

    Private Sub PerformExport()
        If _exportForm Is Nothing Then
            _exportForm = New ExportForm()
        End If

        _exportReportType = DetermineReportType()
        _exportForm.Show(Me, New ExportViewer(arDesigner.ReportViewer), _exportReportType)
    End Sub

    Private Function DetermineReportTypeByOpenedReport() As ExportForm.ReportType
        Dim openedReport As OpenedReport = arDesigner.ReportViewer.OpenedReport

        Select Case openedReport
            Case OpenedReport.Fpl
                Return ExportForm.ReportType.PageFpl
            Case OpenedReport.Dashboard
                Return ExportForm.ReportType.Dashboard
            Case OpenedReport.Section
                Return ExportForm.ReportType.Section
            Case Else
                Return DefaultReportType
        End Select
    End Function

    Private Function DetermineReportType() As ExportForm.ReportType
        If arDesigner.ActiveTab = DesignerTab.Preview Then
            Return DetermineReportTypeByOpenedReport()
        End If

        Dim sectionReport = TryCast(arDesigner.Report, SectionReport)
        If sectionReport IsNot Nothing Then Return ExportForm.ReportType.Section
        Dim pageReport = TryCast(arDesigner.Report, PageReport)
        If pageReport Is Nothing Then Return DefaultReportType
        Dim report = pageReport.Report
        If report IsNot Nothing AndAlso report.ViewerType = ViewerType.Dashboard Then Return ExportForm.ReportType.Dashboard
        If report?.ReportSections.Count = 0 Then Return DefaultReportType
        Dim items As ReportItemCollection = report?.ReportSections(0).Body?.ReportItems
        Return If(items IsNot Nothing AndAlso items.Count = 1 AndAlso TypeOf items(0) Is FixedPage, ExportForm.ReportType.PageFpl, ExportForm.ReportType.PageCpl)
    End Function

    Private Function GetIsMaster() As Boolean
        Dim isMaster As Boolean = False

        If arDesigner.ReportType = DesignerReportType.Rdl OrElse arDesigner.ReportType = DesignerReportType.RdlMultiSection Then
            Dim report = CType(arDesigner.Report, Component)
            Dim site = If(report Is Nothing, Nothing, report.Site)

            If site IsNot Nothing Then
                Dim host = TryCast(site.GetService(GetType(IDesignerHost)), IDesignerHost)

                If host IsNot Nothing Then
                    Dim mcs = TryCast(host.GetService(GetType(IMasterContentService)), IMasterContentService)
                    isMaster = mcs IsNot Nothing AndAlso mcs.IsMaster
                End If
            End If
        End If

        Return isMaster
    End Function

    Private Sub OnSave(ByVal sender As Object, ByVal e As EventArgs)
        PerformSave()
    End Sub

    Private Sub OnSaveAs(ByVal sender As Object, ByVal e As EventArgs)
        PerformSaveAs()
    End Sub

    Private Sub OnExport(ByVal sender As Object, ByVal e As EventArgs)
        PerformExport()
    End Sub

    Private Sub OnExit(ByVal sender As Object, ByVal e As EventArgs)
        Close()
    End Sub

    Private Sub OnAbout(ByVal sender As Object, ByVal e As EventArgs)
        Const showAboutBoxMethodName As String = "ShowAboutBox"
        Dim attributes = arDesigner.[GetType]().GetCustomAttributes(GetType(LicenseProviderAttribute), False)
        If attributes.Length <> 1 Then Return
        Dim attr = CType(attributes(0), LicenseProviderAttribute)
        Dim provider = (CType(Activator.CreateInstance(attr.LicenseProvider), LicenseProvider))
        Dim methodInfo = provider.[GetType]().GetMethod(showAboutBoxMethodName, BindingFlags.NonPublic Or BindingFlags.Instance)
        If methodInfo IsNot Nothing Then methodInfo.Invoke(provider, New Object() {arDesigner.[GetType]()})
    End Sub

    Private Function CreateHelpMenu() As ToolStripDropDownItem
        Dim helpMenu = New ToolStripMenuItem(My.Resources.Help, Nothing, New ToolStripItem() {})
        helpMenu.DropDownItems.Clear()
        helpMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.About, Nothing, AddressOf OnAbout))
        Return helpMenu
    End Function

    Private Sub CreateFileMenu(ByVal fileMenu As ToolStripDropDownItem)
        _exportMenuItem = New ToolStripMenuItem(My.Resources.Export, My.Resources.CmdExport, AddressOf OnExport, Keys.Control Or Keys.E)
        fileMenu.DropDownItems.Clear()
        fileMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.NewText, My.Resources.CmdNewReport, AddressOf OnNew, Keys.Control Or Keys.N))
        fileMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.Open, My.Resources.CmdOpen, AddressOf OnOpen, Keys.Control Or Keys.O))
        fileMenu.DropDownItems.Add(New ToolStripSeparator())
        fileMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.Save, My.Resources.CmdSave, AddressOf OnSave, Keys.Control Or Keys.S))
        fileMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.SaveAs, My.Resources.CmdSaveAs, AddressOf OnSaveAs))
        fileMenu.DropDownItems.Add(New ToolStripSeparator())
        fileMenu.DropDownItems.Add(_exportMenuItem)
        fileMenu.DropDownItems.Add(New ToolStripSeparator())
        fileMenu.DropDownItems.Add(New ToolStripMenuItem(My.Resources.ExitText, Nothing, AddressOf OnExit))
        _exportMenuItem.Enabled = IsEnabledExportButton()
    End Sub

    Private Function CreateReportToolbar() As ToolStrip
        Return New ToolStrip({CreateToolStripButton(My.Resources.NewText, My.Resources.CmdNewReport, New EventHandler(AddressOf OnNew), My.Resources.NewToolTip), CreateToolStripButton(My.Resources.Open, My.Resources.CmdOpen, New EventHandler(AddressOf OnOpen), My.Resources.OpenToolTip), CreateToolStripButton(My.Resources.Save, My.Resources.CmdSave, New EventHandler(AddressOf OnSave), My.Resources.SaveToolTip)}) With {
                .AccessibleName = "toolStripFile"
            }
    End Function

    Private Shared Function CreateToolStripButton(ByVal text As String, ByVal image As Image, ByVal handler As EventHandler, ByVal toolTip As String) As ToolStripButton
        Dim button = New ToolStripButton(text, image, handler) With {
                .DisplayStyle = ToolStripItemDisplayStyle.Image,
                .ToolTipText = toolTip,
                .DoubleClickEnabled = True
            }
        Return button
    End Function

    Private Sub SetReportName(ByVal reportName As String)
        _reportType = arDesigner.ReportType
        _reportName = reportName
        Text = String.Format(My.Resources.TitleFormat, If(String.IsNullOrEmpty(reportName), GetDefaultReportName(_reportType), Path.GetFileName(reportName)), If(IsDirty, My.Resources.DirtySign, String.Empty), My.Resources.DesignerFormTitle)
    End Sub

    Private ReadOnly Property IsDirty As Boolean
        Get
            Return arDesigner.IsDirty OrElse _isDirty
        End Get
    End Property

    Private Function GetDefaultReportName(ByVal reportType As DesignerReportType) As String
        Select Case reportType
            Case DesignerReportType.Section
                Return My.Resources.DefaultReportNameRpx
            Case DesignerReportType.Rdl
                Return If(GetIsMaster(), My.Resources.DefaultReportNameRdlxMaster, My.Resources.DefaultReportNameRdl)
            Case DesignerReportType.Page, DesignerReportType.RdlMultiSection, DesignerReportType.RdlDashboard
                Return My.Resources.DefaultReportNameRdlx
        End Select

        Throw New ApplicationException("Unsupported report type: " & reportType)
    End Function
End Class
