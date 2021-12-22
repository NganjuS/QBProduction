using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Interop.QBFC13;

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void qbbtn_Click(object sender, RoutedEventArgs e)
        {
            Controller d = new Controller();
            d.tt();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnrundb_Click(object sender, RoutedEventArgs e)
        {

            using (var session = NHibernateHelper.OpenSession())
            {

                using (var transaction = session.BeginTransaction())
                {
                   
                    
                    transaction.Commit();
                    MessageBox.Show("saved bom number");
                }


            }

        }

        private void addproduct_Click(object sender, RoutedEventArgs e)
        {
            new Production().ShowDialog();
        }

        private void checkbom_Click(object sender, RoutedEventArgs e)
        {

            //Get list of assemblies
            QbConn assem = new QbConn();
            assem.createConnection();
            assem.openConnection();
            assem.BuildItemInventoryAssemblyQueryRq();
            assem.getResponse();
            
            assem.closeConnection();
            IItemInventoryAssemblyRetList assemlylist = assem.returnDataList<IItemInventoryAssemblyRetList>();
            //get list of BOMs in database
            //MessageBox.Show(assem.responseMsgSet.ToXMLString());

            try
            {
                List<Boms> bomlist = Controller.GetData<Boms>();
                if (assemlylist.Count > 0 && (bomlist == null || bomlist.Count == 0 || bomlist.Count < assemlylist.Count))
                {


                    if (MessageBox.Show("There exists assembly items that have not been added to the BOM, Do you want to add them ?", "Bom Listing", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        for (int i = 0; i < assemlylist.Count; i++)
                        {
                            IItemInventoryAssemblyRet assemitem = assemlylist.GetAt(i);
                          
                            if (!bomlist.Exists(x => x.assemblylistid == assemitem.ListID.GetValue()))
                            {

                                Boms newbom = new Boms()
                                {
                                    assemblylistid = assemitem.ListID.GetValue(),
                                    assemblyitem = assemitem.Name.GetValue(),
                                    uom = assemitem.UnitOfMeasureSetRef.FullName.GetValue(),
                                    bomname = "Bom:" + assemitem.Name.GetValue(),
                                    createdon = DateTime.Now.Date,
                                    modifiedon = DateTime.Now.Date



                                };
                                Controller.SaveData<Boms>(newbom);
                            }

                        }
                    }

                }
                else
                {
                    MessageBox.Show(" All production items have already been added");

                }
                this.PopulateGridList();
            }catch(Exception ex)
            {

                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Unknown error occurred");
            }
 
        }
        private void StartProduction(object sender, RoutedEventArgs e)
        {
         
                new Production().ShowDialog();
           
            
        }
        private void EditBOM(object sender, RoutedEventArgs e)
        {

        }

        public void PopulateGridList()
        {
            try
            {
                this.dgbomlisting.Columns.Clear();
                this.dgbomlisting.ItemsSource = ConvertBomToDataTable(Controller.GetBoms()).DefaultView;
                dgbomlisting.Columns[dgbomlisting.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            


            //Boms[] dt = (Boms[])bomlist[0];

            //MessageBox.Show(dt[0].assemblyitem);

        }
        public void CreateGridColumns()
        {

        }
        public static System.Data.DataTable ConvertBomToDataTable(IList<object[]> d)
        {
            DataTable dt = new DataTable("#tmptable");
            dt.Columns.Add("Bom", typeof(string));
            dt.Columns.Add("Production Item", typeof(string));
            dt.Columns.Add("Created On Date", typeof(string));
            for (int i = 0; i < d.Count; i++)
            {
                DateTime fg = (DateTime)d[i][2];
                dt.Rows.Add(d[i][0], d[i][1], fg.ToString("dd-MM-yyyy"));
             
            }
            return dt;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.PopulateGridList();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Production gh = new Production();
            gh.ShowDialog();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            BatchListing fh = new BatchListing();
            fh.ShowDialog();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            new RawMaterialReport().ShowDialog();
        }
    }
}
