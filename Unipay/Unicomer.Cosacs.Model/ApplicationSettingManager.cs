using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public static class ApplicationSettingManager
    {
        public static bool LogApiSuccessResponse
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["LogApiSuccessResponse"].ToString(),"true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool LogDbError
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["LogDbError"].ToString(), "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool LogDbValidation
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["LogDbValidation"].ToString(), "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static bool LogApiError
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["LogApiError"].ToString(), "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }
        public static bool LogApiRequest
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["LogApiRequest"].ToString(), "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static string MambuBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuBaseUrl"].ToString();
            }
        }

        public static string MambuTokenUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuTokenUrl"].ToString();
            }
        }

        public static string MambuGrantType
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuGrantType"].ToString();
            }
        }

        public static string MambuClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuClientId"].ToString();
            }
        }

        public static string MambuClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuClientSecret"].ToString();
            }
        }

        public static string MambuClientScope
        {
            get
            {
                return ConfigurationManager.AppSettings["MambuClientScope"].ToString();
            }
        }

        public static string WorkflowBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowBaseUrl"].ToString();
            }
        }

        public static string WorkflowTokenUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowTokenUrl"].ToString();
            }
        }

        public static string WorkflowGrantType
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowGrantType"].ToString();
            }
        }

        public static string WorkflowClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowClientId"].ToString();
            }
        }

        public static string WorkflowClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowClientSecret"].ToString();
            }
        }

        public static string WorkflowClientScope
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowClientScope"].ToString();
            }
        }

        public static string WorkflowAccept
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkflowAccept"].ToString();
            }
        }
        
        public static string EXbrand
        {
            get
            {
                return ConfigurationManager.AppSettings["EXbrand"].ToString();
            }
        }
        public static string EXstoreRef
        {
            get
            {
                return ConfigurationManager.AppSettings["EXstoreRef"].ToString();
            }
        }
        public static string EXprocessRef
        {
            get
            {
                return ConfigurationManager.AppSettings["EXprocessRef"].ToString();
            }
        }
        public static string EmmaBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaBaseUrl"].ToString();
            }
        }

        public static string EmmaTokenUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaTokenUrl"].ToString();
            }
        }

        public static string EmmaGrantType
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaGrantType"].ToString();
            }
        }

        public static string EmmaClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaClientId"].ToString();
            }
        }

        public static string EmmaClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaClientSecret"].ToString();
            }
        }

        public static string EmmaClientScope
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaClientScope"].ToString();
            }
        }

        public static string EmmaAccept
        {
            get
            {
                return ConfigurationManager.AppSettings["EmmaAccept"].ToString();
            }
        }

        //Added for APIGEE Mambu Headers
        public static string XCountry
        {
            get
            {
                return ConfigurationManager.AppSettings["X-country"].ToString();
            }
        }

        public static string Xbrand
        {
            get
            {
                return ConfigurationManager.AppSettings["X-brand"].ToString();
            }
        }

        public static string XstoreRef
        {
            get
            {
                return ConfigurationManager.AppSettings["X-storeRef"].ToString();
            }
        }

        public static string XuserTx
        {
            get
            {
                return ConfigurationManager.AppSettings["X-userTx"].ToString();
            }
        }

        public static string XchannelRef
        {
            get
            {
                return ConfigurationManager.AppSettings["X-channelRef"].ToString();
            }
        }

        public static string XconsumerRef
        {
            get
            {
                return ConfigurationManager.AppSettings["X-consumerRef"].ToString();
            }
        }

        public static string XtypeProcessRef
        {
            get
            {
                return ConfigurationManager.AppSettings["X-typeProcessRef"].ToString();
            }
        }

        public static string XtypeProduct
        {
            get
            {
                return ConfigurationManager.AppSettings["X-typeProduct"].ToString();
            }
        }

        public static string XtypeProductHP
        {
            get
            {
                return ConfigurationManager.AppSettings["X-typeProductHP"].ToString();
            }
        }

        public static string XtypeProductCL
        {
            get
            {
                return ConfigurationManager.AppSettings["X-typeProductCL"].ToString();
            }
        }

        public static string Xenvironment
        {
            get
            {
                return ConfigurationManager.AppSettings["X-environment"].ToString();
            }
        }

        public static string Schedule
        {
            get
            {
                return ConfigurationManager.AppSettings["Schedule"].ToString();
            }
        }

        public static string Payment
        {
            get
            {
                return ConfigurationManager.AppSettings["Payment"].ToString();
            }
        }

        public static string Payoff
        {
            get
            {
                return ConfigurationManager.AppSettings["Payoff"].ToString();
            }
        }

        public static string Reverse
        {
            get
            {
                return ConfigurationManager.AppSettings["Reverse"].ToString();
            }
        }
    }
}
