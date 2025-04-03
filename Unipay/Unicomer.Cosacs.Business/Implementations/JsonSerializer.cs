using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Interfaces;

namespace Unicomer.Cosacs.Business.Implementations
{
    public class JsonSerializer: IJsonSerializer
    {
        public string Serialize(object value)
        {
            if (value == null)
                return "";
            if(value is string)
                return value.ToString();

            
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}
