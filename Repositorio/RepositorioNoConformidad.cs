using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WEBSGI.Models;

namespace WEBSGI.Repositorio
{
    public class RepositorioNoConformidad
    {


        public List<NoConformidad> TraerNoConformidades()
        {
            return TraerPorTipo("NOCONFORMIDAD");
        }

        public List<NoConformidad> TraerOportunidadesMejora()
        {
            return TraerPorTipo("OPCIONMEJORA");
        }

        private List<NoConformidad> TraerPorTipo(string tipo)
        {
            List<NoConformidad> lista = new List<NoConformidad>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                // Determinar el período de la auditoría según la fecha actual
                DateTime now = DateTime.Now;
                DateTime start, end;
                int year = now.Year;
                int month = now.Month;

                if (month == 3)
                {
                    start = new DateTime(year - 1, 4, 1);
                    end = new DateTime(year, 3, 31);
                }
                else if (month < 4)
                {
                    start = new DateTime(year - 1, 4, 1);
                    end = new DateTime(year, 2, DateTime.DaysInMonth(year, 2));
                }
                else
                {
                    start = new DateTime(year, 4, 1);
                    end = new DateTime(year + 1, 2, DateTime.DaysInMonth(year + 1, 2));
                }

                string query = "SELECT * FROM CNOCONFORMIDAD WHERE FECHA >= @start AND FECHA <= @end AND TIPO = @tipo ORDER BY ID DESC";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@start", start);
                comando.Parameters.AddWithValue("@end", end);
                comando.Parameters.AddWithValue("@tipo", tipo);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    NoConformidad u = new NoConformidad();
                    u.ID = reader.GetInt32(0);
                    u.NUMERO = reader.GetString(1);
                    u.FECHA = reader.GetDateTime(2);
                    u.ISO = reader.GetString(3);
                    u.NC = reader.GetString(4);
                    u.REFIEREASECTOR = reader.GetString(5);
                    u.PROCEDIMIENTO = reader.GetString(6);
                    u.ORIGEN = reader.GetString(7);
                    u.DESCRIPCION = reader.GetString(8);
                    u.CAUSA = reader.GetString(9);
                    u.CAUSAACCIONINMEDIATA = reader.GetString(10);
                    u.RESPONSABLE = reader.GetString(11);
                    u.PLAZOCIERRE = reader.GetDateTime(12);
                    u.CERRADAENFECHA = reader.GetString(13);
                    u.ACCIONCORRECTIVA = reader.GetString(14);
                    u.ACCIONRESPONSABLE = reader.GetString(15);
                    u.ACCIONPLAZOCIERRE = reader.GetDateTime(16);
                    u.ACCIONCERRADAENFECHA = reader.GetString(17);
                    u.ESTADO = reader.GetString(18);
                    u.CIERRE = reader.GetDateTime(19);
                    u.OBSERVACIONES = reader.GetString(20);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
        }


        public List<NoConformidad> TraerOportunidadMejoras()
        {
            List<NoConformidad> LISTA = new List<NoConformidad>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                SqlCommand _comando = new SqlCommand("ListarOpcionMejora", conexion);
                _comando.CommandType = System.Data.CommandType.StoredProcedure;


                try
                {


                    SqlDataReader reader = _comando.ExecuteReader();
                    while (reader.Read())
                    {
                        NoConformidad u = new NoConformidad();
                        u.ID = reader.GetInt32(0);
                        u.NUMERO = reader.GetString(1);
                        u.FECHA = reader.GetDateTime(2);
                        u.ISO = reader.GetString(3);
                        u.NC = reader.GetString(4);
                        u.REFIEREASECTOR = reader.GetString(5);
                        u.PROCEDIMIENTO = reader.GetString(6);
                        u.ORIGEN = reader.GetString(7);
                        u.DESCRIPCION = reader.GetString(8);
                        u.CAUSA = reader.GetString(9);
                        u.CAUSAACCIONINMEDIATA = reader.GetString(10);
                        u.RESPONSABLE = reader.GetString(11);
                        u.PLAZOCIERRE = reader.GetDateTime(12);
                        u.CERRADAENFECHA = reader.GetString(13);
                        u.ACCIONCORRECTIVA = reader.GetString(14);
                        u.ACCIONRESPONSABLE = reader.GetString(15);
                        u.ACCIONPLAZOCIERRE = reader.GetDateTime(16);
                        u.ACCIONCERRADAENFECHA = reader.GetString(17);
                        u.ESTADO = reader.GetString(18);
                        u.CIERRE = reader.GetDateTime(19);
                        u.OBSERVACIONES = reader.GetString(20);
                        LISTA.Add(u);

                    }
                }
                catch (Exception ex)
                {

                }

                conexion.Close();
            }
            return LISTA;
        }


