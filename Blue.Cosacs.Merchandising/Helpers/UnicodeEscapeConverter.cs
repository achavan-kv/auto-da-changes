using System.Text;

using FileHelpers;

namespace Blue.Cosacs.Merchandising.Helpers
{
    public class UnicodeEscapeConverter : ConverterBase
    {
        public override object StringToField(string @from)
        {
            return @from;
        }

        public override string FieldToString(object @from)
        {
            var str = @from as string;

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (char c in @from as string)
            {
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}