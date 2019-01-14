Imports System.Data.Common
Imports MiCo.MiForms
Imports MiCo.MiForms.Server

''' <summary>
''' ConnectionManager class directs requests to online portal, Mi-Co or local database
''' ConnectionManager class receives data requests And dispatches them to SharePoint if eICF Is online Or Access if offline.
''' </summary>
Public Class ConnectionManager
    Implements IConnectionManager
    Implements ISiteDetails

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connection">Connection</param>
    ''' <param name="provider">SharePoint Provider</param>
    Public Sub New(connection As DbConnection, provider As SharePointContextProvider)
        _connection = connection
        _provider = provider
    End Sub

    ''' <summary>
    ''' Setup our web server call into Mi-Co server by reading from MiCo prefs file
    ''' </summary>
    ''' <param name="wsi">Web Service Interface</param>
    Public Overridable Sub InitializeServerInterface(ByRef wsi As WebserviceInterfaceEx) Implements IConnectionManager.InitializeServerInterface
        GetMiCoConnectionManager().InitializeServerInterface(wsi)
    End Sub

    ''' <summary>
    ''' Verify server and user connectivity to MiCo Server
    ''' </summary>
    ''' <param name="wsi">Web Service Interface</param>
    ''' <returns>True or False</returns>
    Public Overridable Function AuthenticateUser(ByRef wsi As WebserviceInterfaceEx) As Boolean Implements IConnectionManager.AuthenticateUser
        Return GetMiCoConnectionManager().AuthenticateUser(wsi)
    End Function

      ''' <summary>
    ''' Gets Next Subject Number for Site
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Subject Number as String</returns>
    Public Overridable Function NextSubjectNumber(site As Site) As String Implements ISiteDetails.NextSubjectNumber
        Return GetOfflineConnectionManager().NextSubjectNumber(site)
    End Function

    ''' <summary>
    ''' Get Form Template Revision
    ''' </summary>
    ''' <param name="form">Form</param>
    ''' <returns>Revision Number</returns>
    Public Overridable Function GetFormTemplateRevision(form As FormTemplateDescription) As String Implements  ISiteDetails.GetFormTemplateRevision
        Return GetMicoConnectionManager().GetFormTemplateRevision(form)
    End Function

    ''' <summary>
    ''' Get Phone
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>Phone Number</returns>
    Public Overridable Function GetPhone(user As User) As String Implements  ISiteDetails.GetPhone
        Return GetConnectionManager().GetPhone(user)
    End Function

    ''' <summary>
    ''' Get Form Templates
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>List of Form Templates</returns>
    Public Overridable Function GetFormTemplates(user As User) As FormTemplateDescription() Implements  ISiteDetails.GetFormTemplates
        Return GetMicoConnectionManager().GetFormTemplates(user)
    End Function

      ''' <summary>
    ''' Gets Site Number
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Returns 5 Digit Site Number</returns>
    Public Overridable Function GetSiteNumber(site As Site) As String Implements  ISiteDetails.GetSiteNumber
        Return GetConnectionManager().GetSiteNumber(site)
    End Function

    ''' <summary>
    ''' Gets local database ID for site 
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Site ID</returns>
    Public Overridable Function GetSiteId(site As Site) As Integer Implements  ISiteDetails.GetSiteId
        Return GetOfflineConnectionManager().GetSiteId(site)
    End Function

    ''' <summary>
    ''' Gets local database ID for user
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>User ID</returns>
    Public Overridable Function GetUserId(user As User) As Integer Implements  ISiteDetails.GetUserId
        Return GetOfflineConnectionManager().GetUserId(user)
    End Function

    ''' <summary>
    ''' Gets local database ID for subject 
    ''' </summary>
    ''' <param name="subject">Subject</param>
    ''' <returns>Subject ID</returns>
    Public Overridable Function GetSubjectId(subject As Subject) As Integer Implements  ISiteDetails.GetSubjectId
        Return GetOfflineConnectionManager().GetSubjectId(subject)
    End Function

    ''' <summary>
    ''' List of subjects at site loaded from local database
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>List of Subjects</returns>
    Public Overridable Function GetSubjects(site As Site) As ArrayList Implements  ISiteDetails.GetSubjects
        Return GetOfflineConnectionManager().GetSubjects(site)
    End Function


    ''' <summary>
    ''' List of visits for subject loaded from local database
    ''' </summary>
    ''' <param name="subject">Subject</param>
    ''' <returns>List of Visits</returns>
    Public Overridable Function GetVisits(subject As Subject) As ArrayList Implements  ISiteDetails.GetVisits
        Return GetOfflineConnectionManager().GetVisits(subject)
    End Function

    ''' <summary>
    ''' Get Site PI
    ''' </summary>
    ''' <param name="site">Study site</param>
    ''' <returns>PI</returns>
    Public Overridable Function GetPI(site As Site) As User Implements  ISiteDetails.GetPI
        Return GetConnectionManager().GetPI(site)
    End Function

    ''' <summary>
    ''' Get PI Phone
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Phone Number</returns>
    Public Overridable Function GetPIPhone(site As Site) As String Implements  ISiteDetails.GetPIPhone
        Return GetConnectionManager().GetPIPhone(site)
    End Function

    ''' <summary>
    ''' Get PI First Name
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>First Name</returns>
    Public Overridable Function GetPIFirstName(site As Site) As String Implements  ISiteDetails.GetPIFirstName
        Return GetConnectionManager().GetPIFirstName(site)
    End Function


    ''' <summary>
    ''' Get PI Last Name
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Last Name</returns>
    Public Overridable Function GetPILastName(site As Site) As String Implements  ISiteDetails.GetPILastName
        Return GetConnectionManager().GetPILastName(site)
    End Function


    ''' <summary>
    ''' Get Site Address
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Address</returns>
    Public Overridable Function GetSiteAddress(site As Site) As String Implements  ISiteDetails.GetSiteAddress
        Return GetConnectionManager().GetSiteAddress(site)
    End Function

    ''' <summary>
    ''' Is the system online?
    ''' </summary>
    ''' <returns>True or false</returns>
    Public ReadOnly Property Online As Boolean Implements IConnectionManager.Online
        Get
            Return GetOnlineStatus()
        End Get
    End Property

    ''' <summary>
    ''' Logged in user credentials
    ''' </summary>
    ''' <returns>User credentials</returns>
    Public Property UserCredentials As Credentials Implements IConnectionManager.UserCredentials
        Get
            Return _Credentials
        End Get
        Set(value As Credentials)
            _Credentials = value
        End Set
    End Property

    ''' <summary>
    ''' Local Database Connection
    ''' </summary>
    ''' <returns>Access database connection</returns>
    Public ReadOnly Property Connection As DbConnection Implements IConnectionManager.Connection
        Get
            Return _Connection
        End Get
    End Property

    ''' <summary>
    ''' SharePoint Connection
    ''' </summary>
    ''' <returns>SharePoint Connection Provider</returns>
    Public ReadOnly Property Provider As SharePointContextProvider Implements IConnectionManager.Provider
        Get
            Return _Provider
        End Get
    End Property

    ''' <summary>
    ''' Test whether we are online using Mi-Co Server interface
    ''' </summary>
    ''' <returns>True or False</returns>
    Protected Overridable Function GetOnlineStatus() As Boolean
        Return GetMiCoConnectionManager.GetOnlineStatus()
    End Function

    ''' <summary>
    ''' If we are online, use SharePointContextProvider (CSOM)
    ''' If offline, use Access Database
    ''' </summary>
    ''' <returns></returns>
    Private Function GetConnectionManager() As ConnectionManager
        Try
            If Online Then
                Return GetOnlineConnectionManager()
            Else
                Return GetOfflineConnectionManager()
            End If
        Catch ex As Exception
            ErrorLog.Write("GetConnectionManager", ex.StackTrace, ex.Message)
            Return Nothing
        End Try
    End Function

    Private Function GetOnlineConnectionManager() As ConnectionManager
        If _Online Is Nothing Then
            _online = New SharePointConnectionManager(_connection, _provider)
            _online.UserCredentials = _Credentials
        End If
        Return _Online
    End Function

    Private Function GetMiCoConnectionManager() As ConnectionManager
        If _MiCo Is Nothing Then
            _miCo = New MiCoConnectionManager(_connection, _provider, _credentials)
        End If
        Return _MiCo
    End Function

    Private Function GetOfflineConnectionManager() As ConnectionManager
        If _Offline Is Nothing Then
            _offline = New OfflineConnectionManager(_connection, _provider)
            _offline.UserCredentials = _Credentials
        End If
        Return _Offline
    End Function

    Private _credentials As Credentials
    Private ReadOnly _connection As DbConnection
    Private _offline As OfflineConnectionManager
    Private _online As SharePointConnectionManager
    Private ReadOnly _provider As SharePointContextProvider
    Private _miCo As MiCoConnectionManager
End Class