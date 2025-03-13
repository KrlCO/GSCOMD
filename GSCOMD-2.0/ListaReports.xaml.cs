using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace GSCOMD_2._0
{
    public partial class ListaReports : Window
    {
        private string connectionString = "Server=10.10.10.58;Database=GSCOMD;User Id=cn;Password=cnn2k7;";

        public ListaReports()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            CONVERT(VARCHAR(10), T1.FE_EMIS, 103) AS FECHA_EMISION,
                            T3.NO_TIPO_COMI, 
                            T6.NO_CATE, 
                            CASE WHEN T4.IM_SUBV = 0 THEN 'SIN SUBVENCIÓN' ELSE 'CON SUBVENCIÓN' END AS ASIG,    
                            T4.IM_SUBV,     
                            SUM(T4.NU_CANT) AS NU_CANT, 
                            SUM(T4.IM_SUBV) AS IM_SUBV_TOTA, 
                            SUM(T4.IM_EFEC) AS IM_EFEC,     
                            SUM(T4.IM_SUBV) + SUM(T4.IM_EFEC) AS IM_TOTA  
                        FROM TDDOCU_ATEN T4
                        LEFT JOIN TCDOCU_ATEN T1 ON T4.CO_ATEN = T1.CO_ATEN                    
                        INNER JOIN TMTIPO_COMI T3 ON T4.CO_TIPO_COMI = T3.CO_TIPO_COMI        
                        INNER JOIN TMCATE T6 ON T4.CO_CATE = T6.CO_CATE            
                        WHERE (T1.CO_CAJA_COME = '01')
                            AND (T1.CO_COME = '01')
                            AND T1.FE_EMIS >= DATEADD(DAY, 1 - DAY(GETDATE()), CAST(GETDATE() AS DATE))
                            AND T1.FE_EMIS < DATEADD(MONTH, 1, DATEADD(DAY, 1 - DAY(GETDATE()), CAST(GETDATE() AS DATE)))
                            AND T1.CO_ESTA_DOCU = 'ACT' 
                            AND T4.TI_SITU = 'ACT'       
                        GROUP BY CONVERT(VARCHAR(10), T1.FE_EMIS, 103), T3.NO_TIPO_COMI, T6.NO_CATE, T4.IM_SUBV
                        ORDER BY FECHA_EMISION ASC, T3.NO_TIPO_COMI ASC, T6.NO_CATE ASC;                        
                    ";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgLista.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí puedes manejar la selección de filas en el DataGrid si lo necesitas
        }

        private void MyDatePicker_SelectedDateChanged()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
