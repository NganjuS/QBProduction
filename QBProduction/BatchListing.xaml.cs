using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPBusinessObjects.WPF.Viewer;
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

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for BatchListing.xaml
    /// </summary>
    public partial class BatchListing : Window
    {
        public BatchListing()
        {
            try
            {
                InitializeComponent();
                crystalReportViewer1.Owner = this;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
            

        }
        public void DisplayReport()
        {
            ReportDocument reportDocument = new ReportDocument();
            string reportname = @"C:\QBProd\Reports\prodlisting.rpt";//Crystal Report\\
            reportDocument.Load(reportname);//BankRecon.pathlocation + 
            DataSet myData = new DataSet();
            MySql.Data.MySqlClient.MySqlConnection conn;
            MySql.Data.MySqlClient.MySqlCommand cmd;
            MySql.Data.MySqlClient.MySqlDataAdapter myAdapter;

            conn = new MySql.Data.MySqlClient.MySqlConnection();
            cmd = new MySql.Data.MySqlClient.MySqlCommand();
            myAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter();

            conn.ConnectionString = Controller.ReturnConnection();
            DateTime? dtf = this.dtFromDate.SelectedDate;
            DateTime? dt = this.dtToDate.SelectedDate;
            cmd.CommandText = $"SELECT * FROM qbproduction.bomrun where  bomrundate>='{dtf.Value.ToString("yyyy-MM-dd")}' and bomrundate <= '{dt.Value.ToString("yyyy-MM-dd")}'";
            cmd.Connection = conn;

            myAdapter.SelectCommand = cmd;
            myAdapter.Fill(myData);

            reportDocument.Load(reportname);
            reportDocument.Database.Tables[0].SetDataSource(myData.Tables[0]);
            this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
            
            
            //
            //reportDocument.SetDatabaseLogon("sa", "sa", "DESKTOP-VS66IPJ\\SQLSERVER2016", "Focus50A0", true);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtReportHeader;
            //  .SetParameterValue("Grouping 1", this.ddl_group1.SelectedValue); "
            
          //  reportDocument.SetParameterValue("datefrom", dtf.Value.ToShortDateString());
            //reportDocument.SetParameterValue("dateto", dt.Value.ToShortDateString());

            //this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
            //reportDocument.SetParameterValue("Period Year", (object)Form1.incomeStmt(RM.newdash));
            //reportDocument.SetParameterValue("{vwStocksPerWarehouse.Item}", (Object)"");
            //this.crystalReportViewer1.ViewerCore.RefreshReport();

            //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

        }
        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
        {
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in (SCRCollection)reportDocument.Database.Tables)
            {
                TableLogOnInfo logOnInfo = table.LogOnInfo;
                logOnInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(logOnInfo);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.crystalReportViewer1.ShowLogo = false;
                this.crystalReportViewer1.ToggleSidePanel = Constants.SidePanelKind.None;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
            
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            DateTime? dtf = this.dtFromDate.SelectedDate;
            DateTime? dt = this.dtToDate.SelectedDate;
            
            if (dtf != null && dt != null)
            {
                DisplayReport();
            }
        }
    }
}
