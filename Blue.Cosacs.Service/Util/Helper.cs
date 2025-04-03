using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace Blue.Cosacs.Web.Helpers
{
    public class Helper
    {
        public List<Tuple<string,string>> GetDifference(object @new, object old)
        {
            if (@new.GetType() != old.GetType())
                throw new Exception("Objects are not same type");

            var list = new List<Tuple<string, string>>();
            GetAllProps(@new, old, @new.GetType(), list);
            return list;
        }

        private List<Tuple<string,string>> GetAllProps(object @new, object old, Type baseType, List<Tuple<string,string>> list)
        {
            var props = baseType.GetProperties();
            foreach (var propertyInfo in props)
            {
                if (propertyInfo.CanRead)
                {
                    //test is a class
                    if (!propertyInfo.PropertyType.IsPrimitive 
                        && !propertyInfo.PropertyType.Equals(typeof(string)) 
                        && !propertyInfo.PropertyType.Equals(typeof(decimal))
                        && !propertyInfo.PropertyType.Equals(typeof(DateTime))
                        && IsNullable(propertyInfo.PropertyType))
                    {
                        var ptype = propertyInfo.PropertyType.GetInterfaces().FirstOrDefault(p => p == typeof(IEnumerable));
                        if (ptype != null)
                        {
                            var e = ((IEnumerable)propertyInfo.GetValue(@new, null));
                            foreach( var i in e)
                                GetAllProps(@new, old, i.GetType(), list); 
                        }
                        else
                        {
                            GetAllProps(@new, old, propertyInfo.PropertyType, list);
                        }
                    }
                    else
                    {
                        var newValue = propertyInfo.GetValue(@new, null);
                        var oldValue = propertyInfo.GetValue(old, null);
                        if (!object.Equals(newValue, oldValue))
                        {
                            list.Add(new Tuple<string, string>(propertyInfo.Name, newValue.ToString()));
                        }
                    }
                }
            }
            return list;
        }

        public bool IsNullable<T>(T propertyvalue)
        {
            Type fieldType = Nullable.GetUnderlyingType(typeof(T));
            if (object.ReferenceEquals(fieldType, typeof(bool)))
                return true;
            return false;
        }
    }
}