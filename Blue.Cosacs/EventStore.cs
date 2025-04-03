using System.Threading;

namespace Blue.Cosacs
{
    public class EventStore : Blue.Events.EventStoreBase
    {
        private EventStore() : base("Default") { }

        protected override string EventBy
        {
            get { return Thread.CurrentPrincipal.Identity.Name == "" || Thread.CurrentPrincipal.Identity.Name == null ? STL.Common.Static.Credential.Name : Thread.CurrentPrincipal.Identity.Name; }  //#12200
           
        }

        public static readonly Blue.Events.IEventStore Instance = new EventStore();
    }
}
