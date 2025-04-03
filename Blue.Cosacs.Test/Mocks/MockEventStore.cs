using Blue.Events;

namespace Blue.Cosacs.Test.Mocks
{
    public class MockEventStore : IEventStore
    {
        public void Log(System.Data.SqlClient.SqlTransaction transaction, object @event, string type = null, string category = null, object indexes = null)
        {
        }

        public void Log(System.Data.SqlClient.SqlConnection connection, object @event, string type = null, string category = null, object indexes = null)
        {
        }

        public void Log(object @event, string type = null, string category = null, object indexes = null)
        {
        }

        public void LogAsync(object @event, string type = null, string category = null, object indexes = null)
        {
        }
    }
}
