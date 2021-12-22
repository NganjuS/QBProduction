using System;
using System.Collections.Generic;
using System.Data;
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
using Interop.QBFC13;

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        public AddProduct()
        {
            
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.cmdsalesacc.SelectedValue.ToString());

            Dictionary<string, string> stkdata = new Dictionary<string, string>() {
                { "name", this.txtcode.Text},
                { "incomeacc", this.cmdsalesacc.SelectedValue.ToString() },
                { "desc", this. txtdesc.Text },
                { "cogsacc", this.cmbcostsales.SelectedValue.ToString() },
                { "assetsacc", "Stock"}

            };
            QbConn createprod = new QbConn();
            createprod.createConnection();
            createprod.openConnection();
            createprod.BuildItemInventoryAddRq(stkdata);
            createprod.getResponse();
            createprod.closeConnection();
            MessageBox.Show(createprod.responseMsgSet.ResponseList.GetAt(0).StatusMessage);

        }
        private void GetItemCode()
        {
            
            var stksettings = Controller.GetOneRecord<BomSettings>();
             if(stksettings != null && stksettings.bomrefno != 0 )
             {
                MessageBox.Show("");
             }
             else
            {
                BomSettings st = new BomSettings {
                    bomrefno = 1, bomcode = "MANU", stockcode = "Stk", stockrefno = 1

                };
                Controller.SaveData<BomSettings>(st);
                    
                    
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load number
            this.GetItemCode();
            // load sales accounts
            QbConn loadsales = new QbConn();
            loadsales.createConnection();
            loadsales.openConnection();
            loadsales.BuildAccountQueryRq(ENAccountType.atIncome);
            loadsales.getResponse();
            loadsales.closeConnection();
            if(loadsales.responseIsValid<ENResponseType>( ENResponseType.rtAccountQueryRs))
            {
            
                
                IAccountRetList ft = loadsales.returnDataList<IAccountRetList>();
                this.cmdsalesacc.ItemsSource  = WalkAccountRet(ft).DefaultView;
                this.cmdsalesacc.DisplayMemberPath = "nm";
                this.cmdsalesacc.SelectedValuePath = "accid";
               // MessageBox.Show(loadsales.responseMsgSet.ToXMLString());


            }
            //load cost of sale accounts
            QbConn loadcostofsales = new QbConn();
            loadcostofsales.createConnection();
            loadcostofsales.openConnection();
            loadcostofsales.BuildAccountQueryRq(ENAccountType.atCostOfGoodsSold);
            loadcostofsales.getResponse();
            loadcostofsales.closeConnection();
            if (loadcostofsales.responseIsValid<ENResponseType>(ENResponseType.rtAccountQueryRs))
            {


                IAccountRetList ft = loadcostofsales.returnDataList<IAccountRetList>();
                this.cmbcostsales.ItemsSource = WalkAccountRet(ft).DefaultView;
                this.cmbcostsales.DisplayMemberPath = "nm";
                this.cmbcostsales.SelectedValuePath = "accid";
                // MessageBox.Show(loadsales.responseMsgSet.ToXMLString());


            }
            List<string> t = new List<string>()
            {
                "Assembly Item",
                "Stock Item"
            };
            this.cmbprodtype.ItemsSource = t;
            //load unit of measures
            QbConn loaduon = new QbConn();
            loaduon.createConnection();
            loaduon.openConnection();
            loaduon.BuildUnitOfMeasureSetQueryRq();
            loaduon.getResponse();
            loaduon.closeConnection();
            if(loaduon.responseIsValid<ENResponseType>(ENResponseType.rtUnitOfMeasureSetQueryRs ))
            {
                
                IUnitOfMeasureSetRetList fg = loaduon.returnDataList<IUnitOfMeasureSetRetList>();
                this.cmbuom.ItemsSource = WalkUomRet(fg).DefaultView;
                this.cmbuom.DisplayMemberPath = "nm";
                this.cmbuom.SelectedValuePath = "accid";
            }
           // MessageBox.Show(loaduon.responseMsgSet.ToXMLString());


        }
        public DataTable WalkUomRet(IUnitOfMeasureSetRetList uomlist)
        {

            if (uomlist == null) return new DataTable();
            DataTable acc = new DataTable("#tmp");
            acc.Columns.Add("accid");
            acc.Columns.Add("nm");
            for (int i = 0; i < uomlist.Count; i++)
            {
                IUnitOfMeasureSetRet uomret = uomlist.GetAt(i);
                acc.Rows.Add(uomret.ListID.GetValue(), uomret.Name.GetValue());
              
                //MessageBox.Show(AccountRet.ListID.GetValue());
            }
            return acc;


        }

        public DataTable WalkAccountRet(IAccountRetList AccountRetD)
        {
            if (AccountRetD == null) return new DataTable();
            //Go through all the elements of IAccountRetList
            //Get value of ListID
            DataTable acc = new DataTable("#tmp");
            acc.Columns.Add("accid");
            acc.Columns.Add("nm");


            for (int i = 0; i < AccountRetD.Count; i++)
            {
                IAccountRet AccountRet = AccountRetD.GetAt(i);
                acc.Rows.Add(AccountRet.ListID.GetValue(), AccountRet.Name.GetValue());
               
                //MessageBox.Show(AccountRet.ListID.GetValue());
            }


            return acc;


        }

        private void cmbprodtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if(this.cmbprodtype.SelectedValue.ToString() == "Assembly Item")
            {
                this.addassemitems.IsEnabled = true;
                this.dtgridaddprod.Visibility = Visibility.Visible;
            }
            else
            {
                this.addassemitems.IsEnabled = false;
                this.dtgridaddprod.Visibility = Visibility.Hidden;
            }
        }
    }
    
    
}
