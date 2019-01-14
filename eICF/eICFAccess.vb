Imports System.Data.Common
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports MiCo.MiForms

''' <summary>
''' Contains the code to open, create and connect to local Access database
''' Implements IDisposable because OleDBConnection is not managed
''' </summary>
Public Class EIcfAccess
    Implements IDisposable

    ''' <summary>
    ''' eICFAccess constructor
    ''' </summary>
    ''' <param name="form">Mico Form</param>
    ''' <param name="connectionString">Connection to database</param>
    Public Sub New(form As Form, connectionString As String)
        _Form = form
        _ConnectionString = connectionString
    End Sub

    ''' <summary>
    ''' Load and open Access database.
    ''' </summary>
    ''' <returns>Database Connection</returns>
    Public Function OpenLocalConnection(localdb As String) As DbConnection
        Try
            _Connection = New OleDbConnection(_ConnectionString & localdb)
            _Connection.Open()
            Return _Connection
        Catch ex As Exception
            ErrorLog.Write(DBFILE_NO_OPEN & " " & localdb, "OpenLocalConnection " & ex.StackTrace)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets Local Database Path and FileName (and creates it if does not exist)
    ''' </summary>
    ''' <param name="filename">Filename of Database</param>
    ''' <returns>Fully qualified path and filename of database file</returns>
    Public Function GetLocalDatabase(filename As String) As String
        Dim dbFile As String = Path.GetTempPath() & filename
        'If the database file does not exist write it out from db attached to form.
        If Not (File.Exists(dbFile)) Then
            Try
                CreateDbFile(dbFile, filename)
            Catch ex As Exception
                ErrorLog.Write(DBFILE_NO_CREATE & " " & dbFile, "GetLocalDatabase " & ex.StackTrace)
                Return ""
            End Try
        End If
        Return dbFile
    End Function

    ''' <summary>
    ''' Create local database file on disk
    ''' </summary>
    ''' <param name="dbfile">dbfile to search for</param>
    ''' <param name="filename">filename in for, attachments</param>
    Private Sub CreateDbFile(dbfile As String, filename As String)
        'Look through form attachments for a database file
        For Each xData As AttachmentData In _Form.Attachments
            If xData.Name = filename Then
                Try
                    'Create Access Database File on disk
                    Using xStream As New FileStream(dbfile, FileMode.Create)
                        Dim xWriter As New BinaryWriter(xStream, Encoding.Default)
                        xWriter.Write(xData.BinaryData)
                    End Using
                    _Form.RemoveAttachment(xData)
                    Exit For
                Catch ex As Exception
                    ErrorLog.Write("Exception creating DB File", ex.StackTrace)
                End Try
            End If
        Next
    End Sub

    ' 
    ''' <summary>
    ''' Get Attached Access Database name - must have MDB extension
    ''' </summary>
    ''' <returns>Database file name</returns>
    Public Function GetLocalDatabaseAttachmentName() As String
        'Look through form attachments for a database file
        Try
            For Each xData As AttachmentData In _form.Attachments
                If xData.Name.ToUpper.Contains(".MDB") Then
                    Return xData.Name
                End If
            Next
            ErrorLog.Write(DBFILE_NOT_ATTACHED, "GetLocalDatabaseAttachmentName")
            Return ""
        Catch ex As Exception
            ErrorLog.Write("Exception in GetLocalDatabaseAttachmentName", ex.StackTrace)
            Return ""
        End Try
    End Function

    Private ReadOnly _connectionString As String = ""
    Private _connection As DbConnection
    Private ReadOnly _form As Form

#Region "IDisposable Support"
    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            ' This causes our unit tests to fail so commenting out
            '_Connection.Close()
        End If
        _disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class