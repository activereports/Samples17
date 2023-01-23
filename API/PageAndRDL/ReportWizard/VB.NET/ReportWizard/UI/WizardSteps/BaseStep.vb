Imports System.ComponentModel
Imports System.Windows.Forms
Partial Public Class BaseStep
    Inherits UserControl
    Private firstDisplay As Boolean
    Private stepTitle As String
    Private stepDescription As String
    Public Sub New()
        firstDisplay = True
        stepTitle = ""
        stepDescription = ""


        InitializeComponent()
    End Sub
    Private wizState As ReportWizardState
    <Browsable(False)>
    Public Property ReportWizardState() As ReportWizardState
        Get
            Return wizState
        End Get
        Set(ByVal value As ReportWizardState)
            wizState = value
        End Set
    End Property

    <Browsable(True)>
    <Description("The title to display in the wizard")>
    <Category("Appearance")>
    <DefaultValue("")>
    Public Property Title() As String
        Get
            Return stepTitle
        End Get
        Set(ByVal value As String)
            stepTitle = value
        End Set
    End Property

    <Browsable(True)>
    <Description("The text to display describing what to do on this step")>
    <Category("Appearance")>
    <DefaultValue("")>
    Public Property Description() As String

        Get
            Return stepDescription
        End Get
        Set(ByVal value As String)
            stepDescription = value
        End Set
    End Property
    Public Sub OnDisplay()
        OnDisplay(Me.firstDisplay)
        firstDisplay = False
    End Sub
    Protected Overridable Sub OnDisplay(ByVal firstDisplay As Boolean)
    End Sub
    'Called before the wizard moves to the previous or next step
    Public Overridable Sub UpdateState()
    End Sub
End Class
