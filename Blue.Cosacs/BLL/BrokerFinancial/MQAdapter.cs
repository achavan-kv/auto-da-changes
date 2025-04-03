using System;
using IBM.WMQ;
using System.IO;
using System.Text;
using System.Configuration;
using STL.Common;
using STL.BLL;



namespace STL.BLL.BrokerFinancial
{

    public enum MessagingType
    {
        ASync,
        Sync
    }
    public class MQAdapter
    {
        // The name of the queue manager.
        private string QueManager;

        private string InQueName = "COSACS.REQ";
        // The name of the queue where you will be getting your messages.

        public string OutQueName;
        // The time you want a thread to wait on

        // an empty queue until a relevant message shows up.

        private int WaitInterval = 360000; // 60000 milliseconds = 60 seconds
        private MQQueueManager qMgr;
        private MQQueue InputQueue;
        private MQQueue OutputQueue;
        private MQMessage MqsMsg;
        private MQMessage retrievedMessage;
        private MQGetMessageOptions gmo;
        private String msgText;
        //  private UTF8Encoding utf8Enc;
        public MQAdapter(MessagingType Type)
        {
            MQQueueManager QMgr;
            if (Type == MessagingType.ASync)
            {
                try
                {
                    QMgr = new MQQueueManager(QueManager);
                }
                catch (MQException)
                {
                    // Add your MQSeries exception handling here 

                }
            }
        }

        public string SendRequestToMQ(string RequestStream, string QueueManager)
        {
            try
            {
                CommonObject Common = new CommonObject();
                Common.logMessage("Connecting to MQ2", "99999", System.Diagnostics.EventLogEntryType.Information);

                qMgr = new MQQueueManager(QueueManager);


                int InOpenOptions = MQC.MQOO_OUTPUT;
                // |                                   MQC.MQOO_FAIL_IF_QUIESCING;

                //int OutOpenOptions = MQC.MQOO_OUTPUT;
                //     int OutOpenOptions = MQC.MQOO_INPUT_AS_Q_DEF | 
                //                          MQC.MQOO_FAIL_IF_QUIESCING;

                InputQueue = qMgr.AccessQueue(InQueName, InOpenOptions);
                Common.logMessage("Connecting to MQ3", "99999", System.Diagnostics.EventLogEntryType.Information);
                // OutputQueue = qMgr.AccessQueue(OutQueName, OutOpenOptions);
                //Common.logMessage("Connecting to MQ4", "99999", System.Diagnostics.EventLogEntryType.Information);
                MqsMsg = new MQMessage();

                //MqsMsg.ReplyToQueueName = OutQueName;
                Common.logMessage("Connecting to MQ6", "99999", System.Diagnostics.EventLogEntryType.Information);

                MqsMsg.WriteBytes(RequestStream);

                Common.logMessage("Connecting to MQ7", "99999", System.Diagnostics.EventLogEntryType.Information);


                //Common.logMessage("Connecting to MQ7", "99999", System.Diagnostics.EventLogEntryType.Information);
                MQPutMessageOptions pmo = new MQPutMessageOptions();
                //Common.logMessage("Connecting to MQ8", "99999", System.Diagnostics.EventLogEntryType.Information);
                InputQueue.Put(MqsMsg, pmo);
            }
            catch (MQException ex)
            {
                // Add your MQSeries exception handling here
                throw new Exception("MQException Error connecting to MQ in End of Day in Code BrokerFinancial.MQAdapter " + ex.Message);
                // throw ex;

            }
            catch (Exception e)
            {
                // Add your exception handling here
                // throw new Exception("General Exception when attempting to access MQ"); 
                // to handle exceptions of types other than MQException
                throw new Exception("General Error connecting to MQ in End of Day in Code BrokerFinancial.MQAdapter " + e.Message);
            }
            try
            {
                CommonObject Common = new CommonObject();
                Common.logMessage("Waiting for MQ Response", "99999", System.Diagnostics.EventLogEntryType.Information);
                retrievedMessage = new MQMessage();

                Common.logMessage("Response MQ1", "99999", System.Diagnostics.EventLogEntryType.Information);

                retrievedMessage.CorrelationId = MqsMsg.MessageId;
                //Setting the get message options.. 

                Common.logMessage("Response MQ2", "99999", System.Diagnostics.EventLogEntryType.Information);
                gmo = new MQGetMessageOptions();
                // In order to activate "Wait on an empty queue";

                // you have to mask MQGetMessageOptions with MQGMO_WAIT 

                // for more details on other available options kindly refer to Appendix A

                Common.logMessage("Response MQ3", "99999", System.Diagnostics.EventLogEntryType.Information);
                gmo.Options = MQC.MQGMO_FAIL_IF_QUIESCING | MQC.MQGMO_WAIT;
                gmo.WaitInterval = WaitInterval;// wait time

                // to match using CorrelationID

                // or MQMO_MATCH_MSG_ID to match using MessageID

                Common.logMessage("Response MQ4", "99999", System.Diagnostics.EventLogEntryType.Information);
                gmo.MatchOptions = MQC.MQMO_MATCH_CORREL_ID;

                /*Common.logMessage("Response MQ5", "99999", System.Diagnostics.EventLogEntryType.Information);
                OutputQueue.Get(retrievedMessage, gmo);
                //writing Message result 

                Common.logMessage("Response MQ6", "99999", System.Diagnostics.EventLogEntryType.Information);
                msgText = retrievedMessage.ReadString(retrievedMessage.MessageLength);


                Common.logMessage("Response MQ7", "99999", System.Diagnostics.EventLogEntryType.Information);*/
                // TODO check if Xml - then its an Oracle error message
                /*BInterfaceError ie = new BInterfaceError(
                      null,
                      null,
                      "BrokerX",
                      0,
                      DateTime.Now,
                      "Broker Message received for information" + msgText,
                      "W"); */

            }
            catch (MQException ex)
            {
                // Add your MQSeries exception handling here 
                BInterfaceError ie = new BInterfaceError(
                      null,
                      null,
                      "BrokerX",
                      0,
                      DateTime.Now,
                      ex.Message + " ReasonCode:" + ex.ReasonCode.ToString(),
                      "W");

            }
            catch (Exception e)
            {
                // Add your exception handling here to
                // throw new Exception("Exception in End of Day -retrieve MQ Message Code BrokerFinancial.MQAdapter"); 

                // handle exceptions of types other than MQException
                BInterfaceError ie = new BInterfaceError(
                      null,
                      null,
                      "BrokerX",
                      0,
                      DateTime.Now,
                      e.Message,
                      "W");

            }
            try
            {
                //Close the queue 

                OutputQueue.Close();
                InputQueue.Close();
                //Disconnect from the queue manager 

                qMgr.Disconnect();
            }
            catch (MQException)
            {
                // Add your MQSeries exception handling here 
                //throw new Exception("MQException in End of Day -retrieve MQ Message Code BrokerFinancial.MQAdapter"); 

            }
            catch (Exception)
            {
                // Add your exception handling here

                // to handle exceptions of types other than MQException

            }
            return msgText;
        }
        // --------- See the comments one the code above --------//

