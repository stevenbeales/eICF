Imports System.Data.Common
Imports MiCo.MiForms
Imports MiCo.MiForms.Server

Public Interface IConnectionManager
    
    ''' <summary>
    ''' Verify server and user connectivity to MiCo Server
    ''' </summary>
    ''' <param name="wsi">Web Service Interface</param>
    ''' <returns>True or False</returns>
    Function AuthenticateUser(ByRef wsi As WebserviceInterfaceEx) As Boolean
    
    ''' <summary>
    ''' Setup our web server call into Mi-Co server by reading from MiCo prefs file
    ''' </summary>
    ''' <param name="wsi">Web Service Interface</param>
    Sub InitializeServerInterface(ByRef wsi As WebserviceInterfaceEx)

    ''' <summary>
    ''' Is the system online?
    ''' </summary>
    ''' <returns>True or false</returns>
    ReadOnly Property Online As Boolean

    ''' <summary>
    ''' Logged in user credentials
    ''' </summary>
    ''' <returns>User credentials</returns>
    Property UserCredentials As Credentials

    ''' <summary>
    ''' Local Database Connection
    ''' </summary>
    ''' <returns>Access database connection</returns>
    ReadOnly Property Connection As DbConnection

    ''' <summary>
    ''' SharePoint Connection
    ''' </summary>
    ''' <returns>SharePoint Connection Provider</returns>
    ReadOnly Property Provider As SharePointContextProvider
End Interface