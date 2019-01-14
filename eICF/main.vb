Imports System.IO

    ''' <summary>
    ''' This DLL is used by Mi-Co forms to provide eICF specific required functional such as Encryption, Backup, SharePoint storage, Local Access Database
    ''' This unit contains common classes and constants used across eICF Mi-Co forms
    ''' Please refer to inviduals classes for class-specific functionality 
    ''' </summary>
    Public Module eICFGlobals
        Public Const DELIMITER = "$" 'File Delimiter
        Public Const VISIT_FORM As String = "675040"
        Public Const EXCLUDED_FORMS_FROM_DOWNLOAD = VISIT_FORM 'Visit Pages form
        Public Const SITE_NUMBER_LENGTH = 5
        Public Const SUBJECT_NUMBER_LENGTH = 3
        Public Const VISIT_NUMBER_LENGTH = 2
        Public Const CONFIG_FILE_NAME = "eICF.config"
        Public Const MICO_GROUP_PREFIX = "Takeda_Convene_" 'Mico Groups for study 'TODO - make this a configuration
        Public Lockfield As Object = New Object 'Synchronize threads when writing to log
        Public Const VERSION_NUMBER As Decimal = 1.0
        Public Const DATEFORMAT As String = "MM/dd/yyyy"
        Public Const TIMEFORMAT As String = "hh:mm"
        Public Const AUTOSYNC_INTERVAL As Integer = 1
        Public Const PDF_TEMPLATE As String = "blank.pdf"
        Public Const SERVER_PATH As String = "C:\remotesource\source"
        Public Const SPONSOR = "Takeda"
        Public Const STUDY = "Convene"

        'Constants to be translated
        Public Const LOST_CONNECTION_TO_DATABASE As String = "Lost connection to database."
        Public Const DBFILE_NOT_ATTACHED As String = "Cannot find required file - local database attachment. Please contact ePharmaSolutions Help Desk for assistance."
        Public Const DBFILE_NO_OPEN As String = "Cannot open local database file. Please contact ePharmaSolutions Help Desk for assistance. "
        Public Const DBFILE_NO_CREATE As String = "Cannot create local database file "
        Public Const CONFIGFILE_NO_CREATE As String = "Cannot create local configuration file "
        Public Const SITE_NOT_FOUND As String = "Site not found  in database. Please contact ePharmaSolutions Help Desk for assistance."
        Public Const NO_PDF_ATTACHMENT As String = "Cannot create PDF. No PDF was attached to template. Please contact ePharmaSolutions Help Desk for assistance."
        Public Const PDF_GENERATION_FAILURE As String = "Failed to generate PDF: "
        Public Const CONFIGURATION_NOT_FOUND As String = "Could not find Configuration:"
        Public Const CONFIGURATION_FILE_NOT_FOUND As String = "Could not find Configuration File."
        Public Const CANNOT_CONNECT_PORTAL As String = "Unable to connect to Clinical Trial Portal. Working in Offline Mode"
        Public Const CANNOT_FIND_PREFS_FILE As String = "Cannot find Preferences File. Please contact ePharmaSolutions Help Desk for assistance."
        Public Const VISIT_NUMBER_NOT_VALID As String = "Visit Number is not valid."
        Public Const MSGBOX_TITLE As String = "Electronic Informed Consent Form"
        Public Const PLEASE_SELECT_A_SUBJECT As String = "Please use the grid to select a Subject first."
        Public Const CANNOT_LOAD_CONSENT = "Cannot load Informed Consent Form. Please contact ePharmaSolutions Help Desk for assistance."
        Public Const ERROR_SETTING_DEFAULTS_CONSENT = "Not all values were defaulted on Informed Consent Form. Please ask your consent administrator to check data before proceeding."
        Public Const INVALID_YEAR = "Please enter a valid year."
        Public Const LATER_VISIT_EXISTS = "A Later Visit with a higher Visit Number already exists for this Subject."
        Public Const PLEASE_SELECT_SUBJECT = "Please select a Subject."
        Public Const SITE_ERROR_SAVING = "There was an error saving the new Site to the database."
        Public Const SUBJECT_ALREADY_EXISTS = "A Subject with this Subject Number already exists."
        Public Const SUBJECT_NUMBER_INVALID_CHAR_LEN = "Subject Number must be 8 characters long."
        Public Const SUBJECT_NUMBER_CANNOT_END_000 = "Subject Number cannot end with 000."
        Public Const SUBJECT_INITIALS_INVALID_CHAR_LEN = "Subject Initals must contain a space if there is no middle initial."
        Public Const SUBJECT_INITIALS_EMPTY = "Subject Initials cannot be blank."
        Public Const SUBJECT_NUMBER_NO_MATCH = "Subject Numbers do not match. Please try again."
        Public Const SUBJECT_INITIALS_NO_MATCH = "Subject Initials do not match. Please try again."
        Public Const SUBJECT_ERROR_SAVING = "There was an error saving the new Subject to the database."
        Public Const USER_NOT_WITH_SITE = "User is not associated with a Site. Please contact your monitor or ePharmaSolutions Help Desk for assistance."
        Public Const USER_ACCOUNT_NOT_ACTIVE = "You User Account is not Active. Please contact your monitor or ePharmaSolutions Help Desk for assistance."
        Public Const VALIDATION_ERRORS = "Not all required fields have been completed. You must complete all required fields before submitting the Informed Consent."
        Public Const VALIDATION_ERROR = "Not all required fields have been completed. You must complete all required fields before continuing to the next page."
        Public Const USER_ERROR_SAVING = "There was an error saving the current user to the database."
        Public Const VISIT_ERROR_SAVING = "There was an error saving the new Visit to the database."
        Public Const VISIT_ALREADY_EXISTS = "A Visit with this Visit Number already exists for this Subject."

        'In eICF.Config file
        'Public Const LOCAL_CONNECTION_STRING = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
        'Public Const LIVE_HELP_URL = "https://hosted+++usa5.whoson.com/chat/chatstart.htm?domain=www.epharmasolutions.com"
        'Public Const VIDEO_LINK = "C:\temp\videos\eICFTutorial.pptx"
        'Public Const MICO_SERVER_PATH = "C:\RemoteSource\Source\"	
        'Public Const SPONSORNAME = "Takeda"
        'Public Const STUDYNAME = "Convene"
    End Module

    ''' <summary>
    ''' App specific Msgbox Wrapper
    ''' </summary>
    Public Class AMsgBox
        Public Shared Function Show(prompt As String, buttons As MsgBoxStyle) As MsgBoxResult
            Return MsgBox(prompt, buttons, MSGBOX_TITLE)
        End Function
    End Class

    ''' <summary>
    ''' eICF specific validation classes
    ''' </summary>
    Public Class EIcfValidation
        ''' <summary>
        ''' Validates the year of the Date of Visit is between now and 25 years in future
        ''' </summary>
        ''' <param name="dateString">Date</param>
        ''' <returns>True or false</returns>
        Public Shared Function IsValidYear(dateString As String) As Boolean
            Dim dateStr As Date = dateString
            Dim year As Integer = dateStr.Year
            Dim currentYear As Integer = Date.Now.Year
            Return (year >= currentYear And year <= (currentYear + 25))
        End Function

        ''' <summary>
        ''' Validates the character length of the Visit Number and Visit Number is not 00
        ''' </summary>
        ''' <param name="visitNumberString">Visit Number</param>
        ''' <returns>true or false</returns>
        Public Shared Function IsValidVisitNumber(visitNumberString As String) As Boolean
            Dim length As Integer = visitNumberString.Length
            Return length = VISIT_NUMBER_LENGTH And (Not visitNumberString = "00")
        End Function

        ''' <summary>
        ''' Validates the character length of the Subject Number
        ''' </summary>
        ''' <param name="subjectNumberString">Subject Number</param>
        ''' <returns>true or false</returns>
        Public Shared Function IsValidSubjectNumber(subjectNumberString As String) As Boolean
            Dim length As Integer = subjectNumberString.Length
            Return length = SUBJECT_NUMBER_LENGTH + SITE_NUMBER_LENGTH
        End Function

        ''' <summary>
        ''' Validates that Subject Initials is "AAA" or "A_A"
        ''' </summary>
        ''' <param name="subjectInitialsString">Subject Initials</param>
        ''' <returns>True or False</returns>
        Public Shared Function IsValidSubjectInitials(subjectInitialsString As String) As Boolean
            Dim length As Integer = subjectInitialsString.Trim().Length
            Return length = SUBJECT_NUMBER_LENGTH
        End Function

        Public Shared Function IsValidSubjectNumberEnding(subjectNumberString As String) As Boolean
            Return Not Right(subjectNumberString, 3) = "000"
        End Function
    End Class


    ''' <summary>
    ''' App specific Error and Information Logger 
    ''' </summary>
    Public Class ErrorLog
        'PURPOSE:       Open or create an error log and submit error message
        'PARAMETERS:    msg - message to be written to error file
        '               stkTrace - stack trace from error message
        Public Shared Sub Write(msg As String, stkTrace As String, Optional errorMessage As String = "")

            'If we are unit testing or debugging, log to a different file to keep production log clean.
