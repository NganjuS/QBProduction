using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
   
    public partial class Production : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker loadingworker = new BackgroundWorker();
        private readonly BackgroundWorker setproductsworker = new BackgroundWorker();
        private readonly BackgroundWorker produceworker = new BackgroundWorker();
        DataTable assemdttble;
        DataSet prodgroup;
        Interop.QBFC13.IItemInventoryAssemblyRetList assemlist;
        DataTable assemdttbleitems;
        public static string selectedbatchno = "";
        public static string defaultbatchcode;
        public static DataTable massproddatatable;
        public static int currentbatchno;
        public static string currenttransbom;
        public static string editseq;
        int iteratoridnum = 0;
        public static bool IsMassProduce;
        public static string[][] getproductstoproduce;
      
        public Production()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.cmbbomlist.IsEnabled = false;
            this.btnNew.IsEnabled = false;
            //load 
            loadingworker.DoWork += loadworker_DoWork; 
            loadingworker.RunWorkerCompleted += loadworker_RunWorkerCompleted;

            this.lblShowProgress.Content = "Initializing Application ... ";
            this.pgbItemGroup.IsIndeterminate = true;
            loadingworker.RunWorkerAsync();

            //
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            //initialize product grid
            setproductsworker.DoWork += setproductsworker_DoWork;
            setproductsworker.RunWorkerCompleted += setproductsworker_RunWorkerCompleted;
            //
            produceworker.DoWork += producework_DoWork;
            produceworker.RunWorkerCompleted += productioncomplete_RunWorkerCompleted;


        }
        public string ReturnProductionNumber()
        {
            //load production number
            BomSettings st = Controller.GetOneRecord<BomSettings>();
            if (st.bomrefno == 0)
            {
                st.bomrefno = currentbatchno = 1;
                st.bomcode = defaultbatchcode ="PROD";
                Controller.SaveData<BomSettings>(st);
                return st.bomcode + currentbatchno.ToString().PadLeft(4, '0');
            }
            else
            {
                currentbatchno = st.bomrefno;
                defaultbatchcode = st.bomcode;
                 return st.bomcode + currentbatchno.ToString().PadLeft(4, '0');
            }
        }
        public void InitProductGrid()
        {
            assemdttbleitems = null;
            assemdttbleitems = new System.Data.DataTable("#assemlylistitems");
                assemdttbleitems.Columns.Add("Product", typeof(string));
                assemdttbleitems.Columns.Add("Unit of Measure", typeof(string));
                assemdttbleitems.Columns.Add("Cost", typeof(decimal));
                assemdttbleitems.Columns.Add("Qty Available", typeof(decimal));
                assemdttbleitems.Columns.Add("Qty Needed Per Unit", typeof(decimal));
                assemdttbleitems.Columns.Add("Value Per Unit", typeof(decimal));

                //get selected batch item lines
                QbConn getlines = new QbConn();
                getlines.createConnection();
                getlines.openConnection();
                this.Dispatcher.Invoke(() =>
                {
                    getlines.BuildItemInventoryAssemblyQueryRq(this.cmbbomlist.SelectedValue.ToString());
                });
                
                getlines.getResponse();
                getlines.closeConnection();

                

               if (getlines.responseMsgSet.ResponseList == null || getlines.responseMsgSet.ResponseList.Count <= 0)
                        return;
                    Interop.QBFC13.IItemInventoryAssemblyRetList assemblyitem = (Interop.QBFC13.IItemInventoryAssemblyRetList)getlines.responseMsgSet.ResponseList.GetAt(0).Detail;
           // MessageBox.Show(getlines.responseMsgSet.ToXMLString());
                    if (this.assemdttble != null && assemblyitem.Count > 0 && this.assemdttble.Rows.Contains(assemblyitem.GetAt(0).ListID.GetValue()))
                    {
                        Interop.QBFC13.IItemInventoryAssemblyLineList lineitems = assemblyitem.GetAt(0).ItemInventoryAssemblyLineList;
                        
                        for (int j = 0; j < lineitems.Count; j++)
                        {
                            Interop.QBFC13.IItemInventoryAssemblyLine lineitem = lineitems.GetAt(j);
                                if (lineitem.ItemInventoryRef == null)
                                    continue;
                            QbConn getlineitems = new QbConn();
                            getlineitems.createConnection();
                            getlineitems.openConnection();
                    //MessageBox.Show(lineitem.ItemInventoryRef.ListID.GetValue());
                            getlineitems.BuildItemInventoryQueryRq(lineitem.ItemInventoryRef.ListID.GetValue());
                            getlineitems.getResponse();
                            getlineitems.closeConnection();

                            Interop.QBFC13.IItemInventoryRetList invitemlist = getlineitems.returnDataList<Interop.QBFC13.IItemInventoryRetList>();
                            if (invitemlist.Count > 0)
                            {
                                Interop.QBFC13.IItemInventoryRet invitem = invitemlist.GetAt(0);
                        // invitem.UnitOfMeasureSetRef== null ? invitem.UnitOfMeasureSetRef.FullName.GetValue() : "Default"
                            if(invitem.UnitOfMeasureSetRef == null)
                            {
                                assemdttbleitems.Rows.Add(invitem.Name.GetValue(), "Default", invitem.AverageCost.GetValue(), invitem.QuantityOnHand.GetValue(), lineitem.Quantity.GetValue(), invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue());
                            }
                            else
                            {
                                assemdttbleitems.Rows.Add(invitem.Name.GetValue(), invitem.UnitOfMeasureSetRef.FullName.GetValue(), invitem.AverageCost.GetValue(), invitem.QuantityOnHand.GetValue(), lineitem.Quantity.GetValue(), invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue());
                            }
                        
                            }
                        }
                    
                }
                
            
           
        }
        public DataTable ReturnAssemblyHeader(Interop.QBFC13.IBuildAssemblyRet  batch )
        {

           return new DataTable();
        }
        public DataTable ReturnAssemblyDetail(Interop.QBFC13.IComponentItemLineRet batchlines)
        {

            return new DataTable();
        }
        public void OnClickNextOrPrev()
        {
            /*
            QbConn getlineitems = new QbConn();
            getlineitems.createConnection();
            getlineitems.openConnection();
            getlineitems.BuildItemInventoryQueryRq(lineitem.ItemInventoryRef.ListID.GetValue());
            getlineitems.getResponse();
            getlineitems.closeConnection();*/
        }
        public bool ValidateProduction()
        {
            return false;
        }

        private void cmbbomlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.txtItemCost.Visibility = Visibility.Visible;
            this.lblProdCost.Visibility = Visibility.Visible;
            this.txtqtytoprod.IsEnabled = true;

            if (this.cmbbomlist.SelectedIndex != -1)

                    this.btnProcessProduction.IsEnabled = true;
            if (assemdttble != null && assemdttble.Rows.Count > 0)
            {
                DataRow[] listrow = assemdttble.Select(String.Format("listid = '{0}'", this.cmbbomlist.SelectedValue.ToString()));
                
                if (listrow.Length > 0)
                {
                    this.lblproditem.Content = listrow[0]["bom"].ToString();
                    this.lblproduom.Content = listrow[0]["uom"].ToString();
                    this.lblBatchStatus.Content = "Open";

                }
                this.pgbItemGroup.IsIndeterminate = true;
                this.lblShowProgress.Content = "Loading production item list ...";
                setproductsworker.RunWorkerAsync();
                this.SetCanBuildQty(this.cmbbomlist.SelectedValue.ToString(), this.dtproddate.DisplayDate);
                IsMassProduce = false;
                this.txtItemCost.Visibility = Visibility.Visible;
                this.lblProdCost.Visibility = Visibility.Visible;
                this.dgbomlist.Items.Clear();

            }
            
        }
        
        public void SetCanBuildQty(string listid, DateTime dttime)
        {
            QbConn tcon = new QbConn();
            tcon.createConnection();
            tcon.openConnection();
            tcon.BuildItemAssembliesCanBuildQueryRq(listid, dttime);
            tcon.getResponse();
            tcon.closeConnection();
            Interop.QBFC13.IItemAssembliesCanBuildRet rval = tcon.returnDataList<Interop.QBFC13.IItemAssembliesCanBuildRet>();
            this.txtMaxCanBuild.Text =  rval.QuantityCanBuild.GetValue().ToString();

            
        }

        private void txtqtytoprod_KeyUp(object sender, KeyEventArgs e)
        {
           
            if (this.dgproductionitems.Items.Count > 0)
            {
                if (this.txtqtytoprod.Text.Trim() == "")
                {
                    this.txttotalprodval.Text = "";
                    this.txtItemCost.Text = "";
                    return;
                   
                }
                SetFooterTotals(true);
                    
                
            }
            
        }
        public void SetFooterTotals(bool addcols)
        {
            double total = 0;
            double totalvalue = 0;
            if(IsMassProduce)
            {
                foreach (System.Data.DataRowView rowvw in this.dgproductionitems.Items)
                {


                    if (double.TryParse(rowvw["Total Value"].ToString(), out totalvalue))
                    {

                        total += totalvalue;
                    }
                }
                this.txttotalprodval.Text = total.ToString();
                
                
            }
            else
            {
                int v = 0;
                if (this.assemdttbleitems.Columns.Count <= 6 && addcols)
                    this.AddDataTableColumns();
                double minimumqty = 0; //qty required
                double minimumrate = 0;
                double productionqty = 0;
                //string tots = "";
                foreach (System.Data.DataRowView rowvw in this.dgproductionitems.Items)
                {


                    if (double.TryParse(rowvw["Qty Needed Per Unit"].ToString(), out minimumqty) && double.TryParse(rowvw["Cost"].ToString(), out minimumrate) && double.TryParse(this.txtqtytoprod.Text, out productionqty))
                    {

                        total += minimumrate * (minimumqty * productionqty);
                    }

                    if (addcols)
                    {
                        //tots += $" minqty {minimumqty} --prodqty-- {productionqty} -- all qty--minrate-- {productionqty}-- {minimumqty * productionqty} - all vala- {minimumrate * (minimumqty * productionqty)}";
                        this.assemdttbleitems.Rows[v]["Total Quantity"] = minimumqty * productionqty;
                        this.assemdttbleitems.Rows[v]["Total Value"] = minimumrate * (minimumqty * productionqty);
                    }


                    v++;
                }
                //MessageBox.Show(tots);
                if (addcols)
                {
                    this.dgproductionitems.ItemsSource = null;
                    this.dgproductionitems.ItemsSource = this.assemdttbleitems.DefaultView;
                }

                this.txttotalprodval.Text = total.ToString();
                this.txtItemCost.Text = (total / productionqty).ToString();
            }
            
        }
        public void AddDataTableColumns()
        {
            if(assemdttbleitems != null)
            {
                System.Data.DataColumn tqty = new System.Data.DataColumn("Total Quantity", typeof(double));
                System.Data.DataColumn tval = new System.Data.DataColumn("Total Value", typeof(double));
                this.assemdttbleitems.Columns.Add(tqty);
                this.assemdttbleitems.Columns.Add(tval);
            }
        }
        bool HasEnoughQty()
        {
            double avail = 0;
            double toproduce = 0;
            foreach (System.Data.DataRowView rowvw in this.dgproductionitems.Items)
            {
                double.TryParse(rowvw["Qty Available"].ToString(), out avail);
                double.TryParse(rowvw["Total Quantity"].ToString(), out toproduce);
                if(toproduce > avail)
                {
                    return false;
                }
            }
            return true;
        }
        private void btnProcessProduction_Click(object sender, RoutedEventArgs e)
        {
            this.pgbItemGroup.IsIndeterminate = true;
            produceworker.RunWorkerAsync();
        }
        public void producework_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(() => {

                this.lblShowProgress.Content = "Production starting .... ";
                this.btnProcessProduction.IsEnabled = false;
            });


            if (IsMassProduce)
            {
             
                    if (HasEnoughQty() && prodgroup != null && prodgroup.Tables.Count > 0)
                    {
                       // MessageBox.Show("Initiating production");
                        foreach (DataTable itemtbl in prodgroup.Tables)
                        {
                            string itemidentifier = itemtbl.TableName;
                            string itemname = itemtbl.Rows[0]["Product"].ToString();
                            string uom = itemtbl.Rows[0]["Unit of Measure"].ToString();
                            string cost = itemtbl.Rows[0]["Cost"].ToString();
                            string qtyavailable = itemtbl.Rows[0]["Qty Available"].ToString();
                            string qtynperunit = itemtbl.Rows[0]["Qty Needed Per Unit"].ToString();
                            string valuepunit = itemtbl.Rows[0]["Value Per Unit"].ToString();
                            string totalqty = itemtbl.Rows[0]["Total Quantity"].ToString();
                            string totalval = itemtbl.Rows[0]["Total Value"].ToString();
                            string qtytoproduce = itemtbl.Rows[0]["QtyToProduce"].ToString();
                            string ValueToProduce = itemtbl.Rows[0]["ValueToProduce"].ToString();
                            string assmnm = itemtbl.Rows[0]["AssemblyName"].ToString();
                            string assmuom = itemtbl.Rows[0]["AssemblyUOM"].ToString();
                        this.Dispatcher.Invoke(() => {
                                if(!this.IsBatchNoExists())
                                {
                                    this.txtprodno.Text = this.ReturnProductionNumber();
                                    ProcessBatch(itemidentifier, assmnm, assmuom, double.Parse(qtytoproduce), double.Parse(ValueToProduce), itemtbl
                                   );
                                    
                                }
                                else
                                {
                                    MessageBox.Show("Batch no already exists, pls process the batch again");
                                    return;
                                }
                              
                            });

                        }
                       MessageBox.Show("Mass production completed");
                       ReloadDocument();
                    }
               
              
                

            }
            else
            {
                //produce one eitem
                if (this.cmbbomlist.SelectedIndex == -1)
                {
                    MessageBox.Show("Pls fill all the details before processing the batch");
                    return;
                }
                this.Dispatcher.Invoke(() => {
                    ProcessBatch(this.cmbbomlist.SelectedValue.ToString(), this.lblproditem.Content.ToString(), this.lblproduom.Content.ToString(), Convert.ToDouble(this.txtqtytoprod.Text), Convert.ToDouble(this.txttotalprodval.Text), new DataTable("#emptytbl"));
                });

            }
        }
        public void productioncomplete_RunWorkerCompleted(object sender,
                                                   RunWorkerCompletedEventArgs e)
        {
            this.pgbItemGroup.IsIndeterminate = false;
            this.btnProcessProduction.IsEnabled = true;
            this.lblShowProgress.Content = "Production complete .."; 
        }

        void ProcessBatch(string itemcode,string item,string uom, double qtyproduce, double itemproductval, DataTable lineitems)
        {
            if(!IsMassProduce && this.lblBatchStatus.Content.ToString() == "Open" && this.IsBatchNoExists())
            {
                MessageBox.Show("Batch no already exists");
                currentbatchno += 1;
                this.txtprodno.Text = defaultbatchcode + currentbatchno.ToString().PadLeft
                    (4, '0');
                return;
            }
            //item code to produce,productionno ,Qty to produce,qtytoprod

            //check if batch is open

            QbConn assemble = new QbConn();
            assemble.createConnection();
            assemble.openConnection();
            if ((string)this.lblBatchStatus.Content == "Open")
            {
                //check if transation number already

                // creating the build trnsaction
                //this.cmbbomlist.SelectedValue.ToString(), this.lblproditem, this.txtprodno.Text, double.Parse(this.txtqtytoprod.Text)
                assemble.BuildBuildAssemblyAddRq(itemcode,item , this.dtproddate.Text, this.ReturnProductionNumber(), qtyproduce, this.txtRemarks.Text);
                assemble.getResponse();
                assemble.closeConnection();
                Interop.QBFC13.IResponse response = assemble.responseMsgSet.ResponseList.GetAt(0);
                //MessageBox.Show(assemble.responseMsgSet.ToXMLString());
                Interop.QBFC13.IBuildAssemblyRet prodrun = (Interop.QBFC13.IBuildAssemblyRet)response.Detail;

                //Interop.QBFC13.IBuildAssemblyRet prd = prodrun;
                if (prodrun.IsPending.GetValue())
                {
                    BomRun rn = new BomRun()
                    {
                        bomrunref = prodrun.RefNumber.GetValue(),
                        bomrundate = (DateTime)this.dtproddate.SelectedDate,
                        processedby = "admin",
                        transactionid = prodrun.TxnID.GetValue(),
                        productionitem = this.txtprodno.Text,
                        uom = uom,//this.lblproduom.Content.ToString(),
                        totalqtyproduced = qtyproduce,//Convert.ToDouble(this.txtqtytoprod.Text),
                        totalvalue = itemproductval,//Convert.ToDouble(this.txttotalprodval.Text),
                        comments = this.txtRemarks.Text,
                        batchstatus = "Pending"


                    };
                    rn = Controller.ReturnSaveData<BomRun>(rn);

                    // set line items 
                    SaveBatchLines(rn, lineitems);


                    MessageBox.Show($"The system could not process batch {prodrun.RefNumber.GetValue()}, make sure you have enough quantity before proceeding, the batch will be marked as pending");
                    selectedbatchno = prodrun.TxnID.GetValue();
                    ReloadDocument();
                }
                else
                {
                    BomRun rn = new BomRun()
                    {
                        bomrunref = prodrun.RefNumber.GetValue(),
                        bomrundate = (DateTime)this.dtproddate.SelectedDate,//to check
                        processedby = "admin",
                        transactionid = prodrun.TxnID.GetValue(),
                        productionitem = item,//this.lblproditem.Content.ToString(),
                        uom = uom,//this.lblproduom.Content.ToString(),
                        totalqtyproduced = qtyproduce,//Convert.ToDouble(this.txtqtytoprod.Text),
                        totalvalue = itemproductval,//Convert.ToDouble(this.txttotalprodval.Text),
                        comments = this.txtRemarks.Text,
                        batchstatus = "Processed"


                    };
                    Controller.ReturnSaveData<BomRun>(rn);
                    // set line items 
                    SaveBatchLines(rn, lineitems);
                    selectedbatchno = prodrun.TxnID.GetValue();
                    if (!IsMassProduce)
                    {
                        
                        ReloadDocument();
                        MessageBox.Show($"Production batch {prodrun.RefNumber.GetValue()} processed successfully");
                    }
                        
                }
                BomSettings st = Controller.GetOneRecord<BomSettings>();
                st.bomcode = "PROD";
                currentbatchno += 1;
                st.bomrefno = currentbatchno;
                Controller.SaveData<BomSettings>(st);
               
            }
            else
            {
                assemble.BuildBuildAssemblyModRq(editseq, this.dtproddate.Text, selectedbatchno, this.txtprodno.Text, double.Parse(this.txtqtytoprod.Text), this.txtRemarks.Text);
                assemble.getResponse();
                assemble.closeConnection();

                if (assemble.responseMsgSet.ResponseList == null)
                {
                    MessageBox.Show("Error: Could not modify the batch");
                    return;
                }

                Interop.QBFC13.IResponse response = assemble.responseMsgSet.ResponseList.GetAt(0);
                Interop.QBFC13.IBuildAssemblyRet prodrun = (Interop.QBFC13.IBuildAssemblyRet)response.Detail;
                MessageBox.Show("test 1: "+prodrun.RefNumber.GetValue());
                //Interop.QBFC13.IBuildAssemblyRet prd = prodrun;
                BomRun rn = Controller.GetBomRun(currenttransbom);
                if (prodrun.IsPending.GetValue())
                {


                    rn.bomrundate = (DateTime)this.dtproddate.SelectedDate;
                    rn.totalqtyproduced = Convert.ToDouble(this.txtqtytoprod.Text);
                    rn.totalvalue = Convert.ToDouble(this.txttotalprodval.Text);
                    rn.comments = this.txtRemarks.Text;
                    rn = Controller.ReturnSaveData<BomRun>(rn);

                    // set line items 
                    SaveBatchLines(rn, lineitems);

                    //BomSettings st = Controller.GetOneRecord<BomSettings>();
                    //st.bomcode = "PROD";
                    //st.bomrefno += 1;
                    //Controller.SaveData<BomSettings>(st);

                    MessageBox.Show($"The system could not process batch {prodrun.RefNumber.GetValue()}, make sure you have enough quantity before proceeding, the batch will be marked as pending");

                }
                else
                {
                    rn.bomrundate = this.dtproddate.DisplayDate;
                    rn.totalqtyproduced = Convert.ToDouble(this.txtqtytoprod.Text);
                    rn.totalvalue = Convert.ToDouble(this.txttotalprodval.Text);
                    rn.comments = this.txtRemarks.Text;
                    rn.batchstatus = "Processed";
                    Controller.ReturnSaveData<BomRun>(rn);
                    // set line items 
                    SaveBatchLines(rn, lineitems);
                    //BomSettings st = Controller.GetOneRecord<BomSettings>();
                    //st.bomcode = "PROD";
                    // st.bomrefno += 1;
                    //Controller.SaveData<BomSettings>(st);
                    MessageBox.Show($"Production batch {prodrun.RefNumber.GetValue()} processed successfully");

                }
                selectedbatchno = prodrun.TxnID.GetValue();
                ReloadDocument();
            }

        }
        void SaveBatchLines(BomRun rn, DataTable massproducefields)
        {


            if ((string)this.lblBatchStatus.Content == "Open")
            {

                if(IsMassProduce)
                {
                    foreach(DataRow rw in massproducefields.Rows)
                    {
                        BomRunItems itms = new BomRunItems()
                        {
                            bomid = rn,
                            product = rw["Product"].ToString(),
                            uom = rw["Unit of Measure"].ToString(),
                            cost = decimal.Parse(rw["Cost"].ToString(), System.Globalization.NumberStyles.Float),
                            qtyavailable = decimal.Parse(rw["Qty Available"].ToString(), System.Globalization.NumberStyles.Float),
                            qtyperunit = decimal.Parse(rw["Qty Needed Per Unit"].ToString(), System.Globalization.NumberStyles.Float),
                            valueperunit = decimal.Parse(rw["Value Per Unit"].ToString(), System.Globalization.NumberStyles.Float),
                            totalqtyused = decimal.Parse(rw["Total Quantity"].ToString(), System.Globalization.NumberStyles.Float),
                            totalvalue = decimal.Parse(rw["Total Value"].ToString(), System.Globalization.NumberStyles.Float)
                        };
                        Controller.SaveData<BomRunItems>(itms);
                    }

                }
                else
                {
                    foreach (System.Data.DataRowView dr in dgproductionitems.ItemsSource)
                    {

                        if (!dr.IsNew)
                        {


                            BomRunItems itms = new BomRunItems()
                            {

                                bomid = rn,
                                product = dr[0].ToString(),
                                uom = dr[1].ToString(),
                                cost = decimal.Parse(dr[2].ToString(), System.Globalization.NumberStyles.Float),
                                qtyavailable = decimal.Parse(dr[3].ToString(), System.Globalization.NumberStyles.Float),
                                qtyperunit = decimal.Parse(dr[4].ToString(), System.Globalization.NumberStyles.Float),
                                valueperunit = decimal.Parse(dr[5].ToString(), System.Globalization.NumberStyles.Float),
                                totalqtyused = decimal.Parse(dr[6].ToString(), System.Globalization.NumberStyles.Float),
                                totalvalue = decimal.Parse(dr[7].ToString(), System.Globalization.NumberStyles.Float)
                            };
                            Controller.SaveData<BomRunItems>(itms);

                        }
                    }
                }

            }
            else
            {
                IList<BomRunItems> ft = Controller.GetBomRunItems(rn);
                int i = 0;
                
                //MessageBox.Show(ft.Count.ToString());
                foreach (System.Data.DataRowView dr in dgproductionitems.ItemsSource)
                {
                    if (!dr.IsNew)
                    {
                        
                        ft[i].product = dr[0].ToString();
                        ft[i].uom = dr[1].ToString();
                        ft[i].cost = decimal.Parse(dr[2].ToString());
                        ft[i].qtyavailable = decimal.Parse(dr[3].ToString());
                        ft[i].qtyperunit = decimal.Parse(dr[4].ToString());
                        ft[i].valueperunit = decimal.Parse(dr[5].ToString());
                        ft[i].totalqtyused = decimal.Parse(dr[6].ToString());
                        ft[i].totalvalue = decimal.Parse(dr[7].ToString());
                        Controller.SaveData<BomRunItems>(ft[i]);
                        i++;

                    }
                }
            }
                

            
        }
        public bool IsBatchNoExists()
        {
            //checks whether a batch number exists
            QbConn tcon1 = new QbConn();
            tcon1.createConnection();
            tcon1.openConnection();
            string vno = "";
            this.Dispatcher.Invoke(() =>
            {
                vno = this.ReturnProductionNumber();
            });
            tcon1.BuildBuildAssemblyQueryCheckExists(vno);
            tcon1.getResponse();
            tcon1.closeConnection();
            Interop.QBFC13.IResponseList hh = tcon1.responseMsgSet.ResponseList;
           if(hh != null)
            {
                Interop.QBFC13.IResponse hj = hh.GetAt(0);
                if(hj.retCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
           else
            {
                return false;
            }
            
         
        }

        
        void SetDocHeader(Interop.QBFC13.IResponse rs)
        {
            if (rs == null)
                return;
            Interop.QBFC13.IBuildAssemblyRetList assemd = rs.Detail as Interop.QBFC13.IBuildAssemblyRetList;
            Interop.QBFC13.IBuildAssemblyRet returneddata = assemd.GetAt(0);
            currenttransbom = returneddata.TxnID.GetValue();
            editseq = returneddata.EditSequence.GetValue();
            BomRun dbomrn = Controller.GetBomRun(currenttransbom);
            this.Dispatcher.Invoke(() =>
            {
                this.lblproduom.Content = dbomrn.uom;
                this.txtprodno.Text = returneddata.RefNumber.GetValue();
                this.dtproddate.Text = returneddata.TxnDate.GetValue().ToShortDateString();
                this.lblBatchStatus.Content = returneddata.IsPending.GetValue() ? "Pending" : "Processed";
                this.txtMaxCanBuild.Text = returneddata.QuantityCanBuild.GetValue().ToString();
                this.txtqtytoprod.Text = returneddata.QuantityToBuild.GetValue().ToString();
                this.txtRemarks.Text = returneddata.Memo == null ? "" : returneddata.Memo.GetValue();
                this.lblproditem.Content = returneddata.ItemInventoryAssemblyRef.FullName.GetValue();
                this.cmbbomlist.SelectionChanged -= cmbbomlist_SelectionChanged;
                this.cmbbomlist.SelectedValue = returneddata.ItemInventoryAssemblyRef.ListID.GetValue();
                this.cmbbomlist.SelectionChanged += cmbbomlist_SelectionChanged;
                this.btnPrint.IsEnabled = true;
                this.btnDelete.IsEnabled = true;
                this.btnModify.IsEnabled = false;

                if (returneddata.IsPending.GetValue())
                {
                    this.lblBatchStatus.Foreground = System.Windows.Media.Brushes.OrangeRed;
                    this.txtqtytoprod.IsEnabled = true;
                    this.btnProcessProduction.IsEnabled = true;
                    this.cmbbomlist.IsEnabled = false;
                    this.txtRemarks.IsEnabled = true;
                    this.btnDelete.IsEnabled = true;
                    this.btnModify.IsEnabled = false;
                }
                else
                {
                    this.btnModify.IsEnabled = true;
                    this.lblBatchStatus.Foreground = System.Windows.Media.Brushes.PaleGreen;
                    this.txtqtytoprod.IsEnabled = false;
                    this.cmbbomlist.IsEnabled = false;
                    this.txtRemarks.IsEnabled = false;
                    this.btnProcessProduction.IsEnabled = false;
                    this.btnDelete.IsEnabled = true;
                    this.btnModify.IsEnabled = true;
                }


            });
            
            try
            {
                this.Dispatcher.Invoke(
                    ()=> {
                        SetDocDetails(Controller.GetBomRun(returneddata.TxnID.GetValue()));
                    });
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.Source+"\n"+ ex.StackTrace);
                MessageBox.Show("Error getting bom, confirm whether the batch has been deleted");
            }
            

        }
        void SetDocDetails(BomRun hj)
        {
            

            IList<object[]> d = Controller.GetBomLines(hj);

            assemdttbleitems = null;
            assemdttbleitems = new System.Data.DataTable("#assemlylistitems");
            assemdttbleitems.Columns.Add("Product", typeof(string));
            assemdttbleitems.Columns.Add("Unit of Measure", typeof(string));
            assemdttbleitems.Columns.Add("Cost", typeof(decimal));
            assemdttbleitems.Columns.Add("Qty Available", typeof(decimal));
            assemdttbleitems.Columns.Add("Qty Needed Per Unit", typeof(decimal));
            assemdttbleitems.Columns.Add("Value Per Unit", typeof(decimal));
            assemdttbleitems.Columns.Add("Total Quantity", typeof(decimal));
            assemdttbleitems.Columns.Add("Total Value", typeof(decimal));
            for (int i = 0; i < d.Count; i++)
            {
                
                assemdttbleitems.Rows.Add(d[i][0], d[i][1], d[i][2], d[i][3], d[i][4], d[i][5], d[i][6], d[i][7]);

            }
            
            this.dgproductionitems.Columns.Clear();
            this.dgproductionitems.ItemsSource = assemdttbleitems.DefaultView;
            SetFooterTotals(false);

        }
        BomRunItems ReturnClassObject(object[] t, BomRun g)
        {
            /*Id 
         itemid 
        public virtual string product { get; set; }
        public virtual decimal qtyavailable { get; set; }
        public virtual decimal cost { get; set; }
        public virtual decimal qtyperunit { get; set; }
        public virtual decimal valueperunit { get; set; }
        public virtual decimal totalqtyused { get; set; }
        public virtual decimal totalvalue { get; set; }
        public virtual BomRun bomid { get; set; }
        public virtual string uom { get; set; }*/

        BomRunItems lineitema = new BomRunItems()
        {
            Id = Convert.ToInt32 (t[0].ToString()),
            itemid = t[1].ToString(),
            product = t[2].ToString(),
            qtyavailable = Convert.ToDecimal(t[3].ToString()),
            cost = Convert.ToDecimal(t[4].ToString()),
            qtyperunit = Convert.ToDecimal(t[5].ToString()),
            valueperunit = Convert.ToDecimal(t[6].ToString()),
            totalqtyused = Convert.ToDecimal(t[7].ToString()),
            totalvalue = Convert.ToDecimal(t[8].ToString()),
            bomid = g,
            uom = t[10].ToString()



        };

            return lineitema;

    }

        private void btnShowBatches_Click(object sender, RoutedEventArgs e)
        {
            Window1 n = new Window1();
            n.ShowDialog();
            ReloadDocument();
            
        }
        void ReloadDocument()
        {
            if (selectedbatchno.Trim() != "")
            {

                QbConn ll = new QbConn();
                ll.createConnection();
                ll.openConnection();
                ll.BuildBuildAssemblyQueryRqOne(selectedbatchno);
                ll.getResponse();
                ll.closeConnection();
                Interop.QBFC13.IResponseList hh = ll.responseMsgSet.ResponseList;
                Interop.QBFC13.IResponse hj = hh.GetAt(iteratoridnum);

                SetDocHeader(hj);

            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            

            this.lblShowProgress.Content = "Initializing Application ... ";
            this.pgbItemGroup.IsIndeterminate = true;
            loadingworker.RunWorkerAsync();


        }
        void newDocument()
        {
            

            //load bom items
            QbConn assemitems = new QbConn();
            assemitems.createConnection();
            assemitems.openConnection();
            assemitems.BuildItemInventoryAssemblyQueryRq();
            assemitems.getResponse();
            assemitems.closeConnection();

            assemlist = assemitems.returnDataList<Interop.QBFC13.IItemInventoryAssemblyRetList>();
            assemdttble = new System.Data.DataTable("#assemlylist");
           // assemdttble.Columns.Add("Tick", typeof(bool));
            assemdttble.Columns.Add("Listid", typeof(string));
            assemdttble.PrimaryKey = new System.Data.DataColumn[] { assemdttble.Columns["listid"] };
            assemdttble.Columns.Add("Bom", typeof(string));
            assemdttble.Columns.Add("Uom", typeof(string));            
            assemdttble.Columns.Add("Qty", typeof(string));

            IList<Boms> bomlist = Controller.GetData<Boms>();

            if (bomlist.Count > 0)
            {
                for (int i = 0; i < bomlist.Count; i++)
                {
                    assemdttble.Rows.Add(bomlist[i].assemblylistid, bomlist[i].bomname, bomlist[i].uom, "");

                }

                

                //this.InitProductGrid();

            }
            




        }
        public void FinalizeIterfaceInit()
        {
            this.lblBatchStatus.Foreground = System.Windows.Media.Brushes.DarkBlue;
            this.dgproductionitems.ItemsSource = null;
            this.cmbbomlist.SelectionChanged -= cmbbomlist_SelectionChanged;
            this.cmbbomlist.ItemsSource = null;
            this.lblBatchStatus.Content = "Open";
            this.btnProcessProduction.IsEnabled = false;
            this.txtqtytoprod.IsEnabled = true;
            this.cmbbomlist.IsEnabled = true;
            this.txtRemarks.IsEnabled = true;
            this.dtproddate.IsEnabled = true;
            this.btnPrint.IsEnabled = false;
            this.btnDelete.IsEnabled = false;
            this.btnModify.IsEnabled = false;
            this.lblProdCost.Visibility = Visibility.Visible;
            this.txtItemCost.Visibility = Visibility.Visible;

            this.txtqtytoprod.Text = "";
            this.txtMaxCanBuild.Text = "";
            currenttransbom = "";
            this.txtRemarks.Text = "";
            this.dtproddate.Text = DateTime.Now.ToShortDateString();
            this.txtprodno.Text = this.ReturnProductionNumber(); // get production number
            this.lblproditem.Content = "";
            this.lblproduom.Content = "";

            //this.btnLoad.IsEnabled = false;
            //this.massproddatagrid.Columns.Clear();


            this.cmbbomlist.ItemsSource = assemdttble.DefaultView;
            this.cmbbomlist.DisplayMemberPath = "Bom";
            this.cmbbomlist.SelectedValuePath = "Listid";

            this.cmbbomlist.SelectedIndex = -1;
            this.cmbbomlist.SelectionChanged += cmbbomlist_SelectionChanged;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new ProductionReport(this.txtprodno.Text.Trim()).ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            this.txtqtytoprod.IsEnabled = true;
            this.btnProcessProduction.IsEnabled = true;
            this.cmbbomlist.IsEnabled = false;
            this.txtRemarks.IsEnabled = true;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(currenttransbom))
            {
                QbConn tcon1 = new QbConn();
                tcon1.createConnection();
                tcon1.openConnection();
                tcon1.BuildTxnDelRq(currenttransbom);
                tcon1.getResponse();
                tcon1.closeConnection();
                try
                {
                    if (tcon1.responseMsgSet == null) return;
                    Interop.QBFC13.IResponseList responseList = tcon1.responseMsgSet.ResponseList;
                    if (responseList == null) return;
                    //if we sent only one request, there is only one response, we'll walk the list for this sample
                    for (int i = 0; i < responseList.Count; i++)
                    {
                        Interop.QBFC13.IResponse response = responseList.GetAt(i);
                        //check the status code of the response, 0=ok, >0 is warning
                        if (response.StatusCode >= 0)
                        {


                            BomRun dbomrn = Controller.GetBomRun(currenttransbom);
                            Controller.DeleteData<BomRun>(dbomrn);

                            MessageBox.Show($"Deleted batch {this.txtprodno.Text} Successfully");
                            this.lblShowProgress.Content = "Initializing Application ...";
                            this.pgbItemGroup.IsIndeterminate = true;
                            loadingworker.RunWorkerAsync();

                        }
                        else
                        {
                            throw new Exception("Error deleting batch in quickbooks");
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"unable to delete batch {this.txtprodno.Text}, pls confirm that the transaction has been deleted in quickbooks");
                    this.lblShowProgress.Content = "Initializing Application ..";
                    this.pgbItemGroup.IsIndeterminate = true;
                    loadingworker.RunWorkerAsync();
                }
                
            }
            
        }
        void OnTicked(object sender, RoutedEventArgs e)
        {


            CheckBox chbx = (CheckBox)e.OriginalSource;
            if((bool)chbx.IsChecked)
            {

               // DataGridCell cellInfo = this.dgbomlist.SelectedCells[0];
                //DataRowView row = this.dgbomlist.SelectedItem as DataRowView;
                //this.dgbomlist.BeginEdit(2, 4);
                
                

            }
            MessageBox.Show("Checked");


            //throw new NotImplementedException();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

           
        }

        private void btnMassProduce_Click(object sender, RoutedEventArgs e)
        {

         
        }
        private void InitMassGrid()
        {
            try
            {
                massproddatatable = null;
                massproddatatable = ConvertBomToDataTable(Controller.GetBomsMass());
                
                // this.massproddatagrid.Columns[4].IsReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void SetMassGrid()
        {
            this.massproddatagrid.Columns.Clear();
            this.massproddatagrid.ItemsSource = massproddatatable.DefaultView;
            this.massproddatagrid.Columns[massproddatagrid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            this.massproddatagrid.Columns[0].IsReadOnly = true;
            this.massproddatagrid.Columns[1].IsReadOnly = true;
            this.massproddatagrid.Columns[2].IsReadOnly = true;
            this.massproddatagrid.Columns[2].Visibility = Visibility.Hidden;
            this.btnLoad.IsEnabled = true;
            /*this.cmbbomlist.SelectionChanged -= cmbbomlist_SelectionChanged;
            this.cmbbomlist.SelectedIndex = -1;
            this.cmbbomlist.SelectionChanged += cmbbomlist_SelectionChanged; */
            this.assemdttbleitems = null;
        }
        public static System.Data.DataTable ConvertBomToDataTable(IList<object[]> d)
        {
            DataTable dt = new DataTable("#tmptable");
            //dt.Columns.Add("Select", typeof(Boolean));
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Uom", typeof(string));
            dt.Columns.Add("TranId", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            for (int i = 0; i < d.Count; i++)
            {
                //.Substring(8)
                dt.Rows.Add(d[i][0].ToString(), d[i][1], d[i][2], 0);

            }
            return dt;

        }

        private void massproddatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            //this.massproddatagrid.
            /* if (Convert.ToBoolean(massproddatagrid.Rows[0].Cells[0].EditedFormattedValue) == true)
             {
                 //Paste your code here
                 var currentRowIndex = my_dataGrid.Items.IndexOf(my_dataGrid.CurrentItem);
             }*/

        }

        private void massproddatagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        private void massproddatagrid_CurrentCellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            //DataRowView selectedr = (DataRowView)this.massproddatagrid.SelectedItem;

           
            // MessageBox.Show(selectedr.Row[0].ToString());
        }

        private void massproddatagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MessageBox.Show($"row {e.Row.GetIndex()} column {e.Column.DisplayIndex}");
            
        }
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            MessageBox.Show($"row  column ");
            //EditCell(cell, e);
            
        }
        private void Grid_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGridCell cell = (e.OriginalSource as DataGridCell);
                if (cell.IsReadOnly == true)
                    return;
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
                cell.IsEditing = true;
                //string cellValue = GetSelectedCellValue();
            }
        }
        private void EditCell(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly)
                return;

            if (!cell.IsFocused)
            {
                cell.Focus();
            }
           /* DGrid.BeginEdit(e);
            cell.Dispatcher.Invoke(
                   DispatcherPriority.Background,
                   new Action(delegate { })
            );*/

        }

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            this.txtqtytoprod.Text = "";
            this.txtqtytoprod.IsEnabled = false;
            this.lblShowProgress.Content = "Loading Items Please Wait ....";
            this.pgbItemGroup.IsIndeterminate = true;
            IsMassProduce = true;
            worker.RunWorkerAsync();
            

        }
        private void LoadAllProducts()
        {
            // get count of products to produce
            this.Dispatcher.Invoke(() => {

                this.txtItemCost.Visibility = Visibility.Hidden;
                this.lblProdCost.Visibility = Visibility.Hidden;
                this.lblproditem.Content = "<Product List>";
                this.lblproduom.Content = "Default";
                this.txtMaxCanBuild.Text = "";
                this.txtqtytoprod.Text = "";
                this.txtqtytoprod.IsEnabled = false;
            });
            

            prodgroup = new DataSet();
            foreach (DataRowView rw in this.massproddatagrid.ItemsSource)
            {
                //get the quantity in the mass production
                double val = 0;
                double.TryParse(rw[3].ToString(), out val);

                if (val > 0)
                {

                    
                    string itemidentifier = rw[2].ToString();
                    DataTable proddet = new DataTable(itemidentifier);
                    proddet.Columns.Add("Product", typeof(string));
                    proddet.Columns.Add("Unit of Measure", typeof(string));
                    proddet.Columns.Add("Cost", typeof(decimal));
                    proddet.Columns.Add("Qty Available", typeof(decimal));
                    proddet.Columns.Add("Qty Needed Per Unit", typeof(decimal));
                    proddet.Columns.Add("Value Per Unit", typeof(decimal));
                    proddet.Columns.Add("Total Quantity", typeof(decimal));
                    proddet.Columns.Add("Total Value", typeof(decimal));
                    proddet.Columns.Add("QtyToProduce", typeof(decimal));
                    proddet.Columns.Add("ValueToProduce", typeof(decimal));
                    proddet.Columns.Add("AssemblyName", typeof(string));
                    proddet.Columns.Add("AssemblyUOM", typeof(string));
                    //get selected batch bom item lines

                    QbConn getlines = new QbConn();
                    getlines.createConnection();
                    getlines.openConnection();
                    getlines.BuildItemInventoryAssemblyQueryRq(itemidentifier);
                    getlines.getResponse();
                    getlines.closeConnection();



                    if (getlines.responseMsgSet.ResponseList == null || getlines.responseMsgSet.ResponseList.Count <= 0)
                        return;
                    Interop.QBFC13.IItemInventoryAssemblyRetList assemblyitem = (Interop.QBFC13.IItemInventoryAssemblyRetList)getlines.responseMsgSet.ResponseList.GetAt(0).Detail;
                    // MessageBox.Show(getlines.responseMsgSet.ToXMLString());

                    Interop.QBFC13.IItemInventoryAssemblyLineList lineitems = assemblyitem.GetAt(0).ItemInventoryAssemblyLineList;
                    // get the details for each item
                    for (int j = 0; j < lineitems.Count; j++)
                    {
                        Interop.QBFC13.IItemInventoryAssemblyLine lineitem = lineitems.GetAt(j);
                        if (lineitem.ItemInventoryRef == null)
                            continue;
                        QbConn getlineitems = new QbConn();
                        getlineitems.createConnection();
                        getlineitems.openConnection();
                        //MessageBox.Show(lineitem.ItemInventoryRef.ListID.GetValue());
                        getlineitems.BuildItemInventoryQueryRq(lineitem.ItemInventoryRef.ListID.GetValue());
                        getlineitems.getResponse();
                        getlineitems.closeConnection();

                        Interop.QBFC13.IItemInventoryRetList invitemlist = getlineitems.returnDataList<Interop.QBFC13.IItemInventoryRetList>();
                        if (invitemlist.Count > 0)
                        {
                            Interop.QBFC13.IItemInventoryRet invitem = invitemlist.GetAt(0);
                            // invitem.UnitOfMeasureSetRef== null ? invitem.UnitOfMeasureSetRef.FullName.GetValue() : "Default"
                            if (invitem.UnitOfMeasureSetRef == null)
                            {
                                proddet.Rows.Add(invitem.Name.GetValue(), "Default", invitem.AverageCost.GetValue(), invitem.QuantityOnHand.GetValue(),lineitem.Quantity.GetValue(), invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue(), lineitem.Quantity.GetValue() * val, (invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue()) * val,val, val*assemblyitem.GetAt(0).AverageCost.GetValue(), assemblyitem.GetAt(0).Name.GetValue(), assemblyitem.GetAt(0).UnitOfMeasureSetRef.FullName.GetValue());
                            }
                            else
                            {
                                proddet.Rows.Add(invitem.Name.GetValue(), invitem.UnitOfMeasureSetRef.FullName.GetValue(), invitem.AverageCost.GetValue(), invitem.QuantityOnHand.GetValue(), lineitem.Quantity.GetValue(), invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue(), lineitem.Quantity.GetValue() * val, (invitem.AverageCost.GetValue() * lineitem.Quantity.GetValue()) * val, val, val * assemblyitem.GetAt(0).AverageCost.GetValue(), assemblyitem.GetAt(0).Name.GetValue(), assemblyitem.GetAt(0).UnitOfMeasureSetRef.FullName.GetValue());
                            }

                        }


                    }

                    prodgroup.Tables.Add(proddet);


                }
            }
           // this.pgbItemGroup.Visibility = Visibility.Hidden;
            assemdttbleitems = null;
            assemdttbleitems = new DataTable("#assemlylistitems");
            assemdttbleitems.Columns.Add("Product", typeof(string));
            assemdttbleitems.Columns.Add("Unit of Measure", typeof(string));
            assemdttbleitems.Columns.Add("Cost", typeof(decimal));
            assemdttbleitems.Columns.Add("Qty Available", typeof(decimal));
            assemdttbleitems.Columns.Add("Qty Needed Per Unit", typeof(decimal));
            assemdttbleitems.Columns.Add("Value Per Unit", typeof(decimal));
            assemdttbleitems.Columns.Add("Total Quantity", typeof(decimal));
            assemdttbleitems.Columns.Add("Total Value", typeof(decimal));
            if (prodgroup.Tables.Count == 0)
                return;
            foreach (DataTable fg in prodgroup.Tables)
            {
                foreach (DataRow rwh in fg.Rows)
                {
                    DataRow[] foundprod = assemdttbleitems.Select($"Product ='{rwh["Product"].ToString()}'");
                    if (foundprod.Length > 0)
                    {

                        /*
                         * assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Qty Needed Per Unit"] =
                        (decimal)assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Qty Needed Per Unit"] +
                        (decimal)rwh["Qty Needed Per Unit"];
                        assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Value Per Unit"] =
                        (decimal)assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Value Per Unit"] +
                        (decimal)rwh["Value Per Unit"];
                        */

                        assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Total Quantity"] =
                        (decimal)assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Total Quantity"] + (decimal)rwh["Total Quantity"];

                        assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Total Value"] =
                        (decimal)assemdttbleitems.Rows[assemdttbleitems.Rows.IndexOf(foundprod[0])]["Total Value"] + (decimal)rwh["Total Value"];

                    }
                    else
                    {
                        assemdttbleitems.ImportRow(rwh);
                    }
                }


            }
            /*this.dgproductionitems.Columns.Clear();
            this.dgproductionitems.ItemsSource = assemdttbleitems.DefaultView;
            return assemdttbleitems;*/
        }
        private void loadworker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            InitMassGrid();          
            newDocument();

        }
        private void loadworker_RunWorkerCompleted(object sender,
                                                   RunWorkerCompletedEventArgs e)
        {
            SetMassGrid();
            FinalizeIterfaceInit();
            this.cmbbomlist.IsEnabled = true;
            this.btnNew.IsEnabled = true;

            this.lblShowProgress.Content = "";
            this.pgbItemGroup.IsIndeterminate = false;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            LoadAllProducts();
            

        }

        private void worker_RunWorkerCompleted(object sender,
                                                   RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            this.dgproductionitems.Columns.Clear();
            this.dgproductionitems.ItemsSource = assemdttbleitems.DefaultView;
            this.pgbItemGroup.IsIndeterminate = false;
            this.lblShowProgress.Content = "Process complete....";
            
            
            if (this.assemdttbleitems != null && this.assemdttbleitems.Rows.Count > 0)
            {
                this.btnProcessProduction.IsEnabled = true;
                IsMassProduce = true;
            }
            SetFooterTotals(false);


        }
        private void setproductsworker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            InitProductGrid();

        }
        private void setproductsworker_RunWorkerCompleted(object sender,
                                                   RunWorkerCompletedEventArgs e)
        {
            this.dgproductionitems.Columns.Clear();
            this.dgproductionitems.ItemsSource = assemdttbleitems.DefaultView;
            this.pgbItemGroup.IsIndeterminate = false;
            this.lblShowProgress.Content = "";
            if(!IsMassProduce)
            {
                this.txtqtytoprod.IsEnabled = true;
                
            }
            


        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        
    }
    public class MaxProduceConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            //Qty Available
            //Total Quantity
            DataRowView row = values[0] as DataRowView;
            //decimal toproduce = Decimal.Parse((string)values[0]);

            // decimal maxavailable = Decimal.Parse((string)values[1]);
            if (row.Row.ItemArray.Length > 6)
            {
                decimal tqty = decimal.Parse(row.Row.ItemArray[6].ToString());
                decimal qtyavailable = decimal.Parse(row.Row.ItemArray[3].ToString());
                return ( tqty > qtyavailable);
            }
           else
            {
                return false;
            }
            

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)

        {

            throw new NotImplementedException();

        }

    }


}
