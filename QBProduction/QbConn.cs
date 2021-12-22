using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.QBFC13;

namespace QBProduction
{
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
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message+ "\n" +ex.StackTrace);
            }
           
        }
        public void openConnection()
        {
            try
            {
                sessionManager.OpenConnection("", "Production App");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Please make sure quickbooks is open before running this application   \n"+ex.Message + "\n" + ex.StackTrace);
            }
            


            //Send the request and get the response from QuickBooks          

        }
        public void closeConnection()
        {
            sessionManager.EndSession();
            sessionBegun = false;
            sessionManager.CloseConnection();
            connectionOpen = false;
        }
        public void BuildTxnDelRq(string txid)
        {
            ITxnDel TxnDelRq = requestMsgSet.AppendTxnDelRq();
            //Set field value for TxnDelType
            TxnDelRq.TxnDelType.SetValue(ENTxnDelType.tdtBuildAssembly);
            //Set field value for TxnID
            TxnDelRq.TxnID.SetValue(txid);
        }
        public void BuildBuildAssemblyQueryRq(ENiterator numerate, int num, string iteratorrequest)
        {
            IBuildAssemblyQuery BuildAssemblyQueryRq = requestMsgSet.AppendBuildAssemblyQueryRq();
            //Set attributes
            //Set field value for metaData
            // 
            BuildAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
                                      BuildAssemblyQueryRq.ORBuildAssemblyQuery.BuildAssemblyFilter.MaxReturned.SetValue(1);
           // BuildAssemblyQueryRq.iterator.SetValue(ENiterator.itStart);
            
            
            if (num == 0)
            {
               // BuildAssemblyQueryRq.iteratorID.SetValue("088001");
                BuildAssemblyQueryRq.iterator.SetValue(numerate);
            
            }
            else
            {
               
                BuildAssemblyQueryRq.iteratorID.SetValue(iteratorrequest);
                BuildAssemblyQueryRq.iterator.SetValue(numerate);
            }
            //Set field value for iterator
            //BuildAssemblyQueryRq.iterator.SetValue(numtype);
            //BuildAssemblyQueryRq.ORBuildAssemblyQuery.BuildAssemblyFilter.MaxReturned.SetValue(1);
            //Set field value for IncludeComponentLineItems
           // BuildAssemblyQueryRq.IncludeRetElementList.Add("RefNumber");
            BuildAssemblyQueryRq.IncludeComponentLineItems.SetValue(true);
        }
        public void BuildBuildAssemblyQueryRqAll()
        {
            IBuildAssemblyQuery BuildAssemblyQueryRq = requestMsgSet.AppendBuildAssemblyQueryRq();
            //Set attributes
            //Set field value for metaData
            BuildAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            //Set field value for iterator
            //BuildAssemblyQueryRq.ORBuildAssemblyQuery.TxnIDList.Add(productionref);
            //BuildAssemblyQueryRq.iterator.SetValue(ENiterator.itStart);
            //BuildAssemblyQueryRq.ORBuildAssemblyQuery.BuildAssemblyFilter.MaxReturned.SetValue(1);
            BuildAssemblyQueryRq.IncludeRetElementList.Add("RefNumber");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("IsPending");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("TxnID");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("TxnDate");
            

            //Set field value for IncludeComponentLineItems
            BuildAssemblyQueryRq.IncludeComponentLineItems.SetValue(false);
        }
        public void BuildBuildAssemblyQueryRqByRef(string batchno)
        {
            IBuildAssemblyQuery BuildAssemblyQueryRq = requestMsgSet.AppendBuildAssemblyQueryRq();
            //Set attributes
            //Set field value for metaData
            BuildAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            //Set field value for iterator

           // BuildAssemblyQueryRq.iterator.SetValue(ENiterator.itStart);
            BuildAssemblyQueryRq.ORBuildAssemblyQuery.RefNumberList.Add(batchno);
            
            //BuildAssemblyQueryRq.ORBuildAssemblyQuery.BuildAssemblyFilter.MaxReturned.SetValue(1);
            BuildAssemblyQueryRq.IncludeRetElementList.Add("RefNumber");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("IsPending");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("TxnID");
            BuildAssemblyQueryRq.IncludeRetElementList.Add("TxnDate");


            //Set field value for IncludeComponentLineItems
            BuildAssemblyQueryRq.IncludeComponentLineItems.SetValue(false);
        }
        public void BuildBuildAssemblyQueryRqOne(string transid)
        {
            IBuildAssemblyQuery BuildAssemblyQueryRq = requestMsgSet.AppendBuildAssemblyQueryRq();
            //Set attributes
            //Set field value for metaData
            BuildAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            //Set field value for iterator
            //
            // BuildAssemblyQueryRq.iterator.SetValue(ENiterator.itStart);
            //BuildAssemblyQueryRq.ORBuildAssemblyQuery.BuildAssemblyFilter.MaxReturned.SetValue(1);
            BuildAssemblyQueryRq.ORBuildAssemblyQuery.TxnIDList.Add(transid);
            //Set field value for IncludeComponentLineItems
            BuildAssemblyQueryRq.IncludeComponentLineItems.SetValue(true);
        }
        public void BuildBuildAssemblyQueryCheckExists(string productionnumber)
        {
            //checks whether a production number exists
            IBuildAssemblyQuery BuildAssemblyQueryRq = requestMsgSet.AppendBuildAssemblyQueryRq();
            //Set attributes
            //Set field value for metaData
            BuildAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            //Set field value for iterator
            //
            // BuildAssemblyQueryRq.iterator.SetValue(ENiterator.itStart);
            BuildAssemblyQueryRq.ORBuildAssemblyQuery.RefNumberList.Add(productionnumber);
            BuildAssemblyQueryRq.IncludeRetElementList.Add("RefNumber");
            //Set field value for IncludeComponentLineItems
           
        }
        public void BuildItemAssembliesCanBuildQueryRq(string listid, DateTime dateofquery)
        {
            IItemAssembliesCanBuildQuery ItemAssembliesCanBuildQueryRq = requestMsgSet.AppendItemAssembliesCanBuildQueryRq();
            //Set field value for ListID
           
            ItemAssembliesCanBuildQueryRq.ItemInventoryAssemblyRef.ListID.SetValue(listid);
            //Set field value for FullName
            //ItemAssembliesCanBuildQueryRq.ItemInventoryAssemblyRef.FullName.SetValue("ab");
            //Set field value for TxnDate
            ItemAssembliesCanBuildQueryRq.TxnDate.SetValue(dateofquery);

        }
        public void BuildBuildAssemblyModRq(string editseq, string transdate, string txid,string transref, double qtytobuild, string memo)
        {
            IBuildAssemblyMod BuildAssemblyModRq = requestMsgSet.AppendBuildAssemblyModRq();
            BuildAssemblyModRq.TxnID.SetValue(txid);

            //Set field value for TxnID
            BuildAssemblyModRq.EditSequence.SetValue(editseq);
            
            //Set attribut
            //Set field value for TxnDate
            BuildAssemblyModRq.TxnDate.SetValue(DateTime.Parse(transdate));
            //Set field value for RefNumber
            BuildAssemblyModRq.RefNumber.SetValue(transref);
            //Set field value for Memo
            BuildAssemblyModRq.Memo.SetValue(memo);
            //Set field value for QuantityToBuild
            BuildAssemblyModRq.QuantityToBuild.SetValue(qtytobuild);
            //Set field value for MarkPendingIfRequired
            BuildAssemblyModRq.MarkPendingIfRequired.SetValue(true);
            //Set field value for RemovePending
            BuildAssemblyModRq.RemovePending.SetValue(true);
            //Set field value for IncludeRetElementList


        }
        public void BuildBuildAssemblyAddRq(string assemblyitemid, string assemblyitemname, string transdate, string transref,double qtytobuild ,string memo)
        {
            IBuildAssemblyAdd BuildAssemblyAddRq = requestMsgSet.AppendBuildAssemblyAddRq();
            //Set attributes
            BuildAssemblyAddRq.defMacro.SetValue("IQBStringType");
            //Set field value for ListID
            BuildAssemblyAddRq.ItemInventoryAssemblyRef.ListID.SetValue(assemblyitemid);
            //Set field value for FullName
            BuildAssemblyAddRq.ItemInventoryAssemblyRef.FullName.SetValue(assemblyitemname);
            //Set field value for FullName
          

            //Set field value for TxnDate
            BuildAssemblyAddRq.TxnDate.SetValue(DateTime.Parse(transdate));
            //Set field value for RefNumber
            BuildAssemblyAddRq.RefNumber.SetValue(transref);

            //Set field value for Memo
            BuildAssemblyAddRq.Memo.SetValue(memo);
            //Set field value for QuantityToBuild
            BuildAssemblyAddRq.QuantityToBuild.SetValue(qtytobuild);
            //Set field value for MarkPendingIfRequired
            BuildAssemblyAddRq.MarkPendingIfRequired.SetValue(true);
            //Set field value for ExternalGUID

            //Set field value for IncludeRetElementList
            //May create more than one of these if needed
           
        }
        public void BuildItemInventoryAssemblyQueryRq()
        {
            IItemInventoryAssemblyQuery ItemInventoryAssemblyQueryRq = requestMsgSet.AppendItemInventoryAssemblyQueryRq();
            //Set attributes
            ItemInventoryAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);

        }
        public void BuildItemInventoryAssemblyQueryRq(string listid)
        {
            IItemInventoryAssemblyQuery ItemInventoryAssemblyQueryRq = requestMsgSet.AppendItemInventoryAssemblyQueryRq();
            //Set attributes
            ItemInventoryAssemblyQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);

            ItemInventoryAssemblyQueryRq.ORListQueryWithOwnerIDAndClass.ListIDList.Add(listid);
        }
        public void BuildItemInventoryQueryRq(string invlistid)
        {
            IItemInventoryQuery ItemInventoryQueryRq = requestMsgSet.AppendItemInventoryQueryRq();
            ItemInventoryQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            ItemInventoryQueryRq.ORListQueryWithOwnerIDAndClass.ListIDList.Add(invlistid);
            
        }
       public void BuildAccountQueryRq(ENAccountType acctype)
        {
            IAccountQuery AccountQueryRq = requestMsgSet.AppendAccountQueryRq();
            //Set attributes
            //Set field value for metaData
            AccountQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);

            AccountQueryRq.ORAccountListQuery.AccountListFilter.AccountTypeList.Add(acctype);

            // AccountQueryRq.OwnerIDList.Add(Guid.NewGuid().ToString());
        }
        public void BuildItemInventoryAddRq(Dictionary<string, string> fieldvals)
        {
            IItemInventoryAdd ItemInventoryAddRq = requestMsgSet.AppendItemInventoryAddRq();
            //Set field value for Name
            ItemInventoryAddRq.Name.SetValue(fieldvals["name"]);
            //Set field value for IsActive
            ItemInventoryAddRq.IsActive.SetValue(true);

            //Set field value for ListID
            ItemInventoryAddRq.IncomeAccountRef.ListID.SetValue(fieldvals["incomeacc"]);
            //Set field value for PurchaseDesc
            ItemInventoryAddRq.PurchaseDesc.SetValue(fieldvals["desc"]);

            ItemInventoryAddRq.SalesDesc.SetValue(fieldvals["desc"]);
            //Set field value for Pur
            ItemInventoryAddRq.COGSAccountRef.ListID.SetValue(fieldvals["cogsacc"]);
            //ItemInventoryAddRq.AssetAccountRef.ListID.SetValue();
            ItemInventoryAddRq.AssetAccountRef.FullName.SetValue(fieldvals["assetsacc"]);
            ItemInventoryAddRq.PurchaseTaxCodeRef.FullName.SetValue("E");
            ItemInventoryAddRq.SalesTaxCodeRef.FullName.SetValue("E");


        }
        public void BuildUnitOfMeasureSetQueryRq()
        {

            IUnitOfMeasureSetQuery UnitOfMeasureSetQueryRq = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
            //Set attributes
            //Set field value for metaData
            UnitOfMeasureSetQueryRq.metaData.SetValue(ENmetaData.mdMetaDataAndResponseData);
            UnitOfMeasureSetQueryRq.ORListQuery.ListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);




        }

        public void getResponse()
        {
            this.responseMsgSet = sessionManager.DoRequests(this.requestMsgSet);
        }
        
        public T returnDataList<T>()
        {
            IResponseList acclist = this.responseMsgSet.ResponseList;
            IResponse rs1 = acclist.GetAt(0);
            T ft = (T)rs1.Detail;

            return ft;
        }
        public bool responseIsValid<T>(T gresponsetype)
        {
            if (responseMsgSet == null) return false;
            IResponseList responseList = responseMsgSet.ResponseList;
            //if we sent only one request, there is only one response, we'll walk the list for this sample
            if (responseList != null)
            {
                //for (int i = 0; i < responseList.Count; i++)
                //{
                IResponse response = responseList.GetAt(0);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType.Equals(gresponsetype))
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            return true;

                        }
                    }
                }
                //}
            }
            return false;
        }



    }
}
