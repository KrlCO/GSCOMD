using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
//using System.

namespace GSCOMD_2._0
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private string meConectSql;

        public MainWindow()
        {
            InitializeComponent();

            
            meConectSql = ConfigurationManager.ConnectionStrings["GSCOMD_2._0.Properties.Settings.GSCOMDConnectionString1"]?.ConnectionString;


           
        }

        //Metodo para la generacion de atencion comensal
        private void MuestraAtencionCli()
        {
            using (SqlConnection conn = new SqlConnection(meConectSql))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TMCOME_002", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        //listaAtencion.DisplayMemberPath = "NOMBRE";  // Mostrar los nombres en el ComboBox
                        //listaAtencion.SelectedValuePath = "CODIGO";  // Al seleccionar, guardar el código
                        //listaAtencion.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al obtener datos: {ex.Message}");
                }
            }
        }

        // Método para verificar si el trabajador tiene asignación
        private void VerificarAsignacionTrabajador(string codigoTrabajador)
        {
            using (SqlConnection conn = new SqlConnection(meConectSql))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TMCOME_002", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodigoTrabajador", codigoTrabajador);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            lblAsignacion.Content = "Asignado";  // Si tiene asignación
                        }
                        else
                        {
                            lblAsignacion.Content = "No Asignado";  // Si no tiene asignación
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al consultar la asignación: {ex.Message}");
                }
            }
        }

        // Evento que se activa cuando se escribe en el TextBox
        private void txtCodigoTrabajador_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodigoTrabajador.Text))
            {
                VerificarAsignacionTrabajador(txtCodigoTrabajador.Text);
            }
            else
            {
                lblAsignacion.Content = ""; // Limpiar el label si no hay texto
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (listaAtencion.SelectedItem != null)
            //{
            //    DataRowView row = listaAtencion.SelectedItem as DataRowView;
            //    string codigo = row["CODIGO"].ToString();
            //    string nombre = row["NOMBRE"].ToString();

            //    MessageBox.Show($"Seleccionaste: {nombre} (Código: {codigo})");
            //}
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Btn Funciona");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"Cadena de conexión: {meConectSql}");
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        //Tener en cuenta para el Post loggeo se debe mostrar esta opcion-  Seleccionar el comedor y la caja que tenga asignada el comedor
        //private void listaAtencion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //Probar que la conexion a la DB sea exitosa
        //private void TestConnection()
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(meConectSql))
        //        {
        //            conn.Open();
        //            MessageBox.Show("Conexión exitosa.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error de conexión: {ex.Message}");
        //    }
        //}

    }
}
