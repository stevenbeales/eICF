Imports System.Data.Common
Imports Microsoft.SharePoint.Client

''' <summary>
''' SharePointConnectionManager class receives data requests and dispatches them to SharePoint.
''' </summary>
Public Class SharePointConnectionManager
    Inherits ConnectionManager

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connection">Database Connection</param>
    ''' <param name="provider">SharePoint Provider</param>
    Public Sub New(connection As DbConnection, provider As SharePointContextProvider)
        MyBase.New(connection, provider)
        LoadLists()
    End Sub

    ''' <summary>
    ''' Get user's phone from Clinical Trial Portal
    ''' </summary>
    ''' <param name="user">user name</param>
    ''' <returns>phone number</returns>
    Public Overrides Function GetPhone(user As User) As String
        Try
            Dim query As New CamlQuery()
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                            "<Query>" &
                                "<Where>" &
                                    "<And>" &
                                        "<Eq>" &
                                            "<FieldRef Name='LastName' />" &
                                                "<Value Type='Text'>" &
                                                    user.LastName &
                                                "</Value>" &
                                        "</Eq>" &
                                        "<Eq>" &
                                            "<FieldRef Name='FirstName' />" &
                                                "<Value Type='Text'>" &
                                                    user.FirstName &
                                                "</Value>" &
                                        "</Eq>" &
                                    "</And>" &
                                "</Where>" &
                            "</Query>" &
                            "<ViewFields>" &
                                "<FieldRef Name='Phone' />" &
                            "</ViewFields>" &
                        "</View>"

            Dim items As ListItemCollection = _siteUserList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedUser As ListItem = items(0)
                Dim phone As Object = selectedUser("Phone")
                If phone Is Nothing Then
                    ErrorLog.Write("Selected User Phone not found", "Online GetPhone")
                    Return ""
                End If
                ErrorLog.Write("Phone Number found", selectedUser("Phone").ToString())
                Return selectedUser("Phone").ToString()
            Else
                ErrorLog.Write("Phone Number not found", "GetPhone")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("User not found", "GetPhone Online " & ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetPhone(user)
        End Try
    End Function

    ''' <summary>
    ''' Gets 5 digit site number from clinical trial portal
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Site Number</returns>
    Public Overrides Function GetSiteNumber(site As Site) As String
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<Eq>" &
                                    "<FieldRef Name='Login_Name' />" &
                                        "<Value Type='Calculated'>" &
                                            site.UserName &
                                        "</Value>" &
                                "</Eq>" &
                            "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='SiteId' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteUserList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedUser As ListItem = items(0)
                If selectedUser("SiteId") Is Nothing Then
                    ErrorLog.Write("Site Number not found for " & site.UserName, "GetSiteNumber online")
                    Return ""
                End If
                Dim siteNumber As String = selectedUser("SiteId").LookupValue
                Dim localDb As New LocalDatabase(Connection)
                localDb.SaveSiteInDatabase(siteNumber, "", "")
                Return siteNumber
            Else
                ErrorLog.Write("User not found: " & site.UserName, "GetSiteNumber online")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetSiteNumber online", ex.StackTrace, ex.Message)
            Dim mico As New MiCoConnectionManager(Connection, Provider, UserCredentials)
            Return mico.GetSiteNumber(site)
        End Try
    End Function

    
    ''' <summary>
    ''' Gets PI First Name from clinical trial portal
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>First Name</returns>
    Public Overrides Function GetPIFirstName(site As Site) As String
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<Eq>" &
                                    "<FieldRef Name='SiteId' />" &
                                        "<Value Type='Text'>" &
                                            site.Number &
                                        "</Value>" &
                                "</Eq>" &
                            "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='PIFirstName' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedSite As ListItem = items(0)
                If selectedSite("PIFirstName") Is Nothing Then
                    ErrorLog.Write("PIFirstName not found for " & site.Number, "GetPIFirstName online")
                    Return ""
                End If
                Dim piName As String = selectedSite("PIFirstName").ToString()
                Return piName
            Else
                ErrorLog.Write("Site not found: " & site.Number, "PIFirstName online")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPIFirstName online", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetPIFirstName(site)
        End Try
    End Function

    
    ''' <summary>
    ''' Gets PI Last Name from clinical trial portal
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>PI Last Name</returns>
    Public Overrides Function GetPILastName(site As Site) As String
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<Eq>" &
                                    "<FieldRef Name='SiteId' />" &
                                        "<Value Type='Text'>" &
                                            site.Number &
                                        "</Value>" &
                                "</Eq>" &
                            "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='PILastName' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedSite As ListItem = items(0)
                If selectedSite("PILastName") Is Nothing Then
                    ErrorLog.Write("PILastName not found for " & site.Number, "GetPILastName online")
                    Return ""
                End If
                Dim piName As String = selectedSite("PILastName").ToString()
                Return piName
            Else
                ErrorLog.Write("Site not found: " & site.Number, "PILastName online")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPILastName online", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetPILastName(site)
        End Try
    End Function

    ''' <summary>
    ''' Gets PI Phone from clinical trial portal 
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Phone Number</returns>
    Public Overrides Function GetPIPhone(site As Site) As String
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<Eq>" &
                                    "<FieldRef Name='SiteId' />" &
                                        "<Value Type='Text'>" &
                                            site.Number &
                                        "</Value>" &
                                "</Eq>" &
                            "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='Phone' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedSite As ListItem = items(0)
                If selectedSite("Phone") Is Nothing Then
                    ErrorLog.Write("Phone not found for " & site.Number, "GetPhone online")
                    Return ""
                End If
                Return selectedSite("Phone").ToString()
            Else
                ErrorLog.Write("Site not found: " & site.Number, "PIFirstName online")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPIPhone online", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetPIPhone(site)
        End Try
    End Function


    ''' <summary>
    ''' Gets site address from clinical trial portal
    ''' </summary>
    ''' <param name="site">clinical site</param>
    ''' <returns>2 line address </returns>
    Public Overrides Function GetSiteAddress(site As Site) As String
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<Eq>" &
                                    "<FieldRef Name='SiteId' />" &
                                    "<Value Type='Text'>" &
                                        site.Number &
                                    "</Value>" &
                                "</Eq>" &
                            "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='Institution' /> <FieldRef Name='Address1' /> <FieldRef Name='City' /> <FieldRef Name='State' /> <FieldRef Name='Zip' /> <FieldRef Name='Phone' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedSite As ListItem = items(0)
                Dim localDb As New LocalDatabase(Connection)
                localDb.SaveSiteAddressToDatabase(site, selectedSite)
                Return selectedSite("Institution") & " " & selectedSite("Address1") & vbCrLf & selectedSite("City") & ", " & selectedSite("State").LookupValue & " " & selectedSite("Zip")
            Else
                ErrorLog.Write("Site Address not found", "GetSiteAddress")
                Return ""
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in GetSiteAddress online", ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetSiteAddress(site)
        End Try
    End Function

    ''' <summary>
    ''' Gets PI for Site from Clinical Trial Portal
    ''' </summary>
    ''' <param name="site">Clinical Site</param>
    ''' <returns>PI User at Site</returns>
    Public Overrides Function GetPI(site As Site) As User
        Dim query As New CamlQuery()
        Try
            query.ViewXml = "<View Scope='RecursiveAll'>" &
                        "<Query>" &
                            "<Where>" &
                                "<And>" &
                                    "<Eq>" &
                                        "<FieldRef Name='SiteId' />" &
                                            "<Value Type='Text'>" &
                                                site.Number &
                                            "</Value>" &
                                        "</Eq>" &
                                        "<Eq>" &
                                            "<FieldRef Name='InstitutionRole' />" &
                                                "<Value Type='Lookup'>" &
                                                    "PI" &
                                                "</Value>" &
                                        "</Eq>" &
                                    "</And>" &
                                "</Where>" &
                        "</Query>" &
                        "<ViewFields>" &
                            "<FieldRef Name='LoginName' />" &
                            "<FieldRef Name='FirstName' />" &
                            "<FieldRef Name='LastName' />" &
                        "</ViewFields>" &
                    "</View>"

            Dim items As ListItemCollection = _siteUserList.GetItems(query)
            _context.Load(items)
            _context.ExecuteQuery()
            If items.Count > 0 Then
                Dim selectedUser As ListItem = items(0)
                Return New User(selectedUser("LoginName"), selectedUser("FirstName"), selectedUser("LastName"), Me)
            Else
                ErrorLog.Write("PI not found.", "GetPI")
                Return Nothing
            End If
        Catch ex As Exception
            ErrorLog.Write("PI not found", "GetPI online " & ex.StackTrace, ex.Message)
            Dim offline As New OfflineConnectionManager(Connection, Provider)
            Return offline.GetPI(site)
        End Try
    End Function

    ''' <summary>
    ''' Load site and site users lists from Clinical Trial Portal 
    ''' </summary>
    Private Sub LoadLists()
        Try
            _context = Provider.Value
            _siteList = _context.Web.Lists.GetByTitle("UMAStudySiteProfile")
            _siteUserList = _context.Web.Lists.GetByTitle("UMAStudySiteUser")
        Catch ex As Exception
            ErrorLog.Write("LoadLists", "LoadLists online " & ex.StackTrace, ex.Message)
        End Try
    End Sub

    Private _siteList As List
    Private _siteUserList As List
    Private _context As ClientContext
End Class
