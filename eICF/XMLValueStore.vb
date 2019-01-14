Imports System.IO
Imports System.Xml
Imports MiCo.MiForms

''' <summary>
''' Class used to read Mi-Co preferences file and get class specific information
''' </summary>
Public Class XmlValueStore

    ''' <summary>
    ''' Constructor 
    ''' </summary>
    ''' <param name="strFile">Mi-Co preferences file</param>
    Public Sub New(strFile As String)
        _strFile = strFile
        ReadPrefsFile()
    End Sub

    ''' <summary>
    ''' Gets a String Property from Mi-Co Prefs.xml
    ''' </summary>
    ''' <param name="strName">Key Value</param>
    ''' <returns>A string</returns>
    Public Function [Get](strName As String) As String
        Dim strValue As String
        strValue = _Prefs.[Get](strName)
        If strValue Is Nothing Then
            strValue = ""
        End If
        Return strValue
    End Function

    ''' <summary>
    ''' Gets a Number Property from Mi-Co Prefs.xml
    ''' </summary>
    ''' <param name="strName">Key Value</param>
    ''' <returns>A number</returns>
    Public Function GetNumber(strName As String) As Integer
        Dim strValue As String
        Dim nValue As Integer = 0
        strValue = [Get](strName)
        If Not String.IsNullOrEmpty(strValue) Then
            Try
                nValue = Convert.ToInt32(strValue)
            Catch ex As Exception
                'if not a number, return 0
            End Try
        End If
        Return nValue
    End Function

    ''' <summary>
    ''' Gets a Boolean Property from Mi-Co Prefs.xml
    ''' </summary>
    ''' <param name="strName">Key Value</param>
    ''' <returns>True or False</returns>
    Public Function GetBool(strName As String) As Boolean
        Dim strValue As String
        strValue = [Get](strName)
        If strValue = "True" Then
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Reads Mi-Co preference file
    ''' </summary>
    Private Sub ReadPrefsFile()
        _Prefs.Clear()

        'If we find the Prefs.xml file
        If File.Exists(_strFile) Then
            Dim xReader As XmlTextReader
            Dim xFileStream As FileStream
            Try
                'Read file into XML memory stream
                xFileStream = New FileStream(_strFile, FileMode.Open, FileAccess.Read, FileShare.Read, 131072)
                xReader = New XmlTextReader(xFileStream)
            Catch ex As Exception
                Return
            End Try

            Try
                While xReader.Read() 'While XML stream has contents
                    If xReader.NodeType = XmlNodeType.Element Then
                        If xReader.Name = _strHeader Then 'If we've found the header node
                            For c = 0 To xReader.AttributeCount - 1
                                xReader.MoveToAttribute(c) 'Get the timestamp and version from the header
                                If xReader.Name = "timestamp" Then
                                    _Timestamp = DateTimeUtils.UnixTimestampToDateTime(xReader.Value)
                                ElseIf xReader.Name = "version" Then
                                    _strVersion = xReader.Value
                                End If
                            Next
                        Else
                            Dim strName = ""
                            Dim strValue = ""
                            For c = 0 To xReader.AttributeCount - 1
                                xReader.MoveToAttribute(c) 'Add the Values and names to the prefs file
                                If xReader.Name = "name" Then
                                    strName = xReader.Value
                                ElseIf xReader.Name = "value" Then
                                    strValue = xReader.Value
                                    If Not String.IsNullOrEmpty(strName) Then
                                        _Prefs.Add(strName, strValue)
                                    End If
                                End If
                            Next
                        End If
                    End If
                End While
            Catch ex As Exception
                ErrorLog.Write("Error getting preferences from prefs.XML", ex.StackTrace, ex.Message)
            Finally
                xReader.Close()
            End Try
        End If
    End Sub

    Private ReadOnly _strFile As String = ""
    Private _strVersion As String = ""
    Private _Timestamp As DateTime
    Private ReadOnly _Prefs As New Specialized.NameValueCollection()
    Private _strHeader As String = "prefs"
End Class
