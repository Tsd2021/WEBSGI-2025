using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Web.Mvc;
using WEBSGI.Models;
using System.Text.RegularExpressions;

namespace WEBSGI.Repositorio
{
    public class RepositorioUsuarios
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

        public Usuarios Login(string usuario, string contrasenia)
        {
            Usuarios usu = null;

            string query = "SELECT ID, NOMBRE, USUARIO, CONTRASEÑA, NUMERO, NIVEL, PUESTO, FECHAACTUALIZACLAVE FROM USUARIOS WHERE USUARIO = @USUARIO AND CONTRASEÑA = @CONTRASEÑA ";
            string contraseniaEcriptada = Encriptar(contrasenia);


            using (SqlConnection conexion = BD.obtenerConexion())
            {


                using (SqlCommand command = new SqlCommand(query, conexion))
                {
                    command.Parameters.AddWithValue("@USUARIO", usuario);
                    command.Parameters.AddWithValue("@CONTRASEÑA", contraseniaEcriptada);


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usu = new Usuarios
                            {
                                IdUsuario = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0,
                                Nombre = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty,
                                Usuario = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty,
                                Contrasena = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty,
                                Numero = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0,
                                Nivel = !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty,
                                Puesto = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty,
                                FechaActualizaClave = !reader.IsDBNull(7) ? reader.GetDateTime(7) : DateTime.MinValue,

                            };


                        }
                    }
                    return usu;
                }
            }
        }
        #region
        public static byte[] Clave = Encoding.ASCII.GetBytes("210332830012");
        public static byte[] IV = Encoding.ASCII.GetBytes("Devjoker7.37hAES");

        public static string Encriptar(string Cadena)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(Cadena);
            byte[] encripted;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(Clave, IV), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }
            return Convert.ToBase64String(encripted);
        }
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
        #endregion


        public int ModificarContraseña(int NUMERO, string CONTRASEÑA)
        {
            int ret = 0;
            CONTRASEÑA = Encriptar(CONTRASEÑA);

            string query = "UPDATE USUARIOS SET CONTRASEÑA = @CONTRASEÑA, FECHAACTUALIZACLAVE = GETDATE() WHERE NUMERO = @NUMERO";
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand(query, conn);

                comando.Parameters.AddWithValue("@NUMERO", NUMERO);
                comando.Parameters.AddWithValue("@CONTRASEÑA", CONTRASEÑA);

                ret = comando.ExecuteNonQuery();
                
            }
            return ret;
        }

        public int OlvidoContraseña(Usuarios a)
        {
            int ret = 0;
            string query = "UPDATE USUARIOS SET OLVIDO = 1 WHERE NUMERO = @NUMERO";
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@NUMERO", a.Numero);
                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public string GenerateKey()
        {
            int longitud = 8;
            const string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder clave = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < longitud; i++)
            {
                int indice = rnd.Next(alfabeto.Length);
                clave.Append(alfabeto[indice]);
            }
            return clave.ToString();
        }



        public bool EnviarCorreoRecuperacion(string numero)
        {
            bool ret = false;
            Usuarios usu = new Usuarios();
            usu.Numero = Convert.ToInt32(numero);

            string clave = GenerateKey();

            if (ModificarContraseña(usu.Numero, clave) != 0)
            {
                if (OlvidoContraseña(usu) != 0)
                {
                    //TXTUSUARIOOLVIDO.Text = "";

                    ret = true;
                    //lblinfo.Text = "VERIFIQUE SU CASILLA DE CORREO.";
                    ////   Timer1.Enabled = true;
                    //Session["time"] = 0;
                }
                else
                {
                    return ret;
                    //MSG("ERROR, VERIFIQUE !!!!");
                }
            }
            else
            {
                return ret;
                //MSG("NO EXISTE EL USUARIO INGRESADO !!");
                //lblinfo.Visible = false;
            }

            return ret;
        }

        public bool ContrasenaSegura(string contraseñaSinVerificar)
        {
            //letras de la A a la Z, mayusculas y minusculas
            Regex letras = new Regex(@"[A-Z]");
            //digitos del 0 al 9
            Regex numeros = new Regex(@"[0-9]");
            //cualquier caracter del conjunto
            Regex caracEsp = new Regex("[!\"#\\$%&'()*+,-./:;=?@\\[\\]^_`{|}~]");

            bool cumpleCriterios = false;

            //si no contiene las letras, regresa false
            if (!letras.IsMatch(contraseñaSinVerificar))
            {
                return false;
            }
            /*

        //si no contiene los numeros, regresa false
        if (!numeros.IsMatch(contraseñaSinVerificar))
        {
            return false;
        }

        //si no contiene los caracteres especiales, regresa false
        if (!caracEsp.IsMatch(contraseñaSinVerificar))
        {
            return false;
        }
            */
            //si cumple con todo, regresa true
            return true;
        }
        public List<Puestos> ObtenerTodosLosPuestos()
        {
            List<Puestos> u = new List<Puestos>();

            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "SELECT * FROM CPUESTOPADRE WHERE PUESTO IS NOT NULL AND PUESTO <> ''";
                SqlCommand comando = new SqlCommand(query, conexion);

                try
                {
                    SqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        Puestos puesto = new Puestos
                        {
                            ID = reader.GetInt32(0),
                            PUESTO = reader.GetString(1),
                            IDPUESTOPADRE = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0,
                            PUESTOPADRE = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty,
                            DESCRIPCION = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty,
                            FUNCIONES = !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty,
                            PERFIL = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty,
                            REQUISITOS = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty
                        };

                        u.Add(puesto);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los puestos", ex);
                }
                finally
                {
                    conexion.Close();
                }
            }

            return u;
        }



        public Puestos BuscarPuesto(int ID)
        {
            Puestos u = new Puestos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from CPUESTOPADRE WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@ID", ID);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.PUESTO = reader.GetString(1);
                    u.IDPUESTOPADRE = reader.GetInt32(2);
                    u.PUESTOPADRE = reader.GetString(3);
                    u.DESCRIPCION = reader.GetString(4);
                    u.FUNCIONES = reader.GetString(5);
                    u.PERFIL = reader.GetString(6);
                    u.REQUISITOS = reader.GetString(7);
                }
                conexion.Close();
            }
            return u;
        }

        public int Agregar(Puestos C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "INSERT INTO CPUESTOPADRE (PUESTO, IDPUESTOPADRE, PUESTOPADRE, DESCRIPCION, FUNCIONES, PERFIL, REQUISITOS) " +
                               "VALUES (@PUESTO, @IDPUESTOPADRE, @PUESTOPADRE, @DESCRIPCION, @FUNCIONES, @PERFIL, @REQUISITOS)";

                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@PUESTO", C.PUESTO);
                    comando.Parameters.AddWithValue("@IDPUESTOPADRE", C.IDPUESTOPADRE);
                    comando.Parameters.AddWithValue("@PUESTOPADRE", C.PUESTOPADRE);
                    comando.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION);
                    comando.Parameters.AddWithValue("@FUNCIONES", C.FUNCIONES);
                    comando.Parameters.AddWithValue("@PERFIL", C.PERFIL);
                    comando.Parameters.AddWithValue("@REQUISITOS", C.REQUISITOS);

                    ret = comando.ExecuteNonQuery();
                }
                conn.Close();
            }
            return ret;
        }

        public int Modificar(Puestos C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "UPDATE CPUESTOPADRE SET PUESTO = @PUESTO, IDPUESTOPADRE = @IDPUESTOPADRE, " +
                               "PUESTOPADRE = @PUESTOPADRE, DESCRIPCION = @DESCRIPCION, FUNCIONES = @FUNCIONES, " +
                               "PERFIL = @PERFIL, REQUISITOS = @REQUISITOS WHERE ID = @ID";

                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@PUESTO", C.PUESTO);
                    comando.Parameters.AddWithValue("@IDPUESTOPADRE", C.IDPUESTOPADRE);
                    comando.Parameters.AddWithValue("@PUESTOPADRE", C.PUESTOPADRE);
                    comando.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION);
                    comando.Parameters.AddWithValue("@FUNCIONES", C.FUNCIONES);
                    comando.Parameters.AddWithValue("@PERFIL", C.PERFIL);
                    comando.Parameters.AddWithValue("@REQUISITOS", C.REQUISITOS);
                    comando.Parameters.AddWithValue("@ID", C.ID);

                    ret = comando.ExecuteNonQuery();
                }

                conn.Close();
            }
            return ret;
        }

        public int Eliminar(int ID)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "DELETE FROM CPUESTOPADRE WHERE ID = @ID";

                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@ID", ID);

                    ret = comando.ExecuteNonQuery();
                }

                conn.Close();
            }
            return ret;
        }


        public bool VerificarPermiso(int idPuesto, int acceso)
        {
            using (SqlConnection connection = BD.obtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM CPERMISOS WHERE IDPUESTO = @IdPuesto AND ACCESO = @Acceso";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);
                command.Parameters.AddWithValue("@Acceso", acceso);

               
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public List<Permisos> BuscarPorIdPuesto(int idPuesto)
        {
            List<Permisos> listaPermisos = new List<Permisos>();

            using (SqlConnection connection = BD.obtenerConexion())
            {
                string query = "SELECT * FROM CPERMISOS WHERE IDPUESTO = @IdPuesto";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);

          
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Permisos permiso = new Permisos
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        IDPUESTO = Convert.ToInt32(reader["IDPUESTO"]),
                        PUESTO = reader["PUESTO"].ToString(),
                        ACCESO = Convert.ToInt32(reader["ACCESO"])
                    };

                    listaPermisos.Add(permiso);
                }
            }

            return listaPermisos;
        }

        public void EliminarPorIdPuesto(int idPuesto)
        {
            using (SqlConnection connection = BD.obtenerConexion())
            {
                string query = "DELETE FROM CPERMISOS WHERE IDPUESTO = @IdPuesto";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);

              
                command.ExecuteNonQuery();
            }
        }

        public void EliminarPorIdPuestoYAcceso(int idPuesto, int acceso)
        {
            using (SqlConnection connection = BD.obtenerConexion())
            {
                string query = "DELETE FROM CPERMISOS WHERE IDPUESTO = @IdPuesto AND ACCESO = @Acceso";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);
                command.Parameters.AddWithValue("@Acceso", acceso);

               
                command.ExecuteNonQuery();
            }
        }

        public void AgregarPermiso(int idPuesto, int acceso)
        {
            using (SqlConnection connection = BD.obtenerConexion())
            {
                string query = "INSERT INTO CPERMISOS (IDPUESTO, PUESTO, ACCESO) " +
                               "SELECT @IdPuesto, PUESTO, @Acceso FROM CPUESTOPADRE WHERE ID = @IdPuesto";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);
                command.Parameters.AddWithValue("@Acceso", acceso);

                
                command.ExecuteNonQuery();
            }
        }
        public int TraerPuestoPorNombre(string puesto)
        {
            try
            {



                using (SqlConnection conexion = BD.obtenerConexion())
                {
                   
                    string query = "SELECT ID FROM CPUESTOPADRE WHERE PUESTO = @Puesto";
                    SqlCommand comando = new SqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@Puesto", puesto);

                    var resultado = comando.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }

            }
            catch (Exception ex)
            {
                // Es buena práctica registrar la excepción
                // Logger.LogError(ex.Message);

                // Retorna 0 o -1 para indicar que hubo un error
                return 0;
            }
        }


        public List<int> ObtenerAccesosDelUsuario(int idPuesto, string puesto)
        {
            List<int> accesos = new List<int>();

            using (SqlConnection connection = BD.obtenerConexion())
            {
                // Definimos la consulta SQL para obtener todos los accesos de un puesto
                string query = "SELECT ACCESO FROM CPERMISOS WHERE IDPUESTO = @IdPuesto AND PUESTO = @Puesto";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdPuesto", idPuesto);
                command.Parameters.AddWithValue("@Puesto", puesto);

                // Ejecutamos la consulta y obtenemos todos los valores de ACCESO
                SqlDataReader reader = command.ExecuteReader();

                // Leemos todos los resultados y los agregamos a la lista de accesos
                while (reader.Read())
                {
                    accesos.Add(reader.GetInt32(reader.GetOrdinal("ACCESO")));
                }
            }

            return accesos;
        }


        public bool ActualizarNivelUsuario(int id, string nivel)
        {
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "UPDATE USUARIOS SET NIVEL = @Nivel WHERE ID = @Id";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Nivel", nivel);
                comando.Parameters.AddWithValue("@Id", id);

                int filasAfectadas = comando.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

    }


}
