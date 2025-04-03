using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Authentication;

namespace Blue.Cosacs.Shared.Net
{
    internal static class WebRequestUtils
    {
        internal static AuthenticationException CreateCustomException(string uri, AuthenticationException ex)
        {
            if (uri.StartsWith("https"))
            {
                return new AuthenticationException(
                    string.Format("Invalid remote SSL certificate, overide with: \nServicePointManager.ServerCertificateValidationCallback += ((sender, certificate, chain, sslPolicyErrors) => isValidPolicy);"),
                    ex);
            }
            return null;
        }
    }
}