#If DEBUG Then
    Using s1 As StreamWriter = File.AppendText("c:\eicf\testerrors.txt")
        s1.WriteLine("Message: " & msg)
        s1.WriteLine("Error: " & errorMessage)
        s1.WriteLine("StackTrace: " & stkTrace)
        s1.WriteLine("Date/Time: " & Date.Now.ToString())
    End Using 
    Return
#End If
            'log to the file the error message
            SyncLock (Lockfield) 'Use a lock because we have separate threads accessing the log file
                Using s1 As StreamWriter = File.AppendText("c:\eicf\errlog.txt")
                    s1.AutoFlush = True
                    s1.WriteLine("Message: " & msg)
                    If Not errorMessage = "" Then
                        s1.WriteLine("Error: " & errorMessage)
                    End If
                    s1.WriteLine("StackTrace: " & stkTrace)
                    s1.WriteLine("Date/Time: " & Date.Now.ToString())
                End Using
            End SyncLock
        End Sub
    End Class

    ''' <summary>
    ''' Logger used in Unit Tests
    ''' </summary>
    Public Class TestLogger
        'PURPOSE:       Open or create a log for unit testing
        'PARAMETERS:    msg - message to be written to test file
        '               result - result from test message
        Public Shared Sub Write(msg As String, result As String)
            'check and make the directory if necessary; Set to look in the application folder
            'log the message
            Using s1 As StreamWriter = File.AppendText("c:\eicf\tests.txt")
                s1.WriteLine("Message: " & msg)
                s1.WriteLine("Result: " & result)
                s1.WriteLine("Date/Time: " & Date.Now.ToString())
            End Using
        End Sub
    End Class

    ''' <summary>
    ''' General Functions
    ''' </summary>
    Public Class Utils
        'Left fill with zeros e.g. 0001
        Public Shared Function ZeroFill(i As Integer, pad As Integer) As String
            Return i.ToString().PadLeft(pad, "0")
        End Function

        'If flag is blank, return "Y" because subject consents by not checking box
        Public Shared Function GetFlagConsentValue(flag As String) As String
            If flag = "" Then
                Return "Y"
            Else
                Return "N"
            End If
        End Function

End Class