Imports Microsoft.SharePoint.Client

'''<summary>
'''SharePoint Context Provider provides authentication and access to SharePoint site using CSOM
''' It enables our eICF Mi-CO app to connect to Clinical Trial Portal
'''</summary>
Public Class SharePointContextProvider
    Implements IContextProvider

    ''' <summary>
    ''' Our client context for running CSOM CAML queries	
    ''' </summary>
    ''' <returns>Client context</returns>
    Public ReadOnly Property Value As ClientContext Implements IContextProvider.Value
        Get
            Dim clientContext As New ClientContext(SiteUrl)
            Try
                clientContext.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication
                clientContext.FormsAuthenticationLoginInfo = FormsAuthInfo

                'Event handler below is necessary to use forms based authentication in a claims based site
                AddHandler clientContext.ExecutingWebRequest, Function(sender As Object, e As WebRequestEventArgs)
                                                                  e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f")
                                                                  Return Nothing
                                                              End Function
                Return clientContext
            Catch ex As Exception
                ErrorLog.Write(CANNOT_CONNECT_PORTAL, ex.StackTrace)
                Return clientContext
            End Try
        End Get
    End Property

    ''' <summary>
    ''' SharePoint Site URL for eICF Portal 
    ''' </summary>
    ''' <returns>URL</returns>
    Public Property SiteUrl As String Implements IContextProvider.SiteUrl

    ''' <summary>
    ''' Password of SharePoint Service account
    ''' </summary>
    ''' <returns>Password</returns>
    Public Property Password As String Implements IContextProvider.Password

    ''' <summary>
    ''' Username of SharePoint Service account
    ''' </summary>
    ''' <returns>User name</returns>
    Public Property UserName As String Implements IContextProvider.UserName

    ''' <summary>
    ''' SharePoint Forms Based  Authentication Object
    ''' </summary>
    ''' <returns>Return SharePoint login object</returns>
    Private ReadOnly Property FormsAuthInfo As FormsAuthenticationLoginInfo
        Get
            Return New FormsAuthenticationLoginInfo(UserName, Password)
        End Get
    End Property
End Class
