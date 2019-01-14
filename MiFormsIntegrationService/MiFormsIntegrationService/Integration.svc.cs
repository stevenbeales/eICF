using MiCo.MiApp.Server;
using MiCo.MiApp.Server.Error;
using ePs.MicoFormIntegration.Service.MiForms.AuthServices;
using ePs.MicoFormIntegration.Service.MiForms.SyncServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NLog;


namespace ePs.MicoFormIntegration.Service
{
    public class Integration : IService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Calls EncryptedPasswordString method.
        /// </summary>
        /// <param name="composite">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        public string GetTokenString(CompositeType composite)
        {
            return EncryptedPasswordString(composite);
        }

        /// <summary>
        /// Calls AddFormTemplate method.
        /// </summary>
        /// <param name="requestParams">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        public string AddFormTemplateToGroup(RequestParams requestParams)
        {
            return AddFormTemplate(requestParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialParams"></param>
        /// <returns></returns>
        public string SetNewPassword(CredentialParams credentialParams)
        {
            return SetPassword(credentialParams);
        }

        /// <summary>
        /// Generates a token, based on the credentials provided, that expires in 1 hours.
        /// It is assumed that there will be a default admin account used.
        /// </summary>
        /// <param name="composite">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        private string EncryptedPasswordString(CompositeType composite)
        {
            var token = new AuthToken();
            try
            {
                // consume web service
                var authService = new AuthServices();

                // get the public key
                var keyPairResponse = ServerResponse.FromJSON(authService.GetKeyPair(composite.CustomerName, composite.UserName));
                var publicKey = keyPairResponse.Key.XML;

                // Encrypt password with the public key
                var asymEnc = new MiCo.MiApp.Server.Encryption.Asymmetric();
                asymEnc.UpdateFromString(publicKey);
                var pwBytes = System.Text.Encoding.UTF8.GetBytes(composite.Password);
                var pwEncrypt = asymEnc.Encrypt(pwBytes);
                var pwHex = MiCo.MiApp.Server.Encryption.Hex.BytesToHexString(pwEncrypt);

                // get the authentication token
                var exp = DateTime.Parse(keyPairResponse.ResponseDate);
                exp = exp.AddHours(1);
                var authTokenService = authService.GetAuthToken(composite.CustomerName, composite.UserName, pwHex, exp.ToString("s"), -1);
                var serverResponse = ServerResponse.FromJSON(authTokenService);
                if (serverResponse.Success)
                {
                    token = serverResponse.Token;
                }
                else if (serverResponse.Error.GetType() == typeof(AuthenticationError))
                {
                    var err = (AuthenticationError)serverResponse.Error;

                    token.TokenValue = HandleAuthenticationError(err, serverResponse);
                }
                else
                {
                    var parsed = JObject.Parse(authTokenService);

                    token.TokenValue = parsed.SelectToken("Error.Details").ToString();
                }
            }
            catch (Exception ex)
            {
                token.TokenValue = ex.Message;
            }

            logger.Log(LogLevel.Info, token.TokenValue + "; Customer Name: " + composite.CustomerName + "; Username: " + composite.UserName);

            return token.TokenValue;
        }

        /// <summary>
        /// Returns one formId based on the unique Mi-Forms template description provided.
        /// The assumption is made that a Mi-Forms template will have a unique form name and description.
        /// syncService.GetFormTemplate returns a list of form templates based on customer name, token,
        ///   form name and username.
        /// Since the form name will be unique, only one form template should be returned.  If not,
        ///   the first element in the list will be returned.
        /// For this to work, we setup a master group (TakedaMaster) and master user (TemplateAdmin) that 
        /// contains and has access to all the unique forms on the Mi-Forms server.
        /// </summary>
        /// <param name="requestParams">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        private string FormTemplate(RequestParams requestParams)
        {
            string formId = "";

            try {
                // consume web service
                var syncService = new SyncServices();
                
                // get form associated to master user
                var formTemplates = syncService.GetFormTemplates(requestParams.CustomerName, requestParams.Token, requestParams.UserName, requestParams.IncludeInactive);
                var serverResponse = ServerResponse.FromJSON(formTemplates);

                if (serverResponse.Success)
                {
                    var parsed = JObject.Parse(formTemplates);

                    IEnumerable<JToken> forms = parsed.SelectTokens("$..FormTemplates[?(@.Name == '" + requestParams.FormName + "')]");

                    // get formId
                    formId = forms.FirstOrDefault().SelectToken("FormID").ToString();
                }
                else if (serverResponse.Error.GetType() == typeof(AuthenticationError))
                {
                    var err = (AuthenticationError)serverResponse.Error;

                    formId = HandleAuthenticationError(err, serverResponse);
                }
                else
                {
                    var parsed = JObject.Parse(formTemplates);

                    formId = parsed.SelectToken("Error.Details").ToString();
                }
            }
            catch (Exception ex)
            {
                formId = null;
            }

            logger.Log(LogLevel.Info, formId + "; Customer Name: " + requestParams.CustomerName + "; Username: " + requestParams.UserName);

            return formId;
        }

        /// <summary>
        /// Form template will be associated to the designated group.
        /// </summary>
        /// <param name="requestParams">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        private string AddFormTemplate(RequestParams requestParams)
        {
            string template = String.Empty;

            try
            {
                // get formId
                var formId = FormTemplate(requestParams);

                if (!String.IsNullOrEmpty(formId))
                {
                    // consume web service
                    var authService = new AuthServices();

                    // add template to group
                    var addTemplate = authService.AddFormTemplateToGroup(requestParams.CustomerName, requestParams.Token, formId, requestParams.GroupName);
                    var serverResponse = ServerResponse.FromJSON(addTemplate);

                    if (serverResponse.Success)
                    {
                        template = serverResponse.Success.ToString();
                    }
                    else if (serverResponse.Error.GetType() == typeof(AuthenticationError))
                    {
                        var err = (AuthenticationError)serverResponse.Error;

                        template = HandleAuthenticationError(err, serverResponse);
                    }
                }
                else
                {
                    template = "Variable formId is blank.  It could be that either the Customer Name and/or Username provided is incorrect.";
                }
            }
            catch (Exception ex)
            {
                template = ex.Message;
            }

            logger.Log(LogLevel.Info, template + "; Customer Name: " + requestParams.CustomerName + "; Username: " + requestParams.UserName);

            return template;
        }

        /// <summary>
        /// Sets the password for a user.
        /// </summary>
        /// <param name="credentialParams">DataContract; data to be exchanged.</param>
        /// <returns></returns>
        private string SetPassword(CredentialParams credentialParams)
        {
            string success = "";

            try
            {
                // consume web service
                var authService = new AuthServices();

                var setPassword = authService.SetPassword(credentialParams.CustomerName, credentialParams.Token, credentialParams.UserName, credentialParams.NewPassword);
                var serverResponse = ServerResponse.FromJSON(setPassword);
                //var serRes = ServerResponse.ToJSON(setPassword);

                if (serverResponse.Success)
                {
                    success = serverResponse.Success.ToString();
                }
                else if (serverResponse.Error.GetType() == typeof(AuthenticationError))
                {
                    var err = (AuthenticationError)serverResponse.Error;

                    success = HandleAuthenticationError(err, serverResponse);
                }
                else
                {
                    var parsed = JObject.Parse(setPassword);

                    success = parsed.SelectToken("Error.Details").ToString();
                }
            }
            catch (Exception ex)
            {
                success = ex.Message;
            }

            logger.Log(LogLevel.Info, success + "; Customer Name: " + credentialParams.CustomerName + "; Username: " + credentialParams.UserName);

            return success;
        }

        /// <summary>
        /// Evaluates the an error based on standard authentication errors and returns a pre-defined message.
        /// Any non-authentication errors are returned as a message, as well.
        /// </summary>
        /// <param name="err">Authentication error to be evaulated by the method.</param>
        /// <param name="response">Server response that contains a simple success/failure boolean as well as
        /// data returned from the call.  It also contains a potentially non-null error class that indicates
        /// the cause of failure.</param>
        /// <returns></returns>
        private string HandleAuthenticationError(AuthenticationError err, ServerResponse response)
        {
            string message = "";

            if (err.AccountInactive)
            {
                message = "Account Inactive";
            }
            else if (err.AccountLocked )
            {
                message = "Account Locked";
            }
            else if (err.CustomerNotFound)
            {
                message = "Customer not found";
            }
            else if (err.NotAdmin)
            {
                message = "Not admin";
            }
            else if (err.NotAuthorized)
            {
                message = "Not authorized";
            }
            else if (err.PasswordChangeRequired)
            {
                message = "Password change required";
            }
            else if (err.PasswordExpired)
            {
                message = "Password expired";
            }
            else if (err.TokenExpired)
            {
                message = "Token expired";
            }
            else if (err.TokenMaxExceeded)
            {
                message = "Exceeded token max usage";
            }
            else if (err.UsernamePasswordFailure)
            {
                message = "Username/Password failed";
            }
            else
            {
                message = response.Error.Exception.Message;
            }

            return message;
        }
    }
}
