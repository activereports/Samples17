
Imports GrapeCity.ActiveReports.Design.Advanced
Imports GrapeCity.ActiveReports.Samples.MapGallery

Friend Class SessionSettingsStorage
	Implements ISessionSettingsStorage

	Public Sub Load(setting As SessionSettings) Implements ISessionSettingsStorage.Load
		setting.RecentFiles = Settings.Default.RecentFiles
		setting.WindowSize = Settings.Default.WindowSize
		setting.WindowPosition = Settings.Default.WindowPosition
		setting.IsWindowMaximized = Settings.Default.IsWindowMaximized
		setting.RightPanelWidth = Settings.Default.RightPanelWidth
		setting.ActiveRightTab = Settings.Default.ActiveRightTab
		setting.LeftPanelWidth = Settings.Default.LeftPanelWidth
		setting.IsGroupEditorActive = Settings.Default.IsGroupEditorActive
		setting.IsReportExplorerActive = Settings.Default.IsReportExplorerActive
		setting.IsLayersListActive = Settings.Default.IsLayersListActive
	End Sub

	Public Sub Save(setting As SessionSettings) Implements ISessionSettingsStorage.Save
		Settings.Default.RecentFiles = setting.RecentFiles
		Settings.Default.WindowSize = setting.WindowSize
		Settings.Default.WindowPosition = setting.WindowPosition
		Settings.Default.IsWindowMaximized = setting.IsWindowMaximized
		Settings.Default.RightPanelWidth = setting.RightPanelWidth
		Settings.Default.ActiveRightTab = setting.ActiveRightTab
		Settings.Default.LeftPanelWidth = setting.LeftPanelWidth
		Settings.Default.IsGroupEditorActive = setting.IsGroupEditorActive
		Settings.Default.IsReportExplorerActive = setting.IsReportExplorerActive
		Settings.Default.IsLayersListActive = setting.IsLayersListActive
		Settings.Default.Save()
	End Sub

End Class
