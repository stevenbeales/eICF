Imports System.Globalization
Imports MiCo.MiForms

''' <summary>
''' Presenter class to make MiCo form logic reusable 
''' </summary>
Public Class Presenter

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="form">Form</param>
    Public Sub New(form As Form)
        _form = form
        SetDefaultDateFormats()
    End Sub

    ''' <summary>
    ''' Allow checkbox to be toggled with stylus
    ''' </summary>
    ''' <param name="control">Control that is being inked</param>
    Public Sub ToggleCheckBox(control As FormControl)
        If control.GetType() Is GetType(Checkbox) Then
            Dim cb = CType(control, Checkbox)
            If cb.Value = cb.DesignValue Then
                ' Check to see if the data element's data corresponds to the added ink
                Dim s As Stroke = cb.AllInk(cb.AllInk.Count - 1)
                If (s.Timestamp - CType(cb.Data(cb.Data.Count - 1), Data).Timestamp).Milliseconds > 100 Then
                    cb.Value = ""
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Default date and time format
    ''' </summary>
    ''' <returns>DatenTime format info</returns>
    Public ReadOnly Property Fmt As DateTimeFormatInfo
        Get
            Return _fmt
        End Get
    End Property

    Private Sub SetDefaultDateFormats()
        Fmt.ShortDatePattern = DATEFORMAT
        Fmt.ShortTimePattern = TIMEFORMAT
    End Sub

    Private ReadOnly _fmt As DateTimeFormatInfo = (New CultureInfo("en-US")).DateTimeFormat
    Private _form As Form
End Class