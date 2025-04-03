using System;

namespace Blue.Cosacs.Merchandising.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldTitleAttribute : Attribute
    {
        public FieldTitleAttribute()
        {
        }

        public FieldTitleAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}