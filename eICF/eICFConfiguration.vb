Imports System.Configuration
Imports System.IO
Imports System.Text
Imports MiCo.MiForms

''' <summary>
''' Class to manage eICF configuration using eICF.config file
''' </summary>
Public Class EIcfConfiguration

    Public Sub New(form As Form)
        _form = form
    End Sub

    ''' <summary>
    ''' Open configuration file
    ''' </summary>
    Public Sub OpenConfigurationFile()
        Dim configFile As String = GetConfigFile()
        Try
            'We must use a mapped configuration file since our mico app can't use app.config
            Dim map As New ExeConfigurationFileMap()
            map.ExeConfigFilename = configFile
            _config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None)
        Catch ex As Exception
            ErrorLog.Write(CONFIGURATION_FILE_NOT_FOUND & " " & configFile, "OpenConfigurationFile", ex.Message)
            AMsgBox.Show(CONFIGURATION_FILE_NOT_FOUND, MsgBoxStyle.Critical)
        End Try
    End Sub

    ''' <summary>
    ''' Lookup configuration file setting by key.
    ''' Configurations are stored in appSettings section as key/value pairs
    ''' </summary>
    ''' <param name="key">lookup key</param>
    ''' <returns>value</returns>
    Public Function GetConfigurationSetting(key As String) As String
        Dim section As AppSettingsSection = _config.GetSection("appSettings")
        Try
            Dim value As String = section.Settings(key).Value
            If IsNothing(value) Then
                ErrorLog.Write(CONFIGURATION_NOT_FOUND & " " & key, "GetConfigurationSetting")
                AMsgBox.Show(CONFIGURATION_NOT_FOUND & " " & key, MsgBoxStyle.Exclamation)
                Return ""
            End If
            Return value
        Catch ex As Exception
            ErrorLog.Write("Exception in GetConfigurationSetting", ex.StackTrace, ex.Message)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Get eICF Configuration File 
    ''' </summary>
    ''' <returns>file name</returns>
    ''' <example> eICF.config is default name</example>
    Private Function GetConfigFile() As String
        Dim configFile As String = Path.GetTempPath() & CONFIG_FILE_NAME
        Try
            If (File.Exists(configFile)) Then
                File.Delete(configFile)
            End If
            CreateConfigFile(configFile, CONFIG_FILE_NAME)
            Return configFile
        Catch ex As Exception
            ErrorLog.Write(CONFIGFILE_NO_CREATE & " " & configFile, "GetConfigFile", ex.Message)
            AMsgBox.Show(CONFIGFILE_NO_CREATE & " " & configFile, MsgBoxStyle.Critical)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Create local config file on disk
    ''' </summary>
    ''' <param name="configfile">input file</param>
    ''' <param name="filename">output file name</param>
    Private Sub CreateConfigFile(configfile As String, filename As String)
        'Look through form attachments for a config file
        Try
            For Each xData As AttachmentData In _form.Attachments
                If xData.Name = filename Then
                    'Create Config File on disk
                    Using xStream As New FileStream(configfile, FileMode.Create)
                        Dim xWriter As New BinaryWriter(xStream, Encoding.Default)
                        xWriter.Write(xData.BinaryData)
                    End Using
                    _form.RemoveAttachment(xData)
                    Exit For
                End If
            Next
        Catch ex As Exception
            ErrorLog.Write("Exception in CreateConfigFile: ", Ex.StackTrace, Ex.Message)
        End Try
    End Sub

    Private ReadOnly _form As Form
    Private _config As Configuration
End Class

