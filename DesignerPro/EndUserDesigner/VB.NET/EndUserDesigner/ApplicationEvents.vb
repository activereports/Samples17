Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
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
