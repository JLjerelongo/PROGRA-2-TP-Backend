using Microsoft.Data.SqlClient;
using System.Data;

namespace CineCordobaWebApi.Utils
{
    public class DBHelper
    {
        private static DBHelper instancia = null;
        private SqlConnection conexion;

        private DBHelper()
        {
            conexion = new SqlConnection(@"Data Source=DESKTOP-R73QI63\\SQLEXPRESS;Initial Catalog=CINE_CORDOBA;Integrated Security=True;Trust Server Certificate=True");
        }

        public static DBHelper GetInstancia()
        {
            if (instancia == null)
                instancia = new DBHelper();
            return instancia;
        }

        public SqlConnection GetConnection()
        {
            if (conexion.State != ConnectionState.Open)
            {
                conexion.Open();
            }
            return conexion;
        }
        public DataTable ConsultarVista(string query)
        {
            DataTable tabla = new DataTable();
            using (var comando = new SqlCommand(query, GetConnection())) // Abre la conexión si está cerrada
            {
                try
                {
                    tabla.Load(comando.ExecuteReader());
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al consultar la vista: " + ex.Message);
                }
                finally
                {
                    if (conexion.State == ConnectionState.Open)
                        conexion.Close();
                }
            }
            return tabla;
        }

        public DataTable Consultar(string nombreSP, List<SqlParameter>? parameters = null)
        {
            DataTable tabla = new DataTable();
            using (SqlCommand comando = new SqlCommand())
            {
                try
                {
                    conexion.Open();
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.CommandText = nombreSP;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            comando.Parameters.AddWithValue(param.ParameterName, param.Value);
                    }

                    tabla.Load(comando.ExecuteReader());
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al consultar el procedimiento almacenado", ex);
                }
                finally
                {
                    if (conexion.State == ConnectionState.Open)
                    {
                        conexion.Close();
                    }
                }
            }
            return tabla;
        }
        public int EjecutarSQL(string nombreSP, List<SqlParameter>? parameters = null)
        {
            conexion.Open();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = nombreSP;

            if (parameters != null)
            {
                foreach (var param in parameters)
                    comando.Parameters.AddWithValue(param.ParameterName, param.Value);
            }

            int resultado = comando.ExecuteNonQuery();
            conexion.Close();
            return resultado;
        }
    }
}
