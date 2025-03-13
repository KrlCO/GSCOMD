using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;
//using Path = System.IO.Path;
using System.IO;

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
            //InitializeComponent();
            //meConectSql = ConfigurationManager.ConnectionStrings["GSCOMD_2._0.Properties.Settings.GSCOMDConnectionString1"]?.ConnectionString;

            InitializeComponent();
            meConectSql = ConfigurationManager.ConnectionStrings["gscomd_2._0.properties.settings.gscomdconnectionstring1"]?.ConnectionString;

        }

        //private void MuestraAtencionCli()
        //{
        //    string consulta = "select * from TDDOCU_ATEN";

        //    SqlDataAdapter adapter = new SqlDataAdapter(consulta, meConectSql);

        //    using (adapter)
        //    {
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);

        //        listaAtencion.DisplayMemberPath = "CO_USUA_CREA";
        //        listaAtencion.SelectedValuePath = "CO_ATEN";
        //        listaAtencion.ItemsSource = dt.DefaultView;
        //    }
        //}


        private void txtCodeTrab_validacion(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (int.TryParse(txtCodeTrab.Text, out int codigo))
                {
                    (string code,string nom) = consultaTrabajador(codigo);
                    lblNombTrab.Content = nom.ToString();
                    consultarImagen(code);
                }
            }
        }

        private (string, string) consultaTrabajador(int codigo) {
            try {
                using (SqlConnection conn = new SqlConnection(meConectSql))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TCASIG_Q02", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ISCO_TRAB", codigo);
                        cmd.Parameters.AddWithValue("@ISCO_COME", "01");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //string codeTrab = reader[0].ToString();
                                //string nombTrab = reader[1].ToString();

                                //return (reader[0].ToString(), reader[1].ToString());
                                return (reader["CO_TRAB"].ToString(), reader["NO_PERS"].ToString());
                                //MessageBox.Show($"{codeTrab.ToString()} / {nombTrab.ToString()}");
                            }
                            else
                            {
                                MessageBox.Show("NO TIENE ASIGNACION", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                imgTrab.Source = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex){ 
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ("", "");
        }

        private void consultarImagen(string codigo)
        {
            string networkPath = @"\\10.10.10.99\gssist\FOTO";
            string user = "compartido";
            string pass = "P3ruBu$2024";
            string comand = $"net use {networkPath} /user:{user} {pass}";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C " + comand)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    string nombreImagen = "T" + codigo.PadLeft(11, '0') + ".jpg";
                    string rutaImagen = Path.Combine(networkPath, nombreImagen);

                    if (File.Exists(rutaImagen))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(rutaImagen);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        imgTrab.Source = bitmap;
                    }
                    else
                    {
                        MessageBox.Show("Imagen no encontrada para el trabajador.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        imgTrab.Source = null;
                    }
                }
            }
            catch (Exception ex){
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


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
