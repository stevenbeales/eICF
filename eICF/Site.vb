''' <summary>
''' Site class holds current site details
''' We retrieve current site from logged in user
''' </summary>
Public Class Site

    ''' <summary>
    '''  We pass in the logged in user to calculate the current site
    '''  We pass in a connectionManager object that let's us lookup data online or offline
    ''' </summary>
    ''' <param name="userName">User name</param>
    ''' <param name="connectionManager">Connection Manager</param>
    Public Sub New(userName As String, connectionManager As ConnectionManager)
        _userName = userName
        _connectionManager = connectionManager
    End Sub

    ''' <summary>
    ''' Looks up next site number in local database
    ''' </summary>
    ''' <returns>Returns next 3 character subject number for site e.g. 00N</returns>
    Public Function NextSubjectNumber() As String
        Dim nextNumber As String = _connectionManager.NextSubjectNumber(Me)
        Return Utils.ZeroFill(nextNumber, SUBJECT_NUMBER_LENGTH)
    End Function

    ''' <summary>
    ''' Does subject aleady exist with this Subject Number?
    ''' </summary>
    ''' <param name="anumber">Subject Number</param>
    ''' <returns>True or False</returns>
    Public Function SubjectExists(anumber As String) As Boolean
        Try
            For Each subject As Subject In Subjects
                If subject.Number = anumber Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            ErrorLog.Write("Exception in Subject Exists", ex.StackTrace, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Subjects at the site.
    ''' </summary>
    ''' <returns>List of Subjects</returns>
    Public ReadOnly Property Subjects As ArrayList
        Get
            If _Subjects Is Nothing Then
                _Subjects = GetSubjects()
            End If
            Return _Subjects
        End Get
    End Property

    ''' <summary>
    ''' Site Number/Site Id in SharePoint List
    ''' </summary>
    ''' <returns>5 digit string 5NNNN</returns>
    Public Property Number As String
        Get
            If _Number = "" Then
                _Number = GetSiteNumber()
            End If
            Return _Number
        End Get
        Set
            _number = Value
        End Set
    End Property

    'Site ID in database/Site ItemID in List.
    Public ReadOnly Property Id As Integer
        Get
            If _ID = 0 Then
                _ID = GetSiteID()
            End If
            Return _ID
        End Get
    End Property

    'Primary Investigator
    Public ReadOnly Property Pi As User
        Get
            If _PI Is Nothing Then
                _PI = GetPI()
            End If
            Return _PI
        End Get
    End Property

    'Site Address
    Public ReadOnly Property Address As String
        Get
            If _Address = "" Then
                _Address = GetAddress()
            End If
            Return _Address
        End Get
    End Property

    'Phone
    Public ReadOnly Property Phone As String
        Get
            If _Phone = "" Then
                _Phone = GetPhone()
            End If
            Return _Phone
        End Get
    End Property

    'PI First Name
    Public Property PiFirstName As String
        Get
            If _pIFirstName = "" Then
                _pIFirstName = GetPIFirstName()
            End If
            Return _pIFirstName
        End Get
        Set
            _pIFirstName = Value
        End Set
    End Property

    'PI Last Name
    Public Property PiLastName As String
        Get
            If _pILastName = "" Then
                _pILastName = GetPILastName()
            End If
            Return _pILastName
        End Get
        Set
            _pILastName = Value
        End Set
    End Property

    'PI Full Name
    Public ReadOnly Property PiFullName As String
        Get
            Return PiFirstName & " " & PiLastName
        End Get
    End Property

    'Logged in User at the Site
    Public ReadOnly Property UserName As String

    ''' <summary>
    ''' Insert Site in Database
    ''' </summary>
    ''' <returns>True or False</returns>
    Public Function InsertInDatabase() As Boolean
        Dim localDb As New LocalDatabase(_connectionManager.Connection)
        Try
            If Number.Trim().Length = SITE_NUMBER_LENGTH Then
            Return localDb.SaveSiteInDatabase(_number, _pIFirstName,_piLastName)
        Else
                Return False
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in Site InsertInDatabase", ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Associate user with Site
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>True or false</returns>
    Public Function InsertSiteUserInDatabase(user As User) As Boolean
        Dim localDb As New LocalDatabase(_ConnectionManager.Connection)
        Try
            Return localDb.SaveSiteUserInDatabase(Me, user)
        Catch ex As Exception
            ErrorLog.Write("Exception in Site User InsertInDatabase", ex.StackTrace)
            Return False
        End Try
    End Function

     Public Function InsertSiteFormsInDatabase(user As User) As Boolean
        Dim localDb As New LocalDatabase(_ConnectionManager.Connection)
        Try
            Return localDb.SaveSiteFormsInDatabase(Me, user)
        Catch ex As Exception
            ErrorLog.Write("Exception in Site User InsertInDatabase", ex.StackTrace)
            Return False
        End Try
    End Function

    'Gets the PI associated to the site. 
    Private Function GetPi() As User
        Return _ConnectionManager.GetPI(Me)
    End Function

    'Gets the address associated to the site. 
    Private Function GetAddress() As String
        Return _ConnectionManager.GetSiteAddress(Me)
    End Function

    'Gets the phone associated to the site. 
    Private Function GetPhone() As String
        Return _ConnectionManager.GetPIPhone(Me)
    End Function

    'Gets the PI name associated to the site. 
    Private Function GetPiFirstName() As String
        Return _connectionManager.GetPIFirstName(Me)
    End Function

    'Gets the PI name associated to the site. 
    Private Function GetPiLastName() As String
        Return _connectionManager.GetPILastName(Me)
    End Function

    'Return the subjects at the site.
    Private Function GetSubjects() As ArrayList
        Return _connectionManager.GetSubjects(Me)
    End Function

    ''' <summary>
    ''' Return the user's site number associated to the logged in user
    ''' </summary>
    ''' <returns>5 digit subject number in SNNNN format</returns>
    Private Function GetSiteNumber() As String
        Return _connectionManager.GetSiteNumber(Me)
    End Function

    ''' <summary>
    ''' Get the user's site id associated to the logged in user.
    ''' </summary>
    ''' <returns>Returns local database site ID</returns>
    Private Function GetSiteId() As Integer
        Return _connectionManager.GetSiteID(Me)
    End Function

    Private _address As String = ""
    Private _number As String = ""
    Private _phone As String = ""
    Private _pIFirstName As String = ""
    Private _pILastName As String = ""
    Private ReadOnly _connectionManager As ConnectionManager
    Private _pi As User
    Private _subjects As ArrayList
    Private _id As Integer = 0
End Class