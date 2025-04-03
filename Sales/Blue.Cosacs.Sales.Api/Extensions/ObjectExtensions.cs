using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Blue.Cosacs.Sales.Api.Extensions
{
    public static class ObjectExtensions
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, a);
                stream.Position = 0;

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}