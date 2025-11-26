using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class BD
    {
        public static string Obtenerstring()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        }
        public static string ObtenerstringTSD()
        {
            return ConfigurationManager.ConnectionStrings["ConexionTSD"].ConnectionString;

        }


        public static SqlConnection obtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(Obtenerstring());
            conexion.Open();
            return conexion;
        }
        public static SqlConnection obtenerConexionTSD()
        {
            SqlConnection conexion = new SqlConnection(ObtenerstringTSD());
            conexion.Open();
            return conexion;
        }

    }
}