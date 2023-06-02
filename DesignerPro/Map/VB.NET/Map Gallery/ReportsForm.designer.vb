<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReportsForm
	Inherits System.Windows.Forms.Form


	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub


	Private components As System.ComponentModel.IContainer




	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ReportsForm))
		Me.GroupPanelVisibility = New System.Windows.Forms.ToolTip(Me.components)
		Me.treeView = New System.Windows.Forms.TreeView()
		Me.SuspendLayout()
		'
		'treeView
		'
		resources.ApplyResources(Me.treeView, "treeView")
		Me.treeView.Name = "treeView"
		'
		'ReportsForm
		'
		resources.ApplyResources(Me, "$this")
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.treeView)
		Me.Name = "ReportsForm"
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents GroupPanelVisibility As ToolTip
	Friend WithEvents treeView As TreeView
End Class
