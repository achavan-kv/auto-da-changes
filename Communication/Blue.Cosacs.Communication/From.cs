using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Communication.Messages
{
    public partial class From
    {

        public override int GetHashCode()
        {
            return string.Format("Blue.Cosacs.Communication.Messages.From {0}", this.FromMail).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is From && obj != null)
            {
                return this.Equals((From)obj);
            }

            return false;
        }

        public bool Equals(From to)
        {
            return string.Compare(this.FromMail, to.FromMail, true) == 0 &&
                   string.Compare(this.FromName, to.FromName, true) == 0;
        }

        public static bool operator !=(From x, From y)
        {
            return !(x == y);
        }

        public static bool operator ==(From x, From y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
            {
                return true;
            }
            
            return !(object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) &&  x.Equals(y);
        }
    }
}