        public Byte[] SendMessage(string Message)
        {
            int InOpenOptions;
            MQQueue InputQueue = null;
            MQMessage MqsMsg = null;

            MQPutMessageOptions Pmo;
            InOpenOptions = MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING;
            try
            {
                InputQueue = qMgr.AccessQueue(InQueName, InOpenOptions);
                MqsMsg = new MQMessage();
                MqsMsg.WriteBytes(Message);
                Pmo = new MQPutMessageOptions();
                InputQueue.Put(MqsMsg, Pmo);
            }
            catch (MQException)
            {
                // Add your MQSeries exception handling here

            }
            catch (System.Exception)
            {
                // Add your exception handling here

                // to handle exceptions of types other than MQException

            }
            finally
            {
                InputQueue.Close();
            }
            return MqsMsg.MessageId;
        }

        public string GetMessage(Byte[] MessageId)
        {
            int OutOpenOptions;
            MQQueue OutputQueue = null;
            MQMessage RetrievedMessage;
            MQGetMessageOptions Gmo;
            OutOpenOptions = MQC.MQOO_INPUT_AS_Q_DEF |
                             MQC.MQOO_FAIL_IF_QUIESCING;
            RetrievedMessage = new MQMessage();
            Gmo = new MQGetMessageOptions();
            Gmo.Options = MQC.MQGMO_FAIL_IF_QUIESCING |
                          MQC.MQGMO_WAIT;
            Gmo.WaitInterval = WaitInterval;
            Gmo.MatchOptions = MQC.MQMO_MATCH_CORREL_ID;
            try
            {
                OutputQueue = qMgr.AccessQueue(OutQueName, OutOpenOptions);
                RetrievedMessage.CorrelationId = MessageId;
                OutputQueue.Get(RetrievedMessage, Gmo);
            }
            catch (MQException)
            {
                // Add your MQSerie exception handling here 

            }
            catch (System.Exception)
            {
                // Add your exception handling here

                // to handle exceptions of types other than MQException

            }
            finally
            {
                OutputQueue.Close();
            }
            return RetrievedMessage.ReadString(RetrievedMessage.MessageLength);
        }
        ~MQAdapter()
        {
            try
            {
                qMgr.Close();
            }
            catch
            {
            }
        }
    }
}