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
    /// Interaction logic for SelectMultiple.xaml
    /// </summary>
    public partial class SelectMultiple : Window
    {
        public SelectMultiple()
        {
            InitializeComponent();
        }
        public void PopulateGridList()
        {
            try
            {
                this.dgListMultiple.Columns.Clear();
                this.dgListMultiple.ItemsSource = ConvertBomToDataTable(Controller.GetBomsMass()).DefaultView;
                this.dgListMultiple.Columns[dgListMultiple.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                this.dgListMultiple.Columns[1].IsReadOnly = true;
                this.dgListMultiple.Columns[2].IsReadOnly = true;
                this.dgListMultiple.Columns[3].IsReadOnly = true;
                this.dgListMultiple.Columns[3].Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }



            //Boms[] dt = (Boms[])bomlist[0];

            //MessageBox.Show(dt[0].assemblyitem);

        }
        public static System.Data.DataTable ConvertBomToDataTable(IList<object[]> d)
        {
            DataTable dt = new DataTable("#tmptable");
            dt.Columns.Add("Select", typeof(Boolean));
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Uom", typeof(string));
            dt.Columns.Add("TranId", typeof(string));
            dt.Columns.Add("Qty To Produce", typeof(string));
            for (int i = 0; i < d.Count; i++)
            {
                
                dt.Rows.Add(false,d[i][0], d[i][1], d[i][2],0);

            }
            return dt;

        }

        private void dgListMultiple_Loaded(object sender, RoutedEventArgs e)
        {
            this.PopulateGridList();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (System.Data.DataRowView rowvw in this.dgListMultiple.Items)
            {
                double nn;
                
                if(double.TryParse(rowvw["Qty To Produce"].ToString(),out nn))
                {
                    if(nn > 0)
                    {
                        Production.getproductstoproduce[i][0] = rowvw["Item"].ToString();
                        Production.getproductstoproduce[i][1] = rowvw["TranId"].ToString();
                        Production.getproductstoproduce[i][2] = rowvw["Qty To Produce"].ToString();
                    }
                }
                i++;
            }
        }
    }
}
