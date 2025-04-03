using System.Collections.Generic;
using Blue.Hub.Client;

namespace Blue.Cosacs.Test.Mocks
{
    class MockHubClientPublisher : IPublisher
    {
        public int Publish<C, T>(string routing, T message, string correlationId = null) where C : Transactions.DbContextBase, new()
        {
            return 0;
        }

        public int Publish(string routing, System.Xml.XmlReader message, string correlationId = null, System.Data.SqlClient.SqlConnection c = null, System.Data.SqlClient.SqlTransaction t = null)
        {
            return 0;
        }

        public int Publish<T>(string routing, T message, string correlationId = null, System.Data.SqlClient.SqlConnection c = null, System.Data.SqlClient.SqlTransaction t = null)
        {
            return 0;
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
