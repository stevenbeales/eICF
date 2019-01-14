Imports System.IO
Imports MiCo.MiForms
Imports MiCo.MiForms.Server

''' <summary>
''' Class to Download Mico forms dynamically
''' </summary>
Public Class FormDownloader

    ''' <summary>
    ''' Form Downloader Constructor
    ''' </summary>
    ''' <param name="wsi">Web Service Interface</param>
    ''' <param name="formId">Form ID</param>
    Public Sub New(wsi As WebserviceInterfaceEx, formId As String)
        _Wsi = wsi
        _FormId = formId
    End Sub

    ''' <summary>
    ''' Download template dynamically
    ''' </summary>
    ''' <param name="templatePath">Path for MiCo templates</param>
    ''' <returns></returns>
    Public Function DownloadTemplate(templatePath As String) As String
        Try
            'Get all forms associated with user
            Dim templateResponse As ServerResponse = _Wsi.GetFormTemplatesForUser()

            'If no forms found, return empty file name
            If templateResponse.FormTemplates Is Nothing Then
                Return ""
            End If

            Dim tempfile = ""

            For Each form As FormTemplateDescription In templateResponse.FormTemplates
                If (form.FormID = _FormId) Then 'We have found the form we are looking for

                    Dim miForm As New Form()
                    tempfile = Path.Combine(templatePath, _FormId & "-" & form.Revision & ".mft")
                    Dim encFile As String = tempfile & "eps" ' this extension needs to be different than mfte
                    Dim formXml = ""

                    If Not File.Exists(encFile) Then
                        ' download form XML
                        Dim revisionResponse As ServerResponse = _wsi.GetFormTemplateRevision(form.FormID, form.Revision)

                        ' store the template to a form object
                        Directory.CreateDirectory(templatePath)
                        miForm.Load(revisionResponse.StringData)

                        ' download associated images
                        DownloadAssociatedImages(templatePath, form, miForm)

                        ' build new XML with images attached
                        miForm.BuildXMLString(formXml, True, False)

                        ' save the file
                        File.WriteAllText(tempfile, formXml)
                    End If

                    Exit For
                End If
            Next

            Return tempfile
        Catch ex As Exception
            ErrorLog.Write("Exception in Download Template", ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    Private Sub DownloadAssociatedImages(templatePath As String, form As FormTemplateDescription, miForm As Form)
        Try
            Dim revisionDescriptionResponse As ServerResponse = _wsi.GetFormTemplateRevisionDescription(_formId, form.Revision)
            For i = 1 To miForm.Pages.Count
                Dim res As Double = GetMaxImageResolution(revisionDescriptionResponse)

                ' download each background image
                DownloadImage(templatePath, form, miForm, i, res)
            Next
        Catch ex As Exception
            ErrorLog.Write("Exception in Download Associate Images", ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub DownloadImage(templatePath As String, form As FormTemplateDescription, miForm As Form, i As Integer, res As Double)
        Dim imageResponse As ServerResponse = _wsi.GetFormTemplatePageBackgroundImage(_formId, form.Revision, i, res)
        Dim imgFile As String = Path.Combine(templatePath, _formId & "-" & form.Revision & "-" & i & ".png")
        File.WriteAllBytes(imgFile, imageResponse.BinaryData)
        miForm.AddPageBackgroundImage(i, imgFile, True, True)
    End Sub

    Private Function GetMaxImageResolution(revisionDescriptionResponse As ServerResponse) As Double
        Dim res = 0.0
        For Each d As Double In revisionDescriptionResponse.FormTemplates(0).ImageResolutions
            If (d > res) Then
                res = d
            End If
        Next
        Return res
    End Function

    Private ReadOnly _wsi As WebserviceInterfaceEx
    Private ReadOnly _formId As String
End Class