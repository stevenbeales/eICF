Imports MiCo.MiForms.Server

''' <summary>
'''User class holds PI and Consent Administrator details
''' </summary>
Public Class User

    ''' <summary>
    ''' User's firstname and lastname and a connection manager for offline/online access
    ''' </summary>
    ''' <param name="userName">Username</param>
    ''' <param name="firstName">First name</param>
    ''' <param name="lastName">Last name</param>
    ''' <param name="connectionManager">Connection Manager</param>
    Public Sub New(userName As String, firstName As String, lastName As String, connectionManager As ConnectionManager)
        _userName = userName
        _firstName = firstName
        _lastName = lastName
        _connectionManager = connectionManager
    End Sub

    ''' <summary>
    ''' Default string representation
    ''' </summary>
    ''' <returns>Username</returns>
    Public Overrides Function ToString() As String
        Return _userName
    End Function

    ''' <summary>
    ''' Local Database Id
    ''' </summary>
    ''' <returns>Id</returns>
    Public ReadOnly Property Id As Integer
        Get
            If _id = 0 Then
                _id = GetUserId()
            End If
            Return _id
        End Get
    End Property

    Public ReadOnly Property FirstName As String
        Get
            Return _firstName
        End Get
    End Property

    Public ReadOnly Property LastName As String
        Get
            Return _lastName
        End Get
    End Property

    Public ReadOnly Property FullName As String
        Get
            Return _firstName & " " & _lastName
        End Get
    End Property

    Public ReadOnly Property Phone As String
        Get
            If _phone = "" Then
                _phone = GetPhone()
            End If
            Return _phone
        End Get
    End Property

    Public ReadOnly Property UserName As String
        Get
            Return _userName
        End Get
    End Property

    ''' <summary>
    ''' gets the forms associated to the user's MiCo account except Visit Pages
    ''' </summary>
    ''' <returns>List of forms</returns>
    Public ReadOnly Property FormTemplates() As ArrayList
        Get
            'If we have already built list of templates, return from cache 
            If _filteredTemplates.Count > 0 Then
                Return _filteredTemplates
            End If

            'Get the list of templates from MiCo server
            Dim templates As FormTemplateDescription() = GetFormTemplates()
            Try
                For i = 0 To templates.Length - 1

                    'Filter our visit pages template and any non-ICF templates 
                    If EXCLUDED_FORMS_FROM_DOWNLOAD.IndexOf(templates(i).FormID) < 0 Then
                        _filteredTemplates.Add(templates(i))
                    End If
                Next
                Return _FilteredTemplates
            Catch ex As Exception
                ErrorLog.Write("Exception in FormTemplates", ex.StackTrace, ex.Message)
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Gets the forms associated to the user's MiCo account
    ''' </summary>
    ''' <returns>List of Form Template Descriptions</returns>
    Private Function GetFormTemplates() As FormTemplateDescription()
        Return _ConnectionManager.GetFormTemplates(Me)
    End Function

    ''' <summary>
    ''' Gets the Phone associated to the user. 
    ''' </summary>
    ''' <returns>Phone number</returns>
    Private Function GetPhone() As String
        Return _ConnectionManager.GetPhone(Me)
    End Function

    Private Function GetUserId() As Integer
        Return _connectionManager.GetUserId(Me)
    End Function

    Private ReadOnly _userName As String = ""
    Private ReadOnly _firstName As String = ""
    Private ReadOnly _lastName As String = ""
    Private _phone As String = ""
    Private ReadOnly _connectionManager As ConnectionManager
    Private ReadOnly _filteredTemplates As New ArrayList()
    Private _id As Integer = 0
End Class

