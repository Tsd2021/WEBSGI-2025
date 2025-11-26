using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WEBSGI.Models;

namespace WEBSGI.Repositorio
{
    public class RepositorioLog
    {

        public Usuarios BuscarUNO(int NUMERO)
        {
            Usuarios u = new Usuarios();
            u = null;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from USUARIOS WHERE NUMERO = @NUMERO";

                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@NUMERO", NUMERO);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    u = new Usuarios();
                    u.IdUsuario = reader.GetInt32(0);
                    u.Nombre = reader.GetString(1);
                    u.Usuario = reader.GetString(2);
                    u.Contrasena = Desencripta(reader.GetString(3));
                    u.Numero = reader.GetInt32(5);
                    u.FechaNacimiento = reader.GetDateTime(6);
                    u.VtoCarnetSalud = reader.GetDateTime(7);
                    u.VtoPorteArma = reader.GetDateTime(8);
                    u.VtoLibreta = reader.GetDateTime(9);
                    u.Uniforme = reader.GetString(10);
                    u.Direccion = reader.GetString(11);
                    u.Telefono = reader.GetString(12);
                    u.Olvido = reader.GetInt32(13);
                    u.Email = reader.GetString(14);
                    u.AdelantosFijos = reader.GetInt32(15);
                    u.Cedula = reader.GetInt32(16);
                    u.Nivel = reader.GetString(17);
                    u.Estado = reader.GetString(18);
                    u.Puesto = reader.GetString(20);
                    u.FechaActualizaClave = reader.GetDateTime(21);
                }
                conexion.Close();
            }
            return u;
        }
        public static byte[] Clave = Encoding.ASCII.GetBytes("210332830012");
        public static byte[] IV = Encoding.ASCII.GetBytes("Devjoker7.37hAES");

        public string Desencripta(string Cadena)
        {
            try
            {
                byte[] inputBytes = Convert.FromBase64String(Cadena);
                byte[] resultBytes = new byte[inputBytes.Length];
                string textoLimpio = String.Empty;
                RijndaelManaged cripto = new RijndaelManaged();
                using (MemoryStream ms = new MemoryStream(inputBytes))
                {
                    using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(Clave, IV), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(objCryptoStream, true))
                        {
                            textoLimpio = sr.ReadToEnd();
                        }
                    }
                }
                return textoLimpio;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public int Agregar(Log C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "INSERT INTO Log (USUARIO, ARCHIVOMOVIMIENTO, IPPUBLICA, IPPRIVADA, FECHA) " +
                               "VALUES (@usuario, @archivomovimiento, @ippublica, @ipprivada, @fecha)";
                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@usuario", C.USUARIO);
                    comando.Parameters.AddWithValue("@archivomovimiento", C.ARCHIVO_MOVIMIENTO);
                    comando.Parameters.AddWithValue("@ippublica", C.IPPUBLICA);
                    comando.Parameters.AddWithValue("@ipprivada", string.IsNullOrEmpty(C.IPPRIVADA) ? DBNull.Value : (object)C.IPPRIVADA);
                    comando.Parameters.AddWithValue("@fecha", C.FECHA);

                   
                    ret = comando.ExecuteNonQuery();
                }
                conn.Close();
            }
            return ret;
        }

        public int ControlUsuario(Usuarios u)
        {
            using (SqlConnection con = BD.obtenerConexion())
            {
                string query = "INSERT INTO CREGISTROSINICIOSESION (USUARIO, IDWEB, FECHA) VALUES (@idUsuario, @idWeb, @fecha)";
                using (SqlCommand comando = new SqlCommand(query, con))
                {
                    comando.Parameters.AddWithValue("@idUsuario", u.Numero);
                    comando.Parameters.AddWithValue("@idWeb", "SGI");
                    comando.Parameters.AddWithValue("@fecha", DateTime.Now);
                   
                    return comando.ExecuteNonQuery();
                }
            }
        }
        public  int TotalRegistros()
        {
            int TOTAL = 0;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select COUNT(*) from CREGISTROSINICIOSESION";
                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    TOTAL = reader.GetInt32(0);
                }
                conexion.Close();
            }
            return TOTAL;
        }
        public  int TotalRegistros3DIAS()
        {
            int total = 0;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM CREGISTROSINICIOSESION WHERE idWeb = 'SGI' AND fecha >= DATEADD(day, -7, GETDATE())";
                // Se asume que la columna de fecha se llama "fecha".
                // La condición DATEADD(day, -3, GETDATE()) filtra los registros desde 3 días atrás hasta hoy.
                SqlCommand comando = new SqlCommand(query, conexion);
                total = Convert.ToInt32(comando.ExecuteScalar());
                conexion.Close();
            }
            return total;
        }


        public List<Dictionary<string, object>> ObtenerRegistrosUltimoDia()
        {
            List<Dictionary<string, object>> registros = new List<Dictionary<string, object>>();

            using (SqlConnection con = BD.obtenerConexion())
            {
                string query = @"
    SELECT *
    FROM (
        SELECT 'Log' AS Origen, Usuario, ArchivoMovimiento, IpPublica, IpPrivada, CAST(Fecha AS DATETIME2) AS Fecha
        FROM Log
        WHERE Fecha >= @FechaLimite

        UNION ALL

        SELECT 'Registro' AS Origen, Usuario, NULL, NULL, NULL, CAST(Fecha AS DATETIME2) AS Fecha
        FROM CREGISTROSINICIOSESION
        WHERE Fecha >= @FechaLimite
    ) AS Combined
    ORDER BY Combined.Fecha DESC;
";


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FechaLimite", DateTime.Now.AddDays(-1)); 

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> registro = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                registro[reader.GetName(i)] = reader.GetValue(i);
                            }

                            registros.Add(registro);
                        }
                    }
                }
            }

            return registros;
        }






    }
}