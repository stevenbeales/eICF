Imports System.Data.Common

''' <summary>
'''Visit class holds Subject Visit details
''' </summary>
Public Class Visit

    ''' <summary>
    ''' Visit constructor
    ''' </summary>
    ''' <param name="subject">subject</param>
    ''' <param name="form">Mi-Co form</param>
    ''' <param name="versionNumber">Public form version, not Mi-Co version</param>
    ''' <param name="visitDate">Date of subject visit</param>
    ''' <param name="visitNumber">2 digit Visit Number</param>
    Public Sub New(subject As Subject, form As String, versionNumber As String, visitDate As Date, visitNumber As String, status As String)
        _Form = form
        _VersionNumber = versionNumber
        _VisitDate = visitDate
        _Subject = subject
        _VisitNumber = visitNumber
        _Status = status
    End Sub

    ''' <summary>
    '''	Save visit into local database
    ''' Visit will be added to remote database after session is submitted by a server side service
    ''' </summary>
    Public Function InsertInDatabase(connection As DbConnection) As Boolean
        Try
            Dim localDb As New LocalDatabase(connection)
            Return localDb.SaveVisitInDatabase(Me)
        Catch ex As Exception
            ErrorLog.Write("Exception in Visit InsertInDatabase", ex.StackTrace)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' eICF Form Template used in subject visit
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Form As String

    Public ReadOnly Property Status As String
    Public ReadOnly Property VersionNumber As String

    Public ReadOnly Property VisitNumber As String
    Public ReadOnly Property VisitDate As Date
    Public ReadOnly Property Subject As Subject
End Class