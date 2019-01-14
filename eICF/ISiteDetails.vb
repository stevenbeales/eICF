Imports MiCo.MiForms.Server

Public Interface ISiteDetails

    ''' <summary>
    ''' Gets Next Subject Number for Site
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Subject Number as String</returns>
    Function NextSubjectNumber(site As Site) As String

    ''' <summary>
    ''' Get Form Template Revision
    ''' </summary>
    ''' <param name="form">Form</param>
    ''' <returns>Revision Number</returns>
    Function GetFormTemplateRevision(form As FormTemplateDescription) As String

    ''' <summary>
    ''' Get Phone
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>Phone Number</returns>
    Function GetPhone(user As User) As String

    ''' <summary>
    ''' Get Form Templates
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>List of Form Templates</returns>
    Function GetFormTemplates(user As User) As FormTemplateDescription()

    ''' <summary>
    ''' Gets Site Number
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Returns 5 Digit Site Number</returns>
    Function GetSiteNumber(site As Site) As String

    ''' <summary>
    ''' Gets local database ID for site 
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Site ID</returns>
    Function GetSiteId(site As Site) As Integer

    ''' <summary>
    ''' Gets local database ID for user
    ''' </summary>
    ''' <param name="user">User</param>
    ''' <returns>User ID</returns>
    Function GetUserId(user As User) As Integer

    ''' <summary>
    ''' Gets local database ID for subject 
    ''' </summary>
    ''' <param name="subject">Subject</param>
    ''' <returns>Subject ID</returns>
    Function GetSubjectId(subject As Subject) As Integer

    ''' <summary>
    ''' List of subjects at site loaded from local database
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>List of Subjects</returns>
    Function GetSubjects(site As Site) As ArrayList

    ''' <summary>
    ''' List of visits for subject loaded from local database
    ''' </summary>
    ''' <param name="subject">Subject</param>
    ''' <returns>List of Visits</returns>
    Function GetVisits(subject As Subject) As ArrayList

    ''' <summary>
    ''' Get Site PI
    ''' </summary>
    ''' <param name="site">Study site</param>
    ''' <returns>PI</returns>
    Function GetPi(site As Site) As User

    ''' <summary>
    ''' Get PI Phone
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Phone Number</returns>
    Function GetPiPhone(site As Site) As String

    ''' <summary>
    ''' Get PI First Name
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>First Name</returns>
    Function GetPiFirstName(site As Site) As String

    ''' <summary>
    ''' Get PI Last Name
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Last Name</returns>
    Function GetPiLastName(site As Site) As String

    ''' <summary>
    ''' Get Site Address
    ''' </summary>
    ''' <param name="site">Site</param>
    ''' <returns>Address</returns>
    Function GetSiteAddress(site As Site) As String

End Interface
