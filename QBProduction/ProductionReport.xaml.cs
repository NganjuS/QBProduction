
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPBusinessObjects.WPF.Viewer;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using System.Data;

namespace QBProduction
{
    /// <summary>
    /// Interaction logic for ProductionReport.xaml
    /// </summary>
    /// 
    
    public partial class ProductionReport : Window
    {
        string prodno;
        public ProductionReport(string prodno)
        {
            this.prodno = prodno;
            InitializeComponent();
            crystalReportViewer1.Owner = Window.GetWindow(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.crystalReportViewer1.ShowLogo = false;
            this.crystalReportViewer1.ToggleSidePanel = Constants.SidePanelKind.None;
    
            this.DisplayReport();
            
        }
        public void DisplayReport()
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

            cmd.CommandText = $"SELECT* FROM qbproduction.vwbomruns where bomrunref='{prodno}'";
            cmd.Connection = conn;

            myAdapter.SelectCommand = cmd;
            myAdapter.Fill(myData);

            reportDocument.Load(reportname);
            reportDocument.Database.Tables[0].SetDataSource(myData.Tables[0]);
            this.crystalReportViewer1.ViewerCore.ReportSource = (object)reportDocument;
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
        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
        {
            foreach (CrystalDecisions.CrystalReports.Engine.Table table in (SCRCollection)reportDocument.Database.Tables)
            {
                TableLogOnInfo logOnInfo = table.LogOnInfo;
                logOnInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(logOnInfo);
            }
        }


    }
}
