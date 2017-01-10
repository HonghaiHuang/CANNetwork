using CANNetwork.DataClass;
using CVTSTS.DataClass;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace CANNetwork
{
    /// <summary>
    /// VehicleManagement.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleManagement : UserControl
    {
        public VehicleManagement()
        {
            InitializeComponent();
        }

        private void btnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel File | *.xls";
            bool? dialogResult = dialog.ShowDialog();

            if (dialogResult == true)
            {
               
                string Filepath = dialog.FileName;
               DataTable excelDt= ImportExcel.InsernExcelFile(Filepath,"车型");
                DG1.ItemsSource = excelDt.DefaultView;
            }
        }

        private void btnAdddataToaccess_Click(object sender, RoutedEventArgs e)
        {
            string[,] data=new string[8,7];
            if(txtModelName.Text!="")
            {
                AccessHelper.CreateOle(txtModelName.Text);
                DataTable DGDt = DataGridToDatable(DG1);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        data[i, j] = DGDt.Rows[i][j].ToString();
                    }
                }
                AccessHelper.InsertDataToTable_data(txtModelName.Text, data);
            }
        }

        /// <summary>
        /// DataGrid转Datable
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        public System.Data.DataTable DataGridToDatable(RadGridView dg)
        {
            try
            {

                System.Data.DataTable dt = null;

                System.Data.DataTable tmpdt = ((DataView)dg.ItemsSource).Table;
                return tmpdt;
            }
            catch (System.Exception ex)
            {
                throw ex;
                //  return null;
            }
        }


    }
}
