using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Shared.ComponentModel
{
    //public class ConventionTypeDescriptionProvider : TypeDescriptionProvider
    //{
    //    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
    //    {
    //        var td = base.GetTypeDescriptor(objectType, instance);
    //        td = new ConventionTypeDescriptor(td);
    //        return td;
    //    }
    //}

    public class ConventionTypeDescriptor : CustomTypeDescriptor
    {
        public ConventionTypeDescriptor() //ICustomTypeDescriptor parent) : base(parent)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            var cols = base.GetProperties();
            var newCols = new PropertyDescriptor[cols.Count];

            for (var i = 0; i < cols.Count; i++)
                newCols[i] = new MyPropertyDescriptor(cols[i]);

            //var studentPD = cols["StudentInfo"];
            //var studentPDChildProperties = studentPD.GetChildProperties();
            //PropertyDescriptor[] array = new PropertyDescriptor[2];
            //array[0] = new SubPropertyDescriptor(studentPD, studentPDChildProperties["Name"], "Student Name");
            //array[1] = cols["Score"];
            return new PropertyDescriptorCollection(newCols);
        }

        private class MyPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor pd;

            public MyPropertyDescriptor(PropertyDescriptor descriptor) : base(descriptor)
            {
                this.pd = descriptor;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return pd.ShouldSerializeValue(component);
            }

            public override void SetValue(object component, object value)
            {
                pd.SetValue(component, value);
            }

            public override void ResetValue(object component)
            {
                pd.ResetValue(component);
            }

            public override Type PropertyType
            {
                get { return pd.PropertyType; }
            }

            public override bool IsReadOnly
            {
                get { return pd.IsReadOnly; }
            }

            public override object GetValue(object component)
            {
                return pd.GetValue(component);
            }

            public override Type ComponentType
            {
                get { return pd.ComponentType; }
            }

            public override bool CanResetValue(object component)
            {
                return pd.CanResetValue(component);
            }

            public override string DisplayName
            {
                get
                {
                    var name = base.DisplayName;
                    return SplitCamelCase(name);
                }
            }

            private static string SplitCamelCase(string str)
            {
                if (String.IsNullOrEmpty(str))
                    return "";
                string tempStr = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
                if (Char.IsLower(tempStr[0]) && tempStr.Length > 1)
                    tempStr = Char.ToUpper(tempStr[0]) + tempStr.Substring(1);

                return tempStr;
            }
        }
    }
}
