namespace Blue.Cosacs.Test.Mocks
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Xml;
    using Blue.Hub.Client;
    using Blue.Transactions;

    public class MockGenericPublisher<T> : IPublisher where T : class
    {
        public string Routing { get; set; }
        public List<XmlReader> Messages { get; set; }
        public List<T> GenericMessages { get; set; }

        public MockGenericPublisher()
        {
            Messages = new List<XmlReader>();
            GenericMessages = new List<T>();
        } 

        public int Publish<Q>(string routing, Q message, string correlationId = null, SqlConnection c = null, SqlTransaction t = null)
        {
            Routing = routing;
            GenericMessages.Add(message as T);
            return 1;
        }

        public int Publish(string routing, XmlReader message, string correlationId = null, SqlConnection c = null, SqlTransaction t = null)
        {
            Routing = routing;
            Messages.Add(message);
            return 1;
        }

        public int Publish<C, Q>(string routing, Q message, string correlationId = null) where C : DbContextBase, new()
        {
            Routing = routing;
            GenericMessages.Add(message as T);
            return 1;
        }

        public void ReprocessQueue(int queueId)
        {
        }

        public void ReprocessQueueMessage(int queueId, int messageId)
        {
        }

        public List<Blue.Hub.Client.MessageProgress> GetMessagesProgressStatus(List<int> messageIds)
        {
            return null;
        }

        public MessageProgressStatus GetMessageProgressStatus(int messageId)
        {
            throw new System.NotImplementedException();
        }
    }
}
