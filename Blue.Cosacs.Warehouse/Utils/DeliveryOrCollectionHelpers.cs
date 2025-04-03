using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Warehouse.Utils
{
    // omg.... what utter nonsense
    public sealed class DeliveryOrCollection
    {
        public string Name { get; set; }
        public string Code { get; set; }

        private DeliveryOrCollection(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public static readonly DeliveryOrCollection Delivery = new DeliveryOrCollection("Delivery", "D");
        public static readonly DeliveryOrCollection Redelivery = new DeliveryOrCollection("Redelivery", "R");
        public static readonly DeliveryOrCollection Collection = new DeliveryOrCollection("Collection", "C");
        public static readonly DeliveryOrCollection Allocation = new DeliveryOrCollection("Allocation", "A");
        public static readonly DeliveryOrCollection Requisition = new DeliveryOrCollection("Requisition", "Q");
        public static readonly DeliveryOrCollection Transfer = new DeliveryOrCollection("Transfer", "T");

        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;

            var value = obj as DeliveryOrCollection;

            if (value != null)
                return this.Equals(value);

            return false;
        }

        public static bool operator ==(DeliveryOrCollection a, DeliveryOrCollection b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(DeliveryOrCollection a, DeliveryOrCollection b)
        {
            return !(a == b);
        }

        public bool Equals(DeliveryOrCollection value)
        {
            return string.Compare(value.Code, this.Code, true) == 0
                && string.Compare(value.Name, this.Name, true) == 0;
        }

        public override int GetHashCode()
        {
            return string.Format("DeliveryOrCollection {0}-{1}", this.Code, this.Name).GetHashCode();
        }

        public static DeliveryOrCollection FromString(string code)
        {
            return AsEnumerable().FirstOrDefault(p => string.Compare(p.Code, code, true) == 0);
        }

        public static IEnumerable<DeliveryOrCollection> AsEnumerable()
        {
            yield return Delivery;
            yield return Collection;
            yield return Redelivery;
            yield return Allocation;
            yield return Requisition;
            yield return Transfer;
        }

        public static IEnumerable<string> MerchandisingTypes()
        {
            yield return Allocation.Code;
            yield return Requisition.Code;
            yield return Transfer.Code;
        }
    }

}
