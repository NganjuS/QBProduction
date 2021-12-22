using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Interop.QBFC13;

namespace QBProduction
{
    class Controller
    {
       
        public static string ReturnConnection()
        {
            string filename = @"C:\QBProd\dbsettings.inf";
            string servername = "";
            string userid = "";
            string pwd = "";
            string dbname = "";
            if (!File.Exists(filename))
                return "";
            using (StreamReader streamReader = File.OpenText(filename))
            {
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    string[] strArray = str.Split(':');
                    if (strArray[0] == "server")
                        servername = strArray[1];
                    else if (strArray[0] == "uid")
                        userid = strArray[1];
                    else if (strArray[0] == "pwd")
                        pwd = strArray[1];
                    else if (strArray[0] == "database")
                        dbname = strArray[1];
                }
            }
            return $"server={servername};uid={userid};pwd={pwd};database={dbname}";

        }




        public static T GetOneRecord<T>()
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                    var stksettings = session.Query<T>().FirstOrDefault();
                    if (stksettings != null)
                    {
                        return stksettings;
                    }
                    else
                    {
                        return (T)Activator.CreateInstance(typeof(T));
                    }

                }


            }

        }
        public static BomRun GetBomRun(string transactionid)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                try
                {
                    BomRun tx = session.Query<BomRun>().Where(c => c.transactionid == transactionid).First();
                    return tx;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error getting document");
                    return new BomRun();
                }
                
            }

        }
        public static IList<BomRunItems> GetBomRunItems(BomRun transactionid)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                IList<BomRunItems> tx = session.Query<BomRunItems>().Where(c => c.bomid == transactionid).ToList();
                return tx;
            }

        }
        public static List<T> GetData<T>()
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                    var stksettings = session.Query<T>().ToList();
                    if (stksettings != null)
                    {
                        return stksettings;
                    }
                    else
                    {
                        return new List<T> { (T)Activator.CreateInstance(typeof(T)) };
                    }

                }


            }

        }
        public static IList<object[]> GetBoms()
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                IList<object[]> boms = session.QueryOver<Boms>()
                    .SelectList(list => list.Select(p => p.bomname).Select(p => p.assemblyitem).Select(p => p.createdon)).List<object[]>();
                return boms;
            }


        }
        public static IList<object[]> GetBomsMass()
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                IList<object[]> boms = session.QueryOver<Boms>()
                    .SelectList(list => list.Select(p => p.assemblyitem).Select(p => p.uom).Select(p => p.assemblylistid)).List<object[]>();
                return boms;
            }


        }
        public static IList<object[]> GetBomLines(BomRun bm)
        {

            using (var session = NHibernateHelper.OpenSession())
            {
                IList<object[]> boms = session.QueryOver<BomRunItems>()
                    .Where(p => p.bomid == bm)
                    .SelectList(list => list
                    .Select(p => p.product)
                    .Select(p => p.uom)
                    .Select(p => p.cost)
                    .Select(p => p.qtyavailable)
                    .Select(p => p.qtyperunit)
                    .Select(p => p.valueperunit)
                    .Select(p => p.totalqtyused)
                    .Select(p => p.totalvalue))
                    .List<object[]>();
                return boms;
            }



        }

        public static void SaveData<T>(T objtosave)
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(objtosave);
                    transaction.Commit();

                }


            }

        }
        public static T ReturnSaveData<T>(T objtosave)
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(objtosave);
                    transaction.Commit();

                }


            }
            return objtosave;

        }
        public static void DeleteData<T>(T objtodel)
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(objtodel);
                    transaction.Commit();

                }


            }

        }


        public void tt()
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            try
            {
                //Create the session Manager object
                sessionManager = new QBSessionManager();

                //Create the message set request object to hold our request
                

                //Connect to QuickBooks and begin a session
                sessionManager.OpenConnection("", "Production Module");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;
                
                IMsgSetRequest requestMsgSet = getLatestMsgSetRequest(sessionManager);  //sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                IItemInventoryAssemblyQuery getassemblies = requestMsgSet.AppendItemInventoryAssemblyQueryRq();
                //Send the request and get the response from QuickBooks
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse respnce = responseMsgSet.ResponseList.GetAt(0); 
                //End the session and close the connection to QuickBooks
                sessionManager.EndSession();
                sessionBegun = false;
                sessionManager.CloseConnection();
                connectionOpen = false;
      
                //string responseXML = responseMsgSet.ToXMLString();
                //MessageBox.Show(responseXML);
                //MessageBox.Show(respnce.retCount.ToString());
                //IItemInventoryAssemblyLineList lst = (IItemInventoryAssemblyLineList)respnce.Detail;




            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
        }
        private double QBFCLatestVersion(QBSessionManager SessionManager)
        {
            // Use oldest version to ensure that this application work with any QuickBooks (US)
            IMsgSetRequest msgset = SessionManager.CreateMsgSetRequest("US", 1, 0);
            msgset.AppendHostQueryRq();
            IMsgSetResponse QueryResponse = SessionManager.DoRequests(msgset);
            //MessageBox.Show("Host query = " + msgset.ToXMLString());
            //SaveXML(msgset.ToXMLString());


            // The response list contains only one response,
            // which corresponds to our single HostQuery request
            IResponse response = QueryResponse.ResponseList.GetAt(0);

            // Please refer to QBFC Developers Guide for details on why 
            // "as" clause was used to link this derrived class to its base class
            IHostRet HostResponse = response.Detail as IHostRet;
            IBSTRList supportedVersions = HostResponse.SupportedQBXMLVersionList as IBSTRList;

            int i;
            double vers;
            double LastVers = 0;
            string svers = null;

            for (i = 0; i <= supportedVersions.Count - 1; i++)
            {
                svers = supportedVersions.GetAt(i);
                vers = Convert.ToDouble(svers);
                if (vers > LastVers)
                {
                    LastVers = vers;
                }
            }
            return LastVers;
        }
        public IMsgSetRequest getLatestMsgSetRequest(QBSessionManager sessionManager)
        {
            // Find and adapt to supported version of QuickBooks
            double supportedVersion = QBFCLatestVersion(sessionManager);

            short qbXMLMajorVer = 0;
            short qbXMLMinorVer = 0;

            if (supportedVersion >= 6.0)
            {
                qbXMLMajorVer = 6;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 5.0)
            {
                qbXMLMajorVer = 5;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 4.0)
            {
                qbXMLMajorVer = 4;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 3.0)
            {
                qbXMLMajorVer = 3;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 2.0)
            {
                qbXMLMajorVer = 2;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 1.1)
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 1;
            }
            else
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 0;
                MessageBox.Show("It seems that you are running QuickBooks 2002 Release 1. We strongly recommend that you use QuickBooks' online update feature to obtain the latest fixes and enhancements");
            }

            // Create the message set request object
            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
            return requestMsgSet;
        }
        
    }
}
