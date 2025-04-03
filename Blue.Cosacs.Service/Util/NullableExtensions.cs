
namespace Blue.Cosacs.Service.Util
{
   public static class NullableExtensions
    {

       public static int ValueOrDefault(this int? input, int defaultVal = 0)
       {
           return input.HasValue ? input.Value : defaultVal;
       }
    }
}
