using System;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class SalesSettingsController : ApiController
    {
        public dynamic Get(string settingName="")
        {
            dynamic result;

            if (string.IsNullOrEmpty(settingName))
            {
                result = new Settings();
            }
            else
            {
                result = GetSettings(settingName);
            }

            return result;
        }

        private dynamic GetSettings(string settingName)
        {
                        var setting = new Settings();
            var result = setting.GetType().GetProperty(settingName).GetValue(setting);

            return result;
        }
    }
}