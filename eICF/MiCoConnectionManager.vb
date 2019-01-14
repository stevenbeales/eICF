Imports System.Data.Common
Imports System.IO
Imports MiCo.MiForms
Imports MiCo.MiForms.Server

''' <summary>
''' MiCoConnectionManager class receives data requests and dispatches them to MiCo.
''' </summary>
Public Class MiCoConnectionManager
    Inherits ConnectionManager

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connection">Connection</param>
    ''' <param name="provider">SharePoint provider</param>
    ''' <param name="credentials">User Credentials</param>
    Public Sub New(connection As DbConnection, provider As SharePointContextProvider, credentials As Credentials)
        MyBase.New(connection, provider)
        UserCredentials = credentials
    End Sub

    ''' <summary>
    ''' Authenticate User
    ''' </summary>
    ''' <param name="wsi">Web Service</param>
    ''' <returns>True or false</returns>
    Public Overrides Function AuthenticateUser(ByRef wsi As WebserviceInterfaceEx) As Boolean
        Try
            Dim responseCode As ServerResponse = wsi.VerifyServer()
            If Not responseCode.Success Or Not responseCode.BoolData Then
                ErrorLog.Write("Exception: Error Verifying Server in Connection Manager", responseCode.Error.Exception.StackTrace, responseCode.Error.Exception.Message)
            End If
            responseCode = wsi.VerifyLogin()
            If responseCode.Success And responseCode.BoolData Then
                Return True
            Else
                ErrorLog.Write("Exception: Error Verifying Login in Connection Manager", responseCode.Error.Exception.StackTrace, responseCode.Error.Exception.Message)
                Return False
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in Authenticate User:", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Get Mico Form Templates
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>List of form templates</returns>
    Public Overrides Function GetFormTemplates(user As User) As FormTemplateDescription()
        Dim wsi As New WebserviceInterfaceEx()
        Try
            InitializeServerInterface(wsi)
            Dim success As Boolean = AuthenticateUser(wsi)
            If success Then
                Dim sr As ServerResponse = wsi.GetFormTemplatesForUser()
                If Not sr.Success Then
                    ErrorLog.Write("Couldn't find form templates for ", user.FullName)
                End If
                Return sr.FormTemplates
            Else
                ErrorLog.Write("GetFormTemplates: Couldn't authenticate server for ", user.FullName)
                Dim offline As New OfflineConnectionManager(Connection, Provider)
                Return offline.GetFormTemplates(user)
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception finding form templates ", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetFormTemplates(user)
        End Try
    End Function

    ''' <summary>
    ''' Get Current Revision of Mico Form Template
    ''' </summary>
    ''' <param name="form">Form Template</param>
    ''' <returns>XML for Mico Form Template Revision</returns>
    Public Overrides Function GetFormTemplateRevision(form As FormTemplateDescription) As String
        Dim wsi As New WebserviceInterfaceEx()
        Try
            InitializeServerInterface(wsi)
            Dim success As Boolean = AuthenticateUser(wsi)
            If success Then
                Dim sr As ServerResponse = wsi.GetFormTemplateRevision(form.FormID, form.Revision)
                If sr.Success Then
                    Return sr.StringData
                Else
                    ErrorLog.Write("GetFormTemplateRevision: Couldn't Get Form Template", form.Name)
                    Return ""
                End If
            Else
                ErrorLog.Write("GetFormTemplateRevision: Couldn't authenticate server", "")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception finding form template Revision ", ex.StackTrace, ex.Message)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Gets the Mi-Co preferences file
    ''' </summary>
    ''' <returns>Prefs file name and location</returns>
    Private Function GetPrefsfile() As String
        Dim prefFile As String = ""
        Try
            prefFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            prefFile = Path.Combine(prefFile, "Mi-Co")
            prefFile = Path.Combine(prefFile, "Mi-Forms")
            prefFile = Path.Combine(prefFile, "prefs.xml")
            If prefFile = "" Then
                ErrorLog.Write("Cannot find Mi-Co prefs file", "GetPrefsFile()")
                AMsgBox.Show(CANNOT_FIND_PREFS_FILE, MsgBoxStyle.Critical)
            End If
            Return prefFile
        Catch ex As Exception
            ErrorLog.Write("GetPrefsfile Error", ex.StackTrace, ex.Message)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Gets 5 digit site number from MiCo
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>5 digit string</returns>
    Public Overrides Function GetSiteNumber(site As Site) As String
        Dim wsi As New WebserviceInterfaceEx()
        Try
            InitializeServerInterface(wsi)
            Dim success As Boolean = AuthenticateUser(wsi)
            If success Then
                Dim sr As ServerResponse = wsi.GetGroupsForUser()
                Dim groupNames As String() = sr.StringDataArray
                Dim siteNumber As String = ""
                For Each groupName As String In groupNames
                    If groupName.Contains(MICO_GROUP_PREFIX) Then
                        siteNumber = Right(groupName, SITE_NUMBER_LENGTH)
                        Exit For
                    End If
                Next
                If Not siteNumber = "" Then
                    Dim localDb As New LocalDatabase(Connection)
                    localDb.SaveSiteInDatabase(siteNumber, "", "")
                    Return siteNumber
                Else
                    ErrorLog.Write("User not found: " & site.UserName, "GetSiteNumber MiCo")
                    Return ""
                End If
            Else
                ErrorLog.Write("Cannot authenticate user: " & site.UserName, "GetSiteNumber MiCo")
                Dim offline As New OfflineConnectionManager(Connection, Provider)
                Return offline.GetSiteNumber(site)
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetSiteNumber MiCo", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetSiteNumber(site)
        End Try
    End Function

    ''' <summary>
    ''' Initialize server
    ''' </summary>
    ''' <param name="wsi">Web service interface</param>
    Public Overrides Sub InitializeServerInterface(ByRef wsi As WebserviceInterfaceEx)
        Try
            Dim valueStore As New XMLValueStore(PrefsFile)
            wsi.NetworkSettings.Server = valueStore.Get("Network_HostName")
            wsi.NetworkSettings.UseDefaultProxySettings = valueStore.GetBool("Network_DefaultProxy")
            wsi.NetworkSettings.Port = valueStore.GetNumber("Network_Port")
            wsi.NetworkSettings.SSL = valueStore.GetBool("Network_UseHTTPS")
            wsi.NetworkSettings.URLPrefix = "MFS"
            wsi.Credentials.CustomerName = "ePS"
            wsi.Credentials.User = UserCredentials.Username
            wsi.Credentials.Password = Encryption.EncryptPassword(UserCredentials.PasswordHash)

            If UserCredentials.Username = "User" Then 'We are in debug mode
                wsi.Credentials.User = "SC.Samuel"
                wsi.Credentials.Password = "Welcome1"
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception: InitializeServerInterface ", ex.StackTrace, ex.Message & " " & UserCredentials.PasswordHash)
        End Try
    End Sub

    ''' <summary>
    ''' MiCo Preferences File
    ''' </summary>
    ''' <returns>Preference file XML</returns>
    Public ReadOnly Property PrefsFile As String
        Get
            If _prefsFile = "" Then
                _prefsFile = GetPrefsfile()
            End If
            Return _prefsFile
        End Get
    End Property

    Protected Overrides Function GetOnlineStatus() As Boolean
        Try
            Dim wsi As New WebserviceInterfaceEx()
            InitializeServerInterface(wsi)
            Dim sr As ServerResponse = wsi.VerifyServer()
            ErrorLog.Write("Get Online Status", sr.Success.ToString())
            Return sr.Success
        Catch ex As Exception
            ErrorLog.Write("Get Online Status Error", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    Private _prefsFile As String = ""
End Class
