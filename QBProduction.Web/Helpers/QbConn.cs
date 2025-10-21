using System;
using Interop.QBFC13;

namespace QBProduction.Web.Helpers
{
    /// <summary>
    /// QuickBooks Connection Helper for Web Application
    /// Note: QuickBooks SDK integration in a web context requires careful consideration
    /// of session management and may require a Windows service or desktop connector
    /// </summary>
    public class QbConn
    {
        public IMsgSetResponse responseMsgSet { get; set; }
        private bool sessionBegun { get; set; }
        private bool connectionOpen { get; set; }
        private QBSessionManager sessionManager { get; set; }
        public IMsgSetRequest requestMsgSet { get; set; }

        public void createConnection()
        {
            try
            {
                sessionBegun = false;
                connectionOpen = false;

                sessionManager = new QBSessionManager();

                //Create the message set request object to hold our request
                requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create QuickBooks connection: " + ex.Message, ex);
            }
        }

        public void openConnection()
        {
            try
            {
                sessionManager.OpenConnection("", "Production App Web");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to open QuickBooks connection. Please ensure QuickBooks is running: " + ex.Message, ex);
            }
        }

        public void closeConnection()
        {
            if (sessionBegun)
            {
                sessionManager.EndSession();
                sessionBegun = false;
            }
            if (connectionOpen)
            {
                sessionManager.CloseConnection();
                connectionOpen = false;
            }
        }

        public void BuildTxnDelRq(string txid)
        {
            ITxnDel TxnDelRq = requestMsgSet.AppendTxnDelRq();
            TxnDelRq.TxnDelType.SetValue(ENTxnDelType.tdtBuildAssembly);
            TxnDelRq.TxnID.SetValue(txid);
        }

        // Add other QuickBooks methods as needed
        // Note: Web applications should use these methods carefully,
        // possibly through a background service or API
    }
}
