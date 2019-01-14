Imports System.IO

''' <summary>
''' PDFUtils is a class that contains shared functions for reading and encrypting PDFs
''' </summary>
Public Class PdfUtils

    ''' <summary>
    '''  Encrypts PDF File using fast salted hash algorithm in Encrypter 
    ''' </summary>
    ''' <param name="pdfFileName"></param>
    Public Shared Sub EncryptPdfFile(pdfFileName As String)
        Dim pdfText As String = ReadPdf(pdfFileName)
        Dim encryptedPdfText As String = Encrypter.Encrypt(pdfText)

        Using sw As StreamWriter = File.CreateText(pdfFileName)
            sw.Write(encryptedPdfText)
        End Using
    End Sub
    
    ''' <summary>
    ''' Remove PDF File
    ''' </summary>
    ''' <param name="pdfFileName">File name</param>
    Public Shared Sub RemovePdfFile(pdfFileName As String)
        If File.Exists(pdfFileName) Then
            File.Delete(pdfFileName)
        End If
    End Sub

    ''' <summary>
    ''' Read PDF File into memory and return string
    ''' </summary>
    ''' <param name="pdfFileName"></param>
    ''' <returns></returns>
    Private Shared Function ReadPdf(pdfFileName As String) As String
        Dim pdfText = ""
        Try
            Using streamReader As New StreamReader(pdfFileName)
                pdfText = streamReader.ReadToEnd()
            End Using
            Return pdfText
        Catch ex As Exception
            ErrorLog.Write("Error in reading PDF", ex.Message & " " & pdfFileName)
            Return ""
        End Try
    End Function

End Class

