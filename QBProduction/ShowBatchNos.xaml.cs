using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly BackgroundWorker searchworker = new BackgroundWorker();
        public static string searchno = "";
        public static Interop.QBFC13.IBuildAssemblyRetList assemd;
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            searchworker.DoWork += initiateLoadBatch_DoWork;
            searchworker.RunWorkerCompleted += finaliseLoadBatch_RunWorkerCompleted;


        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataRowView row = dgBatchNos.SelectedItem as System.Data.DataRowView;
            if(dgBatchNos.SelectedItems.Count > 0)
            {
                Production.selectedbatchno = row.Row.ItemArray[2].ToString();
                
            }
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            searchno = this.txtSearchbar.Text.Trim();
            initDataLoad(true);


        }

        private void btnLoadAll_Click(object sender, RoutedEventArgs e)
        {
            searchno = "";
            initDataLoad(false);

        }
        public void initDataLoad(bool isSearch)
        {
            if(isSearch)
            {
                
                if (String.IsNullOrEmpty(searchno))
                {
                    MessageBox.Show("No batch no has been entered !!");
                    return;
                }

            }
            this.lblProgress.Content = "Initiating Search....";
            this.pgrSearchPr.IsIndeterminate = true;
            searchworker.RunWorkerAsync();
            
   
            

        }
        public void updateScreenList()
        {
            System.Data.DataTable tbl = new System.Data.DataTable("#tbl");
            tbl.Columns.Add("Batch No", typeof(string));
            tbl.Columns.Add("Batch Status", typeof(string));
            tbl.Columns.Add("Transaction Id", typeof(string));
            tbl.Columns.Add("Batch Date", typeof(string));
            if (assemd == null) {
                MessageBox.Show("Data not found !!");
                return;
            };
            // MessageBox.Show(assemd.Count.ToString());
            for (int i = 0; i < assemd.Count; i++)
            {
                Interop.QBFC13.IBuildAssemblyRet lineitem = assemd.GetAt(i);
                tbl.Rows.Add(lineitem.RefNumber.GetValue(), lineitem.IsPending.GetValue() ? "Pending" : "Processed", lineitem.TxnID.GetValue(), lineitem.TxnDate.GetValue().ToShortDateString());
                //ft += lineitem.RefNumber.GetValue() + "\t";
            }
            this.dgBatchNos.ItemsSource = tbl.DefaultView;
            dgBatchNos.Columns[2].Visibility = Visibility.Hidden;
            dgBatchNos.Columns[dgBatchNos.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);

        }
        private void initiateLoadBatch_DoWork(object sender, DoWorkEventArgs e)
        {
            loadFromQB();

        }
        public void loadFromQB()
        {
            QbConn h = new QbConn();
            h.createConnection();
            h.openConnection();
            if (!String.IsNullOrEmpty(searchno))
            {
                h.BuildBuildAssemblyQueryRqByRef(searchno);
            }
            else
            {
                h.BuildBuildAssemblyQueryRqAll();
            }
            h.getResponse();
            h.closeConnection();
            assemd = h.returnDataList<Interop.QBFC13.IBuildAssemblyRetList>();
            

        }
        private void finaliseLoadBatch_RunWorkerCompleted(object sender,
                                                   RunWorkerCompletedEventArgs e)
        {
            updateScreenList();
            this.lblProgress.Content = "Search Completed";
            this.pgrSearchPr.IsIndeterminate = false;
            
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
