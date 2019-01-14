Imports System.Data.Common
Imports System.Data.OleDb
Imports System.Text
Imports MiCo.MiForms.Server

''' <summary>
''' OfflineConnectionManager class receives data requests and dispatches them to Access
''' </summary>
Public Class OfflineConnectionManager
    Inherits ConnectionManager
    
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connection">Database</param>
    ''' <param name="provider">SharePoint Provider</param>
    Public Sub New(connection As DbConnection, provider As SharePointContextProvider)
        MyBase.New(connection, provider)
        _localConnection = connection
    End Sub

    ''' <summary>
    ''' Get phone from local database
    ''' </summary>
    ''' <param name="user">The user</param>
    ''' <returns>Phone number</returns>
    Public Overrides Function GetPhone(user As User) As String
        Dim phon = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 Phone FROM Users WHERE firstName = @firstname and lastname = @lastname"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@firstName", user.FirstName)
            xcmd.Parameters.AddWithValue("@lastName", user.LastName)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    phon = xreader("Phone").ToString()
                End While
            End Using
            Return phon
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPhone offline", ex.StackTrace, ex.Message)
            Return phon
        End Try
    End Function

    ''' <summary>
    ''' Get form templates from local database
    ''' </summary>
    ''' <param name="user">Current user</param>
    ''' <returns>User's templates</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetFormTemplates(user As User) As FormTemplateDescription()
        Dim forms As New ArrayList()
        Try
            Dim siteNumber As String = GetSiteNumberFromUser(user)
            If Not siteNumber = "" Then
                Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
                xcmd.CommandText = GetSQLFormTemplates()
                xcmd.Parameters.AddWithValue("@Number", siteNumber)

                Dim count = 0
                Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                    While (xreader.Read)
                        count += 1
                        Dim formTemplateDescription = New FormTemplateDescription()
                        formTemplateDescription.Name = xreader("FormName").ToString()
                        formTemplateDescription.Description = xreader("FormName").ToString()
                        forms.Add(formTemplateDescription)
                    End While
                End Using

                If count > 0 Then
                    Dim formTemplates As FormTemplateDescription()
                    'Resize our dynamic array to hold our forms
                    ReDim formTemplates(count - 1)
                    For i = 0 To count - 1
                        formTemplates(i) = forms(i)
                    Next
                    Return formTemplates
                End If
            End If
            ErrorLog.Write("No offline templates", "")
            Return Nothing
        Catch ex As Exception
            ErrorLog.Write("Exception in GetFormTemplates offline", ex.StackTrace, ex.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Get 5 digit site number from local database in format SNNNN
    ''' </summary>
    ''' <param name="site">Current Site</param>
    ''' <returns>Site number string</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetSiteNumber(site As Site) As String
            Return GetSiteNumberFromDb(site.UserName)
    End Function

    ''' <summary>
    ''' Get 5 digit site number from local database in format SNNNN
    ''' </summary>
    ''' <param name="user">Current User </param>
    ''' <returns>Site number string</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Function GetSiteNumberFromUser(user As User) As String
        Return GetSiteNumberFromDb(user.UserName)
    End Function

    ''' <summary>
    ''' Get site address for site from local database
    ''' </summary>
    ''' <param name="site">Site to lookup address on</param>
    ''' <returns>Address</returns>
    Public Overrides Function GetSiteAddress(site As Site) As String
        Dim addr = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 Institution, Address1, City, State, Country, Zip FROM Sites WHERE SiteNumber = @SiteNumber"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    addr = xreader("Institution").ToString() & " " & xreader("Address1").ToString() & vbCrLf & xreader("City").ToString() & ", " &
                  xreader("State").ToString() & " " & xreader("Zip").ToString()
                End While
            End Using
            Return addr
        Catch ex As Exception
            ErrorLog.Write("Exception in GetSiteAddress offline", ex.StackTrace, ex.Message)
            Return addr
        End Try
    End Function

    ''' <summary>
    ''' Get PI For site from local database
    ''' </summary>
    ''' <param name="site">Site to lookup PI on</param>
    ''' <returns>PI</returns>
    Public Overrides Function GetPi(site As Site) As User
        Dim pi As User = Nothing
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 PIFirstName, PILastName FROM Sites WHERE SiteNumber = @SiteNumber"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    pi = New User(xreader("PILastName").ToString() & "." & xreader("PIFirstName").ToString(), xreader("PIFirstName").ToString(), xreader("PILastName").ToString(), Me)
                End While
            End Using
            Return pi
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPI offline", ex.StackTrace, ex.Message)
            Return pi
        End Try
    End Function

    ''' <summary>
    ''' Get PIFirstName For site from local database
    ''' </summary>
    ''' <param name="site">Site to lookup PI First Name on</param>
    ''' <returns>PI First Name</returns>
    Public Overrides Function GetPiFirstName(site As Site) As String
        Dim piFirstName = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 PIFirstName FROM Sites WHERE SiteNumber = @SiteNumber"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    piFirstName = xreader("PIFirstName").ToString()
                End While
            End Using
            Return piFirstName
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPIFirstName offline", ex.StackTrace, ex.Message)
            Return piFirstName
        End Try
    End Function

    ''' <summary>
    ''' Get PILastName For site from local database
    ''' </summary>
    ''' <param name="site">Site to lookup PI Last Name on</param>
    ''' <returns>PI Last Name</returns>
    Public Overrides Function GetPiLastName(site As Site) As String
        Dim piLastName = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 PILastName FROM Sites WHERE SiteNumber = @SiteNumber"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    piLastName = xreader("PILastName").ToString()
                End While
            End Using
            Return piLastName
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPILastName offline", ex.StackTrace, ex.Message)
            Return piLastName
        End Try
    End Function

    ''' <summary>
    ''' Get PIphone for site from local database 
    ''' </summary>
    ''' <param name="site">Site to lookup Phone Number on</param>
    ''' <returns>Phone Number</returns>
    Public Overrides Function GetPiPhone(site As Site) As String
        Dim piPhone = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 Phone FROM Sites WHERE SiteNumber = @SiteNumber"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    piPhone = xreader("Phone").ToString()
                End While
            End Using
            Return piPhone
        Catch ex As Exception
            ErrorLog.Write("Exception in GetPIPhone offline", ex.StackTrace, ex.Message)
            Return piPhone
        End Try
    End Function

    ''' <summary>
    ''' Get local database ID for Site 
    ''' </summary>
    ''' <param name="site">Current Site</param>
    ''' <returns>Database ID</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetSiteId(site As Site) As Integer
        Dim siteId = 0
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = GetSQLSiteId()
            xcmd.Parameters.AddWithValue("@Number", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    siteId = xreader("ID").ToString()
                End While
            End Using
            Return siteId
        Catch ex As Exception
            ErrorLog.Write("Exception: GetSiteID offline", ex.StackTrace, ex.Message)
            Return siteId
        End Try
    End Function

    ''' <summary>
    ''' Get local database ID for User 
    ''' </summary>
    ''' <param name="user">Current User</param>
    ''' <returns>Database ID</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetUserId(user As User) As Integer
        Dim userId = 0
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = GetSQLUserId()
            xcmd.Parameters.AddWithValue("@UserName", user.UserName)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    userId = xreader("ID").ToString()
                End While
            End Using
            Return userId
        Catch ex As Exception
            ErrorLog.Write("Exception: GetUserID offline", ex.StackTrace, ex.Message)
            Return userId
        End Try
    End Function

    ''' <summary>
    ''' List of subjects at site loaded from local database
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>List of subjects</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetSubjects(site As Site) As ArrayList
        Dim subjects As New ArrayList()
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = GetSQLSubjects()
            xcmd.Parameters.AddWithValue("@SiteNumber", site.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    subjects.Add(New Subject(Me, xreader("SubjectNumber").ToString(), xreader("SubjectInitials").ToString()))
                End While
            End Using
            Return subjects
        Catch ex As Exception
            ErrorLog.Write("Exception: GetSubjects Offline ", ex.StackTrace, ex.Message)
            Return subjects
        End Try
    End Function

    ''' <summary>
    ''' List of visits for subject loaded from local database
    ''' </summary>
    ''' <param name="subject">subject</param>
    ''' <returns>List of visits</returns>
    <CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")>
    Public Overrides Function GetVisits(subject As Subject) As ArrayList
        Dim visits As New ArrayList()
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = GetSQLVisits()
            xcmd.Parameters.AddWithValue("@SubjectNumber", subject.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    visits.Add(New Visit(subject, xreader("Form"), xreader("VersionNumber"), xreader("VisitDate"), xreader("VisitNumber"), xreader("Status")))
                End While
            End Using
            Return visits
        Catch ex As Exception
            ErrorLog.Write("Exception in  GetVisits", ex.StackTrace)
            Return visits
        End Try
    End Function

    ''' <summary>
    ''' Get subject local database ID
    ''' </summary>
    ''' <param name="subject">Subject</param>
    ''' <returns>Access Database Subject ID</returns>
    Public Overrides Function GetSubjectId(subject As Subject) As Integer
        Dim subjectId = 0
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT TOP 1 Subjects.ID FROM Subjects WHERE Subjects.SubjectNumber = @SubjectNumber"
            xcmd.Parameters.AddWithValue("@SubjectNumber", subject.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    subjectID = xreader("ID").ToString()
                End While
            End Using
            Return subjectID
        Catch ex As Exception
            ErrorLog.Write("Exception in  GetSubjectID", ex.StackTrace)
            Return subjectID
        End Try
    End Function

    ''' <summary>
    ''' Gets the Max subject number at site and adds 1
    ''' </summary>
    ''' <param name="site">The study site</param>
    ''' <returns>Returns an Integer less than 1000 as a string</returns>
    Public Overrides Function NextSubjectNumber(site As Site) As String
        Dim subjectId = 0
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = "SELECT max(subjectnumber) as NextID from Subjects WHERE SiteID = @SiteID"
            ' parameter(s) being passed to query
            xcmd.Parameters.AddWithValue("@SiteID", site.ID)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    'If we have no subjects, set subject number to 0
                    If IsDBNull(xreader("NextID")) Then
                        subjectID = 0
                    Else 'If we have subjects, get last 3 digits of the subject number and cast to an integer so we can increment it 
                        Dim subjectnumber As String = Right(xreader("NextID").ToString(), 3)
                        subjectID = Integer.Parse(subjectnumber.TrimStart("0"))
                    End If
                End While
            End Using
            Return (subjectID + 1).ToString()
        Catch ex As Exception
            ErrorLog.Write("Exception in NextSubjectNumber", ex.StackTrace, ex.Message)
            subjectID = 0
            Return (subjectID + 1).ToString()
        End Try
    End Function
    
     Private Function GetSiteNumberFromDb(userName As String) As String
       Dim siteNumber = ""
        Try
            Dim xcmd As OleDbCommand = _localConnection.CreateCommand()
            xcmd.CommandText = GetSQLSiteNumberFromUser()
            xcmd.Parameters.AddWithValue("@UserName", userName)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                While (xreader.Read)
                    siteNumber = xreader("SiteNumber").ToString()
                End While
            End Using
            Return siteNumber
        Catch ex As Exception
            ErrorLog.Write("Exception in GetSiteNumberFromDb offline", ex.StackTrace, ex.Message)
            Return siteNumber
        End Try
    End Function

    ''' <summary>
    ''' SQL for Visits
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLVisits() As String
        Dim sb As New StringBuilder("SELECT VisitNumber, VisitDate, VersionNumber, Form, Status FROM Visits INNER JOIN Subjects ")
        sb.Append("ON Subjects.ID = Visits.SubjectID WHERE Subjects.SubjectNumber = @SubjectNumber")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' SQL to get Site ID from Local Database
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLSiteId() As String
        Dim sb As New StringBuilder("SELECT TOP 1 Sites.ID From Sites ")
        sb.Append("WHERE Sites.SiteNumber = @Number")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' SQL for Getting User ID
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLUserId() As String
        Dim sb As New StringBuilder("SELECT TOP 1 Users.ID From Users ")
        sb.Append("WHERE Users.UserName = @UserName")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' SQL to get Subjects from Local Database
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLSubjects() As String
        Dim sb As New StringBuilder("SELECT SubjectNumber, SubjectInitials, Subjects.ID FROM Subjects INNER JOIN Sites ")
        sb.Append("ON Subjects.SiteID = Sites.ID WHERE Sites.SiteNumber = @SiteNumber")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' SQL to get sitenumber for a user from local database
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLSiteNumberFromUser() As String
        Dim sb As New StringBuilder("SELECT TOP 1 Sites.SiteNumber ")
        sb.Append("FROM Sites INNER JOIN (Users INNER JOIN SiteUsers On Users.[ID] = SiteUsers.[UserID]) On Sites.[ID] = SiteUsers.[SiteID] ")
        sb.Append("WHERE Users.UserName = @UserName")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' SQL to get Form Templates from Local Database
    ''' </summary>
    ''' <returns>SQL Statement</returns>
    Private Function GetSQLFormTemplates() As String
        Dim sb As New StringBuilder("SELECT Templates.FormName ")
        sb.Append("FROM Sites INNER JOIN (Templates INNER JOIN SiteTemplates On Templates.[ID] = SiteTemplates.[TemplateID]) On Sites.[ID] = SiteTemplates.[SiteID] ")
        sb.Append("WHERE Sites.SiteNumber = @Number")
        Return sb.ToString()
    End Function
    
    Private ReadOnly _localConnection As DbConnection
End Class
