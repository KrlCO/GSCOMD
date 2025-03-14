using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace GSCOMD_2._0
{
    public partial class MainWindow : Window
    {
        private string meConectSql;
        private string codigoComedor;

        public MainWindow(string codigoComedor)
        {
            InitializeComponent();
            //this.codigoComedor = codigoComedor; // Guardar el código del comedor
            //meConectSql = ConfigurationManager.ConnectionStrings["GSCOMD_2._0.Properties.Settings.GSCOMDConnectionString"]?.ConnectionString;
        }

        //private void MuestraAtencionCli()
        //{
        //    using (SqlConnection conn = new SqlConnection(meConectSql))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand("SP_TCASIG_Q02", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@CodigoComedor", codigoComedor); // Pasar el código del comedor

        //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //                DataTable dt = new DataTable();
        //                adapter.Fill(dt);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error al obtener datos: {ex.Message}");
        //        }
        //    }
        //}
    }
}
