Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
#If NET6_0_OR_GREATER Then
        Private Sub MyApplication_ApplyApplicationDefaults(sender As Object, e As ApplyApplicationDefaultsEventArgs) Handles Me.ApplyApplicationDefaults
            e.HighDpiMode = HighDpiMode.DpiUnawareGdiScaled
        End Sub
#End If

#If Not NETCOREAPP3_1_OR_GREATER Then
        Private Sub MyApplication_Startup(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            SetProcessDpiAwareness(_Process_DPI_Awareness.Process_DPI_Unaware)
        End Sub

        Declare Sub SetProcessDpiAwareness Lib "shcore" (ByVal value As _Process_DPI_Awareness)

        Enum _Process_DPI_Awareness
            Process_DPI_Unaware = 0
            Process_System_DPI_Aware = 1
            Process_Per_Monitor_DPI_Aware = 2
        End Enum
#End If
    End Class
End Namespace
