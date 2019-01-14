using MiCo.MiApp.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ePs.MicoFormIntegration.Service
{
    /// <summary>
    /// Operations being exposed by the service.
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string GetTokenString(CompositeType composite);
        
        [OperationContract]
        string AddFormTemplateToGroup(RequestParams requestParams);

        [OperationContract]
        string SetNewPassword(CredentialParams credentialParams);
    }

    /// <summary>
    /// Data consumed by GetTokenString and EncryptedPasswordString.
    /// </summary>
    [DataContract]
    public class CompositeType
    {
        string _customerName = "";
        string _userName = "";
        string _password = "";

        [DataMember]
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        [DataMember]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        [DataMember]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }

    /// <summary>
    /// Data consumed by AddFormTemplateToGroup and AddFormTemplate.
    /// </summary>
    [DataContract]
    public class RequestParams
    {
        string _customerName;
        string _token;
        string _username;
        string _groupName;
        bool _includeInactive;
        string _formName;

        [DataMember]
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        [DataMember]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        [DataMember]
        public bool IncludeInactive
        {
            get { return _includeInactive; }
            set { _includeInactive = value; }
        }

        [DataMember]
        public string FormName
        {
            get { return _formName; }
            set { _formName = value; }
        }

        [DataMember]
        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }
    }

    /// <summary>
    /// Data consumed by SetNewPassword and SetPassword.
    /// </summary>
    [DataContract]
    public class CredentialParams
    {
        string _customerName = "";
        string _token = "";
        string _userName = "";
        string _newPassword = "";

        [DataMember]
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        [DataMember]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        [DataMember]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        [DataMember]
        public string NewPassword
        {
            get { return _newPassword; }
            set { _newPassword = value; }
        }
    }
}
