using System.IO;

namespace Blue.Cosacs.Warranty.Extensions
{
    public static class Data
    {
        public static string ToJson(this object o)
        {
            var sb = new System.Text.StringBuilder();
            new Newtonsoft.Json.JsonSerializer().Serialize(new StringWriter(sb), o);
            return sb.ToString();
        }
    }
}
