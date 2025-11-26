using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WEBSGI.Models;

namespace WEBSGI.Repositorio
{
    public class RepositorioConfigOrganizacion
    {

        // SELECT [ID], [NORMA], [DESCRIPCION] FROM [CNORMAS] ORDER BY [NORMA]

        public  List<Normas> TraerNormas()
        {
            List<Normas> LISTA = new List<Normas>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from CNORMAS ORDER BY ID DESC";
                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Normas u = new Normas();
                    u.ID = reader.GetInt32(0);
                    u.NORMA = reader.GetString(1);
                    u.DESCRIPCION = reader.GetString(2);
                    LISTA.Add(u);
                }
                conexion.Close();
            }
            return LISTA;
        }

        public  List<Puestos> TraerPuestos()
        {
            List<Puestos> LISTA = new List<Puestos>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
            string query =   "SELECT* FROM CPUESTOPADRE WHERE ISNULL(PUESTO, '') <> '' ORDER BY PUESTO ASC";
                //string query = "select * from CPUESTOPADRE ORDER BY PUESTO asc";


                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Puestos u = new Puestos();
                    u.ID = reader.GetInt32(0);
                    u.PUESTO = reader.GetString(1);
                    u.IDPUESTOPADRE = reader.GetInt32(2);
                    u.PUESTOPADRE = reader.GetString(3);
                    u.DESCRIPCION = reader.GetString(4);
                    u.FUNCIONES = reader.GetString(5);
                    u.PERFIL = reader.GetString(6);
                    u.REQUISITOS = reader.GetString(7);
                    LISTA.Add(u);
                }
                conexion.Close();
            }
            return LISTA;
        }


        public List<Usuarios> TraerUsuarios()
        {
            List<Usuarios> lista = new List<Usuarios>();

            using (SqlConnection conexion = BD.obtenerConexion())
            {

                string query = "select ID,NOMBRE,USUARIO,CONTRASEÑA,NUMERO,ACTIVO,PUESTO,NIVEL from USUARIOS ORDER BY NOMBRE ASC";
                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Usuarios u = new Usuarios();
                    u.IdUsuario = reader.GetInt32(0);
                    u.Nombre = reader.IsDBNull(1) ? null : reader.GetString(1);
                    u.Usuario = reader.IsDBNull(2) ? null : reader.GetString(2);
                    u.Contrasena = reader.IsDBNull(3) ? null : reader.GetString(3);
                    u.Numero = reader.GetInt32(4);
                    u.Activo = reader.IsDBNull(5) ? null : reader.GetString(5); 
                    u.Puesto = reader.IsDBNull(6) ? null : reader.GetString(6);
                    u.Nivel = reader.IsDBNull(7) ? null : reader.GetString(7);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
        }


        public Normas buscarUNO(int ID)
        {
            Normas u = new Normas();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand("SELECT * FROM CNORMAS WHERE ID = @ID", conexion);
                comando.Parameters.AddWithValue("@ID", ID);

              

                SqlDataReader reader = comando.ExecuteReader();
                if (reader.Read()) // Use if instead of while as we expect only one result
                {
                    u.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                    u.NORMA = reader.GetString(reader.GetOrdinal("NORMA"));
                    u.DESCRIPCION = reader.GetString(reader.GetOrdinal("DESCRIPCION"));
                }
                conexion.Close();
            }
            return u;
        }

        public int Agregar(Normas C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand("INSERT INTO CNORMAS (NORMA, DESCRIPCION) VALUES (@NORMA, @DESCRIPCION)", conn);
                comando.Parameters.AddWithValue("@NORMA", C.NORMA);
                comando.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION);

              

                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public int Modificar(Normas C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "UPDATE CNORMAS SET NORMA = @NORMA, DESCRIPCION = @DESCRIPCION WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@ID", C.ID);
                comando.Parameters.AddWithValue("@NORMA", C.NORMA);
                comando.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION);

                

                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public int Eliminar(int ID)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand("DELETE FROM CNORMAS WHERE ID = @ID", conn);
                comando.Parameters.AddWithValue("@ID", ID);

              

                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

    }
}