using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public sealed class StoreCardCardStatus_Lookup
    {
        private StoreCardCardStatus_Lookup(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }
           

        public string Code { get; private set; }
        public string Description { get; private set; }

        // These status haven't been validated. Still subject to revision
        public static readonly StoreCardCardStatus_Lookup Active = new StoreCardCardStatus_Lookup("A", "Active");
        public static readonly StoreCardCardStatus_Lookup AwaitingActivation = new StoreCardCardStatus_Lookup("AA", "Awaiting Activation");
        public static readonly StoreCardCardStatus_Lookup Cancelled = new StoreCardCardStatus_Lookup("C", "Cancelled");
        public static readonly StoreCardCardStatus_Lookup CardToBeIssued = new StoreCardCardStatus_Lookup("TBI", "Card To Be Issued");
        public static readonly StoreCardCardStatus_Lookup Unknown = new StoreCardCardStatus_Lookup("U", "");


        public override bool Equals(object obj)
        {
            if (obj is StoreCardCardStatus_Lookup)
                return this.Code == ((StoreCardCardStatus_Lookup)obj).Code;
            else if (obj is string) 
                return this.Code == ((string)obj);
            else //if (obj == null)
             //   throw new ArgumentNullException("obj");
            return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static StoreCardCardStatus_Lookup FromString(string code)
        {
           
                foreach (var status in AsEnumerable())
                {
                    if (status.Equals(code))
                        return status;
                }

                return StoreCardCardStatus_Lookup.Unknown;
            
        } 

        public static IEnumerable<StoreCardCardStatus_Lookup> AsEnumerable()
        {
            yield return Unknown;
            yield return Active;
            yield return AwaitingActivation;
            yield return Cancelled;
            yield return CardToBeIssued;
      
        }
    }

   
}
