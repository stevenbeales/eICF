''' <summary>
''' Subject class holds Study Subject details
''' </summary>
Public Class Subject

    ''' <summary>
    ''' Constructor takes the local database connection, the subject number and subject initials 
    ''' </summary>
    ''' <param name="connectionManager">Connection manager</param>
    ''' <param name="number">Subject number</param>
    ''' <param name="initials">Subject initials</param>
    Public Sub New(connectionManager As ConnectionManager, number As String, initials As String)
        _connectionManager = connectionManager
        If number.Trim().Length = SUBJECT_NUMBER_LENGTH + SITE_NUMBER_LENGTH Then
            _Number = number
        Else
            ErrorLog.Write("Subject Number is not valid", number)
            Throw New ConstraintException("Subject Number is not valid")
        End If
        _Initials = initials
    End Sub

    ''' <summary>
    ''' Is Form in existing Visits
    ''' </summary>
    ''' <param name="name">Form name</param>
    ''' <returns>true or false</returns>
    Public Function IsFormInVisits(name As String) As Boolean
        For Each visit As Visit In Visits
            If visit.Form = name Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Does visit aleady exist with this Visit Number?
    ''' </summary>
    ''' <param name="anumber">Visit number</param>
    ''' <returns>True or false</returns>
    Public Function VisitExists(anumber As String) As Boolean
        Try
            If Not anumber = "" Then
                For Each visit As Visit In Visits
                    If visit.VisitNumber = anumber Then
                        Return True
                    End If
                Next
            End If
            Return False
        Catch ex As Exception
            ErrorLog.Write("Exception in  VisitExists", ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Does a later visit aleady exist For this Subject?
    ''' </summary>
    ''' <param name="anumber">Visit number</param>
    ''' <returns>True or false</returns>
    Public Function LaterVisitExists(anumber As String) As Boolean
        Try
            If Not anumber = "" Then
                For Each visit As Visit In Visits
                    If Not visit.VisitNumber = "" Then
                        If Convert.ToInt32(visit.VisitNumber.Trim()) > Convert.ToInt32(anumber.Trim()) Then
                            Return True
                        End If
                    End If
                Next
            End If
            Return False
        Catch ex As Exception
            ErrorLog.Write("Exception in  LaterVisitExists", ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    '''     Save subject into local database
    '''     Subject will be added to remote database after session is submitted by a server side service
    ''' </summary>-
    ''' <param name="SiteID">Database site ID</param>
    ''' <returns>Returns true if Subject is inserted successfully</returns>
    Public Function InsertInDatabase(siteId As Integer) As Boolean
        Dim localDb As New LocalDatabase(_connectionManager.Connection)
        Try
            If _Number.Trim().Length = SITE_NUMBER_LENGTH + SUBJECT_NUMBER_LENGTH Then
                Return localDb.SaveSubjectInDatabase(Me, siteId)
            Else
                Return False
            End If
        Catch ex As Exception
            ErrorLog.Write("Exception in Subject InsertInDatabase", ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Subject Initials in 3 letter format. A_A format if not middle initial
    ''' </summary>
    ''' <returns>Initials</returns>
    Public Property Initials As String
        Public ReadOnly Property Language As String
            Get
                Return _language
            End Get
        End Property
    Public ReadOnly Property Number As String

    ''' <summary>
    ''' Subject visits
    ''' </summary>
    ''' <returns>List of visits</returns>
    Public ReadOnly Property Visits() As ArrayList
        Get
            If _visits Is Nothing Then
                _visits = GetVisits()
            End If
            Return _visits
        End Get
    End Property

    ''' <summary>
    ''' Subject database ID
    ''' </summary>
    ''' <returns>ID</returns>
    Public ReadOnly Property Id As Integer
        Get
            If _id = 0 Then
                _id = GetSubjectId()
            End If
            Return _id
        End Get
    End Property

    ''' <summary>
    ''' Get subjects visits 
    ''' </summary>
    ''' <returns>Return the subject's visits.</returns>
    Private Function GetVisits() As ArrayList
        Return _connectionManager.GetVisits(Me)
    End Function

    ''' <summary>
    ''' Get the subject id associated to the subject.
    ''' </summary>
    ''' <returns>Returns local database ID</returns>
    Private Function GetSubjectId() As Integer
        Return _connectionManager.GetSubjectId(Me)
    End Function

    Private ReadOnly _connectionManager As ConnectionManager
    Private _visits As ArrayList
    Private _id As Integer
    Private _language As String = "en"
End Class