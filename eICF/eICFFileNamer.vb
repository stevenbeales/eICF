Imports System.IO
Imports System.Text

''' <summary>
''' Names eICF Files with metadata for loading into eICF portal
''' </summary>
Public Class EIcfFileNamer

    Public Property SiteNumber As String

    Public Property SubjectNumber As String

    Public Property Version As String

    Public Property Visit As String
        Get
            Return GetVisit()
        End Get
        Set
            _visit = Value
        End Set
    End Property

    Public Property Templatename As String

    Public Property SubjectSigDate As String

    Public Property ConsentorSigDate As String

    Public Property Pgx As String

    Public Property BioMarker As String

    Public Property BackupPath As String

    Public ReadOnly Property PdfFileName As String
        Get
            Return GetPdfFileName()
        End Get
    End Property

    Public Property Language As String
        Get
            Return _language
        End Get
        Set
            _language = Value
        End Set
    End Property

    Private Function GetVisit() As String
        If Not _visit = "" Then
            Return _visit
        Else
            Return "UN" 'Unscheduled visit
        End If
    End Function

    Private Function GetPdfFileName() As String
        Dim sb As New StringBuilder()
        sb.Append(SPONSOR)
        sb.Append(DELIMITER)
        sb.Append(STUDY)
        sb.Append(DELIMITER)
        sb.Append(Templatename)
        sb.Append(DELIMITER)
        sb.Append(SiteNumber)
        sb.Append(DELIMITER)
        sb.Append(SubjectNumber)
        sb.Append(DELIMITER)
        sb.Append(Version)
        sb.Append(DELIMITER)
        sb.Append(SubjectSigDate)
        sb.Append(DELIMITER)
        sb.Append(ConsentorSigDate)
        sb.Append(DELIMITER)
        sb.Append(Pgx)
        sb.Append(DELIMITER)
        sb.Append(BioMarker)
        sb.Append(DELIMITER)
        sb.Append(Language)
        sb.Append(DELIMITER)
        sb.Append(Visit)
        sb.Append(".pdf")
        Return Path.Combine(BackupPath, sb.ToString())
    End Function

    Private _language As String = "en"
    Private _visit As String = ""
End Class
