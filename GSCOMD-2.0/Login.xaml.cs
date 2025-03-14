﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace GSCOMD_2._0
{
    public partial class Login : Window
    {
        private string connectionString;

        public Login()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["GSCOMD_2._0.Properties.Settings.GSCOMDConnectionString"]?.ConnectionString;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Debe ingresar usuario y contraseña.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string codigoComedor = ValidarCredenciales(username, password);

            if (codigoComedor != null)
            {
                MessageBox.Show($"¡Bienvenido!", "Acceso concedido", MessageBoxButton.OK, MessageBoxImage.Information);

                if (codigoComedor == "01") // Comedor Lima
                {
                    new MainWindow(codigoComedor).Show();
                }
                else if (codigoComedor == "02") // Comedor Ica
                {
                    new ComedorIca().Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ValidarCredenciales(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_TMUSUA_001", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ISCO_USUA", username);
                        cmd.Parameters.AddWithValue("@ISNO_CLAV", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string codigoUsuario = reader["CO_USUA"].ToString();

                                // Determinar el código del comedor
                                if (codigoUsuario.StartsWith("COMELIMA"))
                                    return "01";
                                else if (codigoUsuario.StartsWith("COMEICA"))
                                    return "02";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }
    }
}
