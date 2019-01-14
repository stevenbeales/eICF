Imports System.Data.Common
Imports System.Data.OleDb
Imports MiCo.MiForms.Server
Imports Microsoft.SharePoint.Client

''' <summary>
''' Class responsible for saving data into local database
''' </summary>
Public Class LocalDatabase

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connection">Database connection</param>
    Public Sub New(connection As DbConnection)
        _connection = connection
    End Sub

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Save visit to Local Database
    ''' </summary>
    ''' <param name="visit">Visit</param>
    ''' <returns>true if visit saved</returns>
    Public Function SaveVisitInDatabase(visit As Visit) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Select form, subjectid, visitdate from visits where form = @form and subjectid = @subjectid and visitdate = @visitdate"
            xcmd.Parameters.AddWithValue("@form", visit.Form)
            xcmd.Parameters.AddWithValue("@subjectid", visit.Subject.ID)
            xcmd.Parameters.AddWithValue("@visitdate", visit.VisitDate)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertVisitInDatabase(visit)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveVisitInDatabase " & visit.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save user to Local Database
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>true if user saved</returns>
    Public Function SaveUserInDatabase(user As User) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Select username from Users where username = @username"
            xcmd.Parameters.AddWithValue("@Username", user.UserName)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertUserInDatabase(user)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveUserInDatabase " & user.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save Site User Into Local Database
    ''' </summary>
    ''' <param name="site">site</param>
    ''' <param name="user">user</param>
    ''' <returns>true or false</returns>
    Public Function SaveSiteUserInDatabase(site As Site, user As User) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Select userid from SiteUsers where userid = @userid"
            xcmd.Parameters.AddWithValue("@Userid", user.ID)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertSiteUserInDatabase(site, user)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveSiteUserInDatabase " & user.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    Public Function SaveSiteFormsInDatabase(site As Site, user As User) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Select siteid from Site where siteid = @siteid"
            xcmd.Parameters.AddWithValue("@Siteid", site.ID)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertSiteFormsInDatabase(site, user)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveSiteUserInDatabase " & user.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save Subject to Local Database 
    ''' </summary>
    ''' <param name="subject">subject</param>
    ''' <param name="siteId">site id</param>
    ''' <returns>true or false</returns>
    Public Function SaveSubjectInDatabase(subject As Subject, siteId As Integer) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Select subjectnumber from Subjects where subjectnumber = @Number"
            xcmd.Parameters.AddWithValue("@Number", subject.Number)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertSubjectInDatabase(subject, siteId)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveSubjectInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save Site to Local Database
    ''' </summary>
    ''' <returns>true or false</returns>
    Public Function SaveSiteInDatabase(siteNumber As String, piFirstName as string, piLastName As string) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Select sitenumber from Sites where sitenumber = @Number"
            xcmd.Parameters.AddWithValue("@Number", siteNumber)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertSiteInDatabase(siteNumber, piFirstName, piLastName)
                Else
                    Return UpdateSiteInDatabase(siteNumber, piFirstName, piLastName)
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveSiteInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save updated Site Address to Local Database 
    ''' </summary>
    ''' <param name="site">Local Database Site</param>
    ''' <param name="selectedSite">SharePoint Study Site </param>
    ''' <returns>True or false</returns>
    Public Function SaveSiteAddressToDatabase(site As Site, selectedSite As ListItem) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Update Sites set Institution=@institution, Address1=@Address1, City=@City, State=@State, Zip=@Zip, Phone=@Phone where SiteNumber = @Number"
            xcmd.Parameters.AddWithValue("@Number", site.Number)
            xcmd.Parameters.AddWithValue("@Institution", selectedSite("Institution"))
            xcmd.Parameters.AddWithValue("@Address1", selectedSite("Address1"))
            xcmd.Parameters.AddWithValue("@City", selectedSite("City"))
            xcmd.Parameters.AddWithValue("@State", selectedSite("State").LookupValue)
            xcmd.Parameters.AddWithValue("@Zip", selectedSite("Zip"))
            xcmd.Parameters.AddWithValue("@Phone", selectedSite("Phone"))
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveSiteAddressInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Update Site PI details in local database
    ''' </summary>
    ''' <param name="siteNumber">Site Number</param>
    ''' <param name="piFirstName">Pi</param>
    ''' <param name="piLastName">Pi</param>
    ''' <returns>true or false</returns>
    ''' Note: we can't pass in Site object because called site.property will cause an infinite loop
    Private Function UpdateSiteInDatabase(siteNumber As String, piFirstName As String, piLastName As String) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Update Sites set PIFirstName = @PIFirstName, PILastName = @PiLastName Where SiteNumber = @SiteNumber"
            xcmd.Parameters.AddWithValue("@PIFirstName", piFirstName)
            xcmd.Parameters.AddWithValue("@PILastName", piLastName)
            xcmd.Parameters.AddWithValue("@SiteNumber", siteNumber)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in UpdateSiteInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save new Site to Local Database
    ''' </summary>
    ''' <returns>True if site saved</returns>
    Private Function InsertSiteInDatabase(siteNumber As String, piFirstName As String, piLastName As String) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into Sites(SiteNumber, PIFirstName, PILastName) values (@SiteNumber, @PIFirstName, @PiLastName)"
            xcmd.Parameters.AddWithValue("@SiteNumber", siteNumber)
            xcmd.Parameters.AddWithValue("@PIFirstName", piFirstName)
            xcmd.Parameters.AddWithValue("@PILastName", piLastName)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertSiteInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    '''  Insert new user/site association to Local Database
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>True if user saved</returns>
    Private Function InsertSiteUserInDatabase(site As Site, user As User) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into SiteUsers(SiteID, UserID) values (@SiteID, @UserID)"
            xcmd.Parameters.AddWithValue("@SiteID", site.ID)
            xcmd.Parameters.AddWithValue("@UserID", user.ID)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertSiteUserInDatabase for site " & site.ID & " user " & user.ID, ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

     Private Function SaveTemplateInDatabase(template As FormTemplateDescription) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Select FormName from Templates where formName = @formName"
            xcmd.Parameters.AddWithValue("@formName", template.name)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If Not xreader.HasRows() Then
                    Return InsertTemplateInDatabase(template)
                Else
                    Return True
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in SaveTemplateInDatabase " & template.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
     End Function

      Private Function InsertTemplateInDatabase(template As FormTemplateDescription) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into Templates(FormName) from Templates where formName = @formName"
            xcmd.Parameters.AddWithValue("@formName", template.name)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertTemplateInDatabase " & template.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
     End Function
    
    Private Function InsertTemplatesInDatabase(user As User, templates As ArrayList) As Boolean
        dim formTemplate as FormTemplateDescription
        Try  
            for i = 0 To templates.Count - 1 
                formTemplate = templates(i)
                SaveTemplateInDatabase(formTemplate)
            Next
            Return True
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertTemplatesInDatabase for user " & user.ID, ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    Private function GetTemplateId(templateName As string) As integer
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Select FormName from Templates where formName = @formName"
            xcmd.Parameters.AddWithValue("@formName", templateName)
            Using xreader As OleDbDataReader = xcmd.ExecuteReader()
                If xreader.HasRows() Then
                    Return xreader("ID")
                Else
                    Return -1
                End If
            End Using
        Catch ex As Exception
            ErrorLog.Write("Exception in GetTemplateID " & templateName.ToString(), ex.StackTrace, ex.Message)
            Return False
        End Try
    End function

    Private Function InsertSiteFormsInDatabase(site As Site, user As User) As Boolean
        dim templates as ArrayList = user.FormTemplates
        dim template as FormTemplateDescription
        Try
            InsertTemplatesInDatabase(user, templates)
            Dim xcmd As OleDbCommand = _Connection.CreateCommand
            For i = 0 To templates.Count - 1 
                template = templates(i)
                xcmd.CommandText = "Insert into SiteTemplates(SiteID, TemplateID) values (@SiteID, @TemplateID)"
                xcmd.Parameters.AddWithValue("@SiteID", site.ID)
                xcmd.Parameters.AddWithValue("@TemplateID", GetTemplateID(template.Name))
                xcmd.ExecuteNonQuery()
            Next
            Return True
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertSiteFormsInDatabase for site " & site.ID & " user " & user.ID, ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    '''  Insert new user to Local Database
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>True if user saved</returns>
    Private Function InsertUserInDatabase(user As User) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into Users(UserName, UserFirstName, UserLastName) values (@UserName, @UserFirstName, @UserLastName)"
            xcmd.Parameters.AddWithValue("@Username", user.UserName)
            xcmd.Parameters.AddWithValue("@UserFirstName", user.FirstName)
            xcmd.Parameters.AddWithValue("@UserLastName", user.LastName)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertUserInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save new subject to Local Database 
    ''' </summary>
    ''' <param name="subject"></param>
    ''' <param name="SiteID"></param>
    ''' <returns></returns>
    Private Function InsertSubjectInDatabase(subject As Subject, siteId As Integer) As Boolean
        Dim xcmd As OleDbCommand = _connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into Subjects(SubjectNumber, SubjectInitials, SiteID, SubjectLanguage) values (@Number, @Initials, @SiteId, @Language)"
            xcmd.Parameters.AddWithValue("@Number", subject.Number)
            xcmd.Parameters.AddWithValue("@Initials", subject.Initials)
            xcmd.Parameters.AddWithValue("@SiteId", siteId)
            xcmd.Parameters.AddWithValue("@Language", subject.Language)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertSubjectInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Save new visit to Local Database
    ''' </summary>
    ''' <param name="visit">Visit</param>
    ''' <returns>True if visit inserted</returns>
    Private Function InsertVisitInDatabase(visit As Visit) As Boolean
        Dim xcmd As OleDbCommand = _Connection.CreateCommand()
        Try
            xcmd.CommandText = "Insert into Visits(Form, VersionNumber, SubjectID, VisitDate, VisitNumber, Status) values (@Form, @VersionNumber,  @SubjectID, @VisitDate, @VisitNumber, @Status)"
            xcmd.Parameters.AddWithValue("@Form", visit.Form)
            xcmd.Parameters.AddWithValue("@VersionNumber", visit.VersionNumber)
            xcmd.Parameters.AddWithValue("@SubjectID", visit.Subject.ID)
            xcmd.Parameters.AddWithValue("@VisitDate", visit.VisitDate)
            xcmd.Parameters.AddWithValue("@VisitNumber", visit.VisitNumber)
            xcmd.Parameters.AddWithValue("@Status", visit.Status)
            Dim rowCount As Integer = xcmd.ExecuteNonQuery()
            Return rowCount > 0
        Catch ex As Exception
            ErrorLog.Write("Exception in InsertVisitInDatabase", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    Private ReadOnly _connection As DbConnection
End Class