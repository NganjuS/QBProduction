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
    /// Interaction logic for RawMaterialReport.xaml
    /// </summary>
    public partial class RawMaterialReport : Window
    {
        public RawMaterialReport()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.crystalReportViewer1.ShowLogo = false;
            this.crystalReportViewer1.ToggleSidePanel = Constants.SidePanelKind.None;

            
        }
        public void DisplayReport()
        {
            try
            {
                ReportDocument reportDocument = new ReportDocument();
                string reportname = @"C:\QBProd\Reports\rawmaterials.rpt";//Crystal Report\\

                DataSet myData = new DataSet();
                MySql.Data.MySqlClient.MySqlConnection conn;
                MySql.Data.MySqlClient.MySqlCommand cmd;
                MySql.Data.MySqlClient.MySqlDataAdapter myAdapter;

                conn = new MySql.Data.MySqlClient.MySqlConnection();
                cmd = new MySql.Data.MySqlClient.MySqlCommand();
                myAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter();

                conn.ConnectionString = Controller.ReturnConnection();

                DateTime? dt = this.dtTo.SelectedDate;
                cmd.CommandText = $"SELECT * FROM qbproduction.vwrawmaterials where  bomrundate = '{dt.Value.ToString("yyyy-MM-dd")}'";
                cmd.Connection = conn;

                myAdapter.SelectCommand = cmd;
                myAdapter.Fill(myData);

                reportDocument.Load(reportname);
                reportDocument.Database.Tables[0].SetDataSource(myData.Tables[0]);
                this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error running report, please make sure you've selected the date");
            }
            
            //
            //BankRecon.pathlocation + 
            /*this.SetDBLogonForReport(new ConnectionInfo()
            {
                ServerName = "QB",
                DatabaseName = "qbproduction",
                UserID = "root",
                Password = "Met@c1ty"
            }, reportDocument);*/

            //reportDocument.SetDatabaseLogon("sa", "sa", "DESKTOP-VS66IPJ\\SQLSERVER2016", "Focus50A0", true);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtReportHeader;
            //  .SetParameterValue("Grouping 1", this.ddl_group1.SelectedValue); "

            //reportDocument.SetParameterValue("bomrunref", prodno);


            //this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
            //reportDocument.SetParameterValue("Period Year", (object)Form1.incomeStmt(RM.newdash));
            //reportDocument.SetParameterValue("{vwStocksPerWarehouse.Item}", (Object)"");
            //this.crystalReportViewer1.ViewerCore.RefreshReport();

            //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayReport();
        }
    }
}