        public List<NoConformidad> TraerNoConformidadesFiltradas(DateTime desde, DateTime hasta)
        {
            return TraerPorTipoYFechas("NOCONFORMIDAD", desde, hasta);
        }

        public List<NoConformidad> TraerOportunidadesMejoraFiltradas(DateTime desde, DateTime hasta)
        {
            return TraerPorTipoYFechas("OPCIONMEJORA", desde, hasta);
        }

        private List<NoConformidad> TraerPorTipoYFechas(string tipo, DateTime desde, DateTime hasta)
        {
            List<NoConformidad> lista = new List<NoConformidad>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                // Consulta que filtra por fecha y tipo
                string query = "SELECT * FROM CNOCONFORMIDAD WHERE FECHA >= @desde AND FECHA <= @hasta AND TIPO = @tipo ORDER BY ID DESC";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@desde", desde);
                comando.Parameters.AddWithValue("@hasta", hasta);
                comando.Parameters.AddWithValue("@tipo", tipo);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    NoConformidad nc = new NoConformidad();
                    nc.ID = reader.GetInt32(0);
                    nc.NUMERO = reader.GetString(1);
                    nc.FECHA = reader.GetDateTime(2);
                    nc.ISO = reader.GetString(3);
                    nc.NC = reader.GetString(4);
                    nc.REFIEREASECTOR = reader.GetString(5);
                    nc.PROCEDIMIENTO = reader.GetString(6);
                    nc.ORIGEN = reader.GetString(7);
                    nc.DESCRIPCION = reader.GetString(8);
                    nc.CAUSA = reader.GetString(9);
                    nc.CAUSAACCIONINMEDIATA = reader.GetString(10);
                    nc.RESPONSABLE = reader.GetString(11);
                    nc.PLAZOCIERRE = reader.GetDateTime(12);
                    nc.CERRADAENFECHA = reader.GetString(13);
                    nc.ACCIONCORRECTIVA = reader.GetString(14);
                    nc.ACCIONRESPONSABLE = reader.GetString(15);
                    nc.ACCIONPLAZOCIERRE = reader.GetDateTime(16);
                    nc.ACCIONCERRADAENFECHA = reader.GetString(17);
                    nc.ESTADO = reader.GetString(18);
                    nc.CIERRE = reader.GetDateTime(19);
                    nc.OBSERVACIONES = reader.GetString(20);
                    lista.Add(nc);
                }
                conexion.Close();
            }
            return lista;
        }



        public NoConformidad Buscar(int ID)
        {
            NoConformidad u = new NoConformidad();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from CNOCONFORMIDAD WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("ID", ID);  

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.NUMERO = reader.IsDBNull(1) ? null : reader.GetString(1);
                    u.FECHA = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2); // Si FECHA es opcional, declara la propiedad como DateTime?
                    u.ISO = reader.IsDBNull(3) ? null : reader.GetString(3);
                    u.NC = reader.IsDBNull(4) ? null : reader.GetString(4);
                    u.REFIEREASECTOR = reader.IsDBNull(5) ? null : reader.GetString(5);
                    u.PROCEDIMIENTO = reader.IsDBNull(6) ? null : reader.GetString(6);
                    u.ORIGEN = reader.IsDBNull(7) ? null : reader.GetString(7);
                    u.DESCRIPCION = reader.IsDBNull(8) ? null : reader.GetString(8);
                    u.CAUSA = reader.IsDBNull(9) ? null : reader.GetString(9);
                    u.CAUSAACCIONINMEDIATA = reader.IsDBNull(10) ? null : reader.GetString(10);
                    u.RESPONSABLE = reader.IsDBNull(11) ? null : reader.GetString(11);
                    u.PLAZOCIERRE = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12); // O usa DateTime? si es opcional
                    u.CERRADAENFECHA = reader.IsDBNull(13) ? null : reader.GetString(13);
                    u.ACCIONCORRECTIVA = reader.IsDBNull(14) ? null : reader.GetString(14);
                    u.ACCIONRESPONSABLE = reader.IsDBNull(15) ? null : reader.GetString(15);
                    u.ACCIONPLAZOCIERRE = reader.IsDBNull(16) ? DateTime.MinValue : reader.GetDateTime(16);
                    u.ACCIONCERRADAENFECHA = reader.IsDBNull(17) ? null : reader.GetString(17);
                    u.ESTADO = reader.IsDBNull(18) ? null : reader.GetString(18);
                    u.CIERRE = reader.IsDBNull(19) ? DateTime.MinValue : reader.GetDateTime(19);
                    u.OBSERVACIONES = reader.IsDBNull(20) ? null : reader.GetString(20);

                }
                conexion.Close();
            }
            return u;
        }

        public int Agregar(NoConformidad C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = @"INSERT INTO CNOCONFORMIDAD 
            (NUMERO, FECHA, ISO, NC, REFIEREASECTOR, PROCEDIMIENTO, ORIGEN, DESCRIPCION, CAUSA, 
             CAUSAACCIONINMEDIATA, RESPONSABLE, PLAZOCIERRE, CERRADAENFECHA, ACCIONCORRECTIVA, 
             ACCIONRESPONSABLE, ACCIONPLAZOCIERRE, ACCIONCERRADAENFECHA, ESTADO, CIERRE, OBSERVACIONES, TIPO)
            VALUES
            (@NUMERO, @FECHA, @ISO, @NC, @REFIEREASECTOR, @PROCEDIMIENTO, @ORIGEN, @DESCRIPCION, @CAUSA, 
             @CAUSAACCIONINMEDIATA, @RESPONSABLE, @PLAZOCIERRE, @CERRADAENFECHA, @ACCIONCORRECTIVA, 
             @ACCIONRESPONSABLE, @ACCIONPLAZOCIERRE, @ACCIONCERRADAENFECHA, @ESTADO, @CIERRE, @OBSERVACIONES, @TIPO)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NUMERO", C.NUMERO);
                    cmd.Parameters.AddWithValue("@FECHA", C.FECHA);
                    cmd.Parameters.AddWithValue("@ISO", C.ISO);
                    cmd.Parameters.AddWithValue("@NC", C.NC);
                    cmd.Parameters.AddWithValue("@REFIEREASECTOR", C.REFIEREASECTOR);
                    cmd.Parameters.AddWithValue("@PROCEDIMIENTO", C.PROCEDIMIENTO);
                    cmd.Parameters.AddWithValue("@ORIGEN", C.ORIGEN);
                    cmd.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION);
                    cmd.Parameters.AddWithValue("@CAUSA", C.CAUSA);
                    cmd.Parameters.AddWithValue("@CAUSAACCIONINMEDIATA", C.CAUSAACCIONINMEDIATA);
                    cmd.Parameters.AddWithValue("@RESPONSABLE", C.RESPONSABLE);

                    cmd.Parameters.AddWithValue("@PLAZOCIERRE", C.PLAZOCIERRE);

                    cmd.Parameters.AddWithValue("@CERRADAENFECHA", C.CERRADAENFECHA);
                    cmd.Parameters.AddWithValue("@ACCIONCORRECTIVA", C.ACCIONCORRECTIVA);
                    cmd.Parameters.AddWithValue("@ACCIONRESPONSABLE", C.ACCIONRESPONSABLE);

                    cmd.Parameters.AddWithValue("@ACCIONPLAZOCIERRE", C.ACCIONPLAZOCIERRE);

                    cmd.Parameters.AddWithValue("@ACCIONCERRADAENFECHA", C.ACCIONCERRADAENFECHA);
                    cmd.Parameters.AddWithValue("@ESTADO", C.ESTADO);
                    cmd.Parameters.AddWithValue("@CIERRE", C.CIERRE);
                    cmd.Parameters.AddWithValue("@OBSERVACIONES", C.OBSERVACIONES);
                    cmd.Parameters.AddWithValue("@TIPO", C.TIPO);

                    ret = cmd.ExecuteNonQuery();
                }
            }
            return ret;
        }

        public int Modificar(NoConformidad C, int ID)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = @"UPDATE CNOCONFORMIDAD SET 
            NUMERO = @NUMERO,
            FECHA = @FECHA,
            ISO = @ISO,
            NC = @NC,
            REFIEREASECTOR = @REFIEREASECTOR,
            PROCEDIMIENTO = @PROCEDIMIENTO,
            ORIGEN = @ORIGEN,
            DESCRIPCION = @DESCRIPCION,
            CAUSA = @CAUSA,
            CAUSAACCIONINMEDIATA = @CAUSAACCIONINMEDIATA,
            RESPONSABLE = @RESPONSABLE,
            PLAZOCIERRE = @PLAZOCIERRE,
            CERRADAENFECHA = @CERRADAENFECHA,
            ACCIONCORRECTIVA = @ACCIONCORRECTIVA,
            ACCIONRESPONSABLE = @ACCIONRESPONSABLE,
            ACCIONPLAZOCIERRE = @ACCIONPLAZOCIERRE,
            ACCIONCERRADAENFECHA = @ACCIONCERRADAENFECHA,
            ESTADO = @ESTADO,
            CIERRE = @CIERRE,
            OBSERVACIONES = @OBSERVACIONES
            WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NUMERO", C.NUMERO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FECHA", C.FECHA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ISO", C.ISO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NC", C.NC ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@REFIEREASECTOR", C.REFIEREASECTOR ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PROCEDIMIENTO", C.PROCEDIMIENTO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ORIGEN", C.ORIGEN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DESCRIPCION", C.DESCRIPCION ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAUSA", C.CAUSA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CAUSAACCIONINMEDIATA", C.CAUSAACCIONINMEDIATA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RESPONSABLE", C.RESPONSABLE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PLAZOCIERRE", C.PLAZOCIERRE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CERRADAENFECHA", C.CERRADAENFECHA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACCIONCORRECTIVA", C.ACCIONCORRECTIVA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACCIONRESPONSABLE", C.ACCIONRESPONSABLE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACCIONPLAZOCIERRE", C.ACCIONPLAZOCIERRE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ACCIONCERRADAENFECHA", C.ACCIONCERRADAENFECHA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ESTADO", C.ESTADO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CIERRE", C.CIERRE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OBSERVACIONES", C.OBSERVACIONES ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ID", ID);

                    ret = cmd.ExecuteNonQuery();


                    ret = cmd.ExecuteNonQuery();
                }
            }
            return ret;
        }


        public Normas TraerNormaPorId(int ID)
        {
            Normas u = new Normas();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from CNORMAS WHERE ID = @ID"; 
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@ID", ID);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.NORMA = reader.GetString(1);
                    u.DESCRIPCION = reader.GetString(2);
                }
                conexion.Close();
            }
            return u;
        }

        public List<Normas> TraerNromas()
        {
            List<Normas> lista = new List<Normas> ();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select * from CNORMAS";
                SqlCommand comando = new SqlCommand(query, conexion);


                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Normas u = new Normas();
                     u.ID= reader.GetInt32(0);
                    u.NORMA = reader.GetString(1);
                    u.DESCRIPCION = reader.GetString(2);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
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


        public NoConformidad AbiertaEnFechaCierre(List<NoConformidad> lista)
        {
            DateTime hoy = DateTime.Now;

            return lista.FirstOrDefault(item =>
                item.ESTADO.Equals("ABIERTA", StringComparison.OrdinalIgnoreCase) &&
                item.CIERRE.HasValue &&
                (item.CIERRE.Value <= hoy || (item.CIERRE.Value - hoy).TotalDays <= 3)
            );
        }

    }
}