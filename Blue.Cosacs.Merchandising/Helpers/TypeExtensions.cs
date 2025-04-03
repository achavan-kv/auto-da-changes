namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> TypeCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();

        public static List<PropertyInfo> GetModelProperties(this Type type)
        {
            return TypeCache.GetOrAdd(type, GetProps(type));
        }

        private static List<PropertyInfo> GetProps(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite
                    && p.CanRead
                    && p.GetSetMethod(false) != null
                    && p.GetGetMethod(false) != null)
                .ToList();
        }

        public static bool IsSubclassOfRawGeneric(this Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
