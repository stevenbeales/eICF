Imports Microsoft.SharePoint.Client

Public interface IContextProvider
    ''' <summary>
    ''' Our client context for running CSOM CAML queries	
    ''' </summary>
    ''' <returns>Client context</returns>
    ReadOnly Property Value As ClientContext

    ''' <summary>
    ''' SharePoint Site URL for eICF Portal 
    ''' </summary>
    ''' <returns>URL</returns>
    Property SiteUrl As String

    ''' <summary>
    ''' Password of SharePoint Service account
    ''' </summary>
    ''' <returns>Password</returns>
    Property Password As String

    ''' <summary>
    ''' Username of SharePoint Service account
    ''' </summary>
    ''' <returns>User name</returns>
    Property UserName As String
end interface