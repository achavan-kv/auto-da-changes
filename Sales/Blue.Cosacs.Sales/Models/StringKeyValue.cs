using System;

namespace Blue.Cosacs.Sales.Models
{
    public class StringKeyValue : IEquatable<StringKeyValue>
    {
        public StringKeyValue()
        {
        }

        public StringKeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public bool Equals(StringKeyValue other)
        {
            return Key == other.Key && Value == other.Value;
        }
    }
}
