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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPBusinessObjects.WPF.Viewer;

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for ProductionSummaryReport.xaml
    /// </summary>
    public partial class ProductionSummaryReport : Window
    {
        public ProductionSummaryReport()
        {
            InitializeComponent();
        }

        private void summWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.crystalReportViewer1.ShowLogo = false;
            this.crystalReportViewer1.ToggleSidePanel = Constants.SidePanelKind.None;
        }
        public void DisplayReport()
        {
            try
            {
                ReportDocument reportDocument = new ReportDocument();
                string reportname = @"C:\QBProd\Reports\prodsheet.rpt";//Crystal Report\\
                DataSet myData = new DataSet();
                MySql.Data.MySqlClient.MySqlConnection conn;
                MySql.Data.MySqlClient.MySqlCommand cmd;
                MySql.Data.MySqlClient.MySqlDataAdapter myAdapter;

                conn = new MySql.Data.MySqlClient.MySqlConnection();
                cmd = new MySql.Data.MySqlClient.MySqlCommand();
                myAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter();

                conn.ConnectionString = Controller.ReturnConnection();

                DateTime? dt = this.dtTo.SelectedDate;
                cmd.CommandText = $"SELECT * FROM qbproduction.vwbomruns where  bomrundate = '{dt.Value.ToString("yyyy-MM-dd")}'";
                cmd.Connection = conn;

                myAdapter.SelectCommand = cmd;
                myAdapter.Fill(myData);

                reportDocument.Load(reportname);
                reportDocument.Database.Tables[0].SetDataSource(myData.Tables[0]);
                this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running report, please make sure you've selected the date");
            }
        }
            private void btnView_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayReport();
        }
    }
}
