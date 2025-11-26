using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WEBSGI.Models;
using WEBSGI.Models.ViewModel;

namespace WEBSGI.Repositorio
{
    public class RepositorioDocumentos
    {

        public int TotalDocumentosObsoletos()
        {
            int TOTAL = 0;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select COUNT(*) from CDOCUMENTOS WHERE ESTADO ='BAJA'";
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
        public int TotalDocumentosActivos()
        {
            int TOTAL = 0;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select COUNT(*) from CDOCUMENTOS WHERE ESTADO ='ALTA'";
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

        public int TotalDocumentos()
        {

            int TOTAL = 0;

            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select COUNT(*) from CDOCUMENTOS";

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

        public List<Documentos> TraerDocumentos(string titulo = "", string estado = "")
        {
            List<Documentos> LISTA = new List<Documentos>();

            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = @"
        SELECT ID, TITULO, REVISION, IDCARPETA, CARPETA, 
               IDTIPO, TIPO, IDESTADO, ESTADO, 
               OBSERVACIONES, NOMBREARCHIVO, ARCHIVO
        FROM CDOCUMENTOS
        WHERE (ESTADO = @Estado OR @Estado = '')
          AND (LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(TITULO, 'Á', 'A'), 'É', 'E'), 'Í', 'I'), 'Ó', 'O'), 'Ú', 'U')) LIKE '%' + LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@Titulo, 'Á', 'A'), 'É', 'E'), 'Í', 'I'), 'Ó', 'O'), 'Ú', 'U')) + '%')
        ORDER BY TITULO ASC"; // Aquí he corregido la posición de ORDER BY

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Estado", string.IsNullOrEmpty(estado) ? "" : estado);
                    comando.Parameters.AddWithValue("@Titulo", string.IsNullOrEmpty(titulo) ? "" : titulo);

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Documentos u = new Documentos
                            {
                                ID = reader.GetInt32(0),
                                TITULO = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                REVISION = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                IDCARPETA = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                CARPETA = reader.IsDBNull(4) ? "Sin carpeta" : reader.GetString(4),
                                IDTIPO = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                TIPO = reader.IsDBNull(6) ? "Desconocido" : reader.GetString(6),
                                IDESTADO = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                ESTADO = reader.IsDBNull(8) ? "Desconocido" : reader.GetString(8),
                                OBSERVACIONES = reader.IsDBNull(9) ? "Sin observaciones" : reader.GetString(9),
                                NOMBREARCHIVO = reader.IsDBNull(10) ? "Sin archivo" : reader.GetString(10),
                                ARCHIVO = reader.IsDBNull(11) ? null : (byte[])reader["ARCHIVO"]
                            };

                            LISTA.Add(u);
                        }
                    }
                }
            }
            return LISTA;
        }






        public int Agregar(Documentos C, HttpPostedFileBase fileUpload)
        {
            int ret = 0;

            string query = "INSERT INTO CDOCUMENTOS (TITULO, REVISION, IDCARPETA, CARPETA, IDTIPO, TIPO, IDESTADO, ESTADO, OBSERVACIONES, NOMBREARCHIVO, ARCHIVO) " +
                           "VALUES (@TITULO, @REVISION, @IDCARPETA, @CARPETA, @IDTIPO, @TIPO, @IDESTADO, @ESTADO, @OBSERVACIONES, @NOMBREARCHIVO, @ARCHIVO)";

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(fileUpload.InputStream))
            {
                fileData = binaryReader.ReadBytes(fileUpload.ContentLength);
            }

            using (SqlConnection conn = BD.obtenerConexion())


            using (SqlCommand comando = new SqlCommand(query, conn))
            {
                comando.Parameters.AddWithValue("@TITULO", C.TITULO);
                comando.Parameters.AddWithValue("@REVISION", C.REVISION);
                comando.Parameters.AddWithValue("@IDCARPETA", C.IDCARPETA);
                comando.Parameters.AddWithValue("@CARPETA", C.CARPETA);
                comando.Parameters.AddWithValue("@IDTIPO", C.IDTIPO);
                comando.Parameters.AddWithValue("@TIPO", C.TIPO);
                comando.Parameters.AddWithValue("@IDESTADO", C.IDESTADO);
                comando.Parameters.AddWithValue("@ESTADO", C.ESTADO);
                comando.Parameters.AddWithValue("@OBSERVACIONES", C.OBSERVACIONES);
                comando.Parameters.AddWithValue("@NOMBREARCHIVO", fileUpload.FileName);
                comando.Parameters.AddWithValue("@ARCHIVO", fileData);


                ret = comando.ExecuteNonQuery();
            }

            return ret;
        }
        public int Agregar(DocumentosVisibilidad C)
        {
            int ret = 0;

            string query = "INSERT INTO CDOCUMENTOVISIBILIDAD (IDDOCUMENTO, IDPUESTO, PUESTO) " +
                           "VALUES (@IDDOCUMENTO, @IDPUESTO, @PUESTO)";

            using (SqlConnection conn = BD.obtenerConexion())
            using (SqlCommand comando = new SqlCommand(query, conn))
            {
                comando.Parameters.AddWithValue("@IDDOCUMENTO", C.IDDOCUMENTO);
                comando.Parameters.AddWithValue("@IDPUESTO", C.IDPUESTO);
                comando.Parameters.AddWithValue("@PUESTO", C.PUESTO);

                ret = comando.ExecuteNonQuery();
            }

            return ret;
        }

        public string ObtenerCarpetaPorDocumento(int idDocumento)
        {
            string carpeta = string.Empty;
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "SELECT CARPETA FROM CDOCUMENTOS WHERE ID = @idDocumento";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idDocumento", idDocumento);
                    conexion.Open();
                    object result = comando.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        carpeta = result.ToString();
                    }
                }
                conexion.Close();
            }
            return carpeta;
        }

        public Documentos TraerCarpetaRutaPorIdCarpeta(int id)
        {
            Documentos u = new Documentos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select ID, CARPETA, IDCARPETA from CDOCUMENTOS WHERE IDCARPETA = @IDCARPETA";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@IDCARPETA", id);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                    u.IDCARPETA = reader.GetInt32(2);
                }
                conexion.Close();
            }
            return u;
        }
        public Documentos TraerInfoCarpetaPorIdDocumento(int ID)
        {
            Documentos u = new Documentos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select ID, CARPETA, IDCARPETA from CDOCUMENTOS WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@ID", ID);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                   u.IDCARPETA = reader.GetInt32(2);
                }
                conexion.Close();
            }
            return u;

        }

      


        public DocumentosCarpeta BuscarPorId(int ID)
        {
            DocumentosCarpeta u = new DocumentosCarpeta();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select * from CCARPETAS WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@ID", ID);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                    u.TIPO = reader.GetInt32(2);
                    u.IDCARPETA = reader.GetInt32(3);
                }
                conexion.Close();
            }
            return u;
        }


        public List<Documentos> TraerDocumentosPorCarpeta(int idCarpeta, string puesto, string tipoUsuario)
        {
            List<Documentos> lista = new List<Documentos>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                // Si el usuario es ADMINISTRADOR, se muestran todos los documentos de la carpeta
                // de lo contrario se filtra por el puesto asignado en la tabla de visibilidad.
                string query = "";

                if (tipoUsuario.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                {
                    query = @"SELECT D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                             D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
                      FROM CDOCUMENTOS AS D 
                      WHERE D.IDCARPETA = @idCarpeta 
                      ORDER BY D.TITULO ASC";
                }
                else
                {
                    query = @"SELECT D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                             D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
                      FROM CDOCUMENTOS AS D 
                      LEFT JOIN CDOCUMENTOVISIBILIDAD AS CD ON CD.IDDOCUMENTO = D.ID
                      WHERE D.IDCARPETA = @idCarpeta AND CD.PUESTO = @puesto 
                      GROUP BY D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                               D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
                      ORDER BY D.TITULO ASC";
                }

                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@idCarpeta", idCarpeta);
                if (!tipoUsuario.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                {
                    comando.Parameters.AddWithValue("@puesto", puesto);
                }
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Documentos doc = new Documentos();
                    doc.ID = reader.GetInt32(0);
                    doc.TITULO = reader.GetString(1);
                    doc.REVISION = reader.GetString(2);
                    doc.IDCARPETA = reader.GetInt32(3);
                    doc.CARPETA = reader.GetString(4);
                    doc.IDTIPO = reader.GetInt32(5);
                    doc.TIPO = reader.GetString(6);
                    doc.IDESTADO = reader.GetInt32(7);
                    doc.ESTADO = reader.GetString(8);
                    doc.OBSERVACIONES = reader.IsDBNull(9) ? null : reader.GetString(9);
                    doc.NOMBREARCHIVO = reader.GetString(10);
                    lista.Add(doc);
                }
                conexion.Close();
            }
            return lista;
        }


        public List<DocumentosCarpeta> TraerCarpetasEnteras()
        {
            List<DocumentosCarpeta> carpetas = new List<DocumentosCarpeta>();
            using (SqlConnection conecion = BD.obtenerConexion())
            {
                string query = "SELECT * FROM CCARPETAS";
                SqlCommand comando = new SqlCommand(query, conecion);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosCarpeta carpeta = new DocumentosCarpeta();
                    carpeta.ID = reader.GetInt32(0);
                    carpeta.CARPETA = reader.GetString(1);
                    carpeta.TIPO = reader.GetInt32(2);
                    carpeta.IDCARPETA = reader.GetInt32(3);
                    carpetas.Add(carpeta);
                }
                conecion.Close();
            }
            return carpetas;
        }
        public List<Documentos> TraerCarpetas()
        {
            List<Documentos> LISTA = new List<Documentos>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {

                string query = "SELECT MIN(IDCARPETA) AS IDCARPETA, CARPETA FROM CDOCUMENTOS GROUP BY CARPETA  ORDER BY CARPETA;";

                SqlCommand comando = new SqlCommand(query, conexion);
                

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Documentos u = new Documentos();
              
                    u.IDCARPETA = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    u.CARPETA = reader.IsDBNull(1) ? null : reader.GetString(1);
                   
                    LISTA.Add(u);

                }
                conexion.Close();
            }
            return LISTA;
        }

        public List<DocumentosCarpeta> ObtenerCarpetasPorRuta(string ruta)
        {
            if (string.IsNullOrEmpty(ruta))
                return new List<DocumentosCarpeta>();

            var carpetas = new List<DocumentosCarpeta>();

            // Obtener todas las carpetas desde el repositorio (suponiendo que trae todas las carpetas)
            var todasLasCarpetas = TraerCarpetas();

            string[] niveles = ruta.Split('/');
            string rutaAcumulada = "";

            foreach (var nivel in niveles)
            {
                rutaAcumulada += (rutaAcumulada == "" ? "" : "/") + nivel;

                // Buscar la carpeta en la lista obtenida del repositorio
                var carpeta = todasLasCarpetas.FirstOrDefault(c => c.CARPETA == rutaAcumulada);
                if (carpeta != null)
                {
                    // Convertimos `Documentos` en `DocumentosCarpeta`
                    var carpetaConvertida = new DocumentosCarpeta
                    {
                        ID = carpeta.ID,
                        CARPETA = carpeta.CARPETA,
                        IDCARPETA = carpeta.IDCARPETA
                    };

                    carpetas.Add(carpetaConvertida);
                }
            }

            return carpetas;
        }


        public List<DocumentosCarpeta> TraerCarpetasRaiz()
        {
            List<DocumentosCarpeta> lista = new List<DocumentosCarpeta>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                // Se asume que las carpetas raíz tienen IDCARPETA = 0 o NULL.
                string query = "SELECT * FROM CCARPETAS WHERE TIPO = 1 AND (IDCARPETA = 0 OR IDCARPETA IS NULL) ORDER BY CARPETA ASC";
                SqlCommand comando = new SqlCommand(query, conexion);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosCarpeta u = new DocumentosCarpeta();
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                    u.TIPO = reader.GetInt32(2);
                    u.IDCARPETA = reader.GetInt32(3);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
        }

        public List<DocumentosCarpeta> TraerSubCarpetas(int parentId)
        {
            List<DocumentosCarpeta> lista = new List<DocumentosCarpeta>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "SELECT * FROM CCARPETAS WHERE TIPO = 2 AND IDCARPETA = @parentId ORDER BY CARPETA ASC";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@parentId", parentId);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosCarpeta u = new DocumentosCarpeta();
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                    u.TIPO = reader.GetInt32(2);
                    u.IDCARPETA = reader.GetInt32(3);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
        }


      

        public List<DocumentosTipo> TraerDocumentoTipo()
        {
            List<DocumentosTipo> lista = new List<DocumentosTipo>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "SELECT * FROM CDOCUMENTOTIPO ORDER BY TIPO";
                SqlCommand comando = new SqlCommand(query, conexion);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosTipo dt = new DocumentosTipo();
                    dt.ID = reader.GetInt32(0);
                    dt.TIPO = reader.GetString(1);
                    lista.Add(dt);
                }
                conexion.Close();
            }
            return lista;
        }

        public DocumentosTipo BuscarTipo(int ID)
        {
            DocumentosTipo u = new DocumentosTipo();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
               string query = " select * from CDOCUMENTOTIPO WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@ID", ID); 

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.TIPO = reader.GetString(1);
                }
                conexion.Close();
            }
            return u;
        }

        public Documentos BuscarUno(int ID)
        {
            Documentos u = new Documentos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select * from CDOCUMENTOS WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@ID", ID); 

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.TITULO = reader.GetString(1);
                    u.REVISION = reader.GetString(2);
                    u.IDCARPETA = reader.GetInt32(3);
                    u.CARPETA = reader.GetString(4);
                    u.IDTIPO = reader.GetInt32(5);
                    u.TIPO = reader.GetString(6);
                    u.IDESTADO = reader.GetInt32(7);
                    u.ESTADO = reader.GetString(8);
                    u.OBSERVACIONES = reader.IsDBNull(9) ? null : reader.GetString(9);
                    u.NOMBREARCHIVO = reader.GetString(11);
                }
                conexion.Close();
            }
            return u;
        }
        public Documentos TraerDocumentoEstadoPorId(int ID)
        {

            Documentos u = new Documentos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select * from CDOCUMENTOS WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@ID", ID);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.TITULO = reader.GetString(1);
                    u.REVISION = reader.GetString(2);
                    u.IDCARPETA = reader.GetInt32(3);
                    u.CARPETA = reader.GetString(4);
                    u.IDTIPO = reader.GetInt32(5);
                    u.TIPO = reader.GetString(6);
                    u.IDESTADO = reader.GetInt32(7);
                    u.ESTADO = reader.GetString(8);
                    u.OBSERVACIONES = reader.GetString(9);
                    u.NOMBREARCHIVO = reader.GetString(11);
                }
                conexion.Close();
            }
            return u;
        }


     public List<Puestos> TraerPuestos()
{
    List<Puestos> lista = new List<Puestos>();
    using (SqlConnection conexion = BD.obtenerConexion())
    {
        // Filtramos para que sólo se traigan los registros donde PUESTO no es nulo ni vacío.
        string query = "SELECT * FROM CPUESTOPADRE WHERE ISNULL(PUESTO, '') <> '' ORDER BY PUESTO ASC";
        SqlCommand comando = new SqlCommand(query, conexion);

        SqlDataReader reader = comando.ExecuteReader();
        while (reader.Read())
        {
            Puestos u = new Puestos();
            u.ID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
            u.PUESTO = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            u.IDPUESTOPADRE = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
            u.PUESTOPADRE = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            u.DESCRIPCION = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            u.FUNCIONES = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            u.PERFIL = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
            u.REQUISITOS = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
            lista.Add(u);
        }
        conexion.Close();
    }
    return lista;
}


        public List<DocumentosVisibilidad> BuscarPorIdDocumento(int IDDOCUMENTO)
        {
            List<DocumentosVisibilidad> lista = new List<DocumentosVisibilidad>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = $"SELECT * FROM CDOCUMENTOVISIBILIDAD WHERE IDDOCUMENTO = @IDDOCUMENTO";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@IDDOCUMENTO", IDDOCUMENTO);
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    DocumentosVisibilidad u = new DocumentosVisibilidad();
                    u.ID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    u.IDDOCUMENTO = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    u.IDPUESTO = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                    u.PUESTO = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    lista.Add(u);
                }
                conexion.Close();
            }
            return lista;
        }

        public int Modificar(Documentos C, int ID)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "update CDOCUMENTOS SET TITULO = @TITULO,REVISION = @REVISION ,IDCARPETA = @IDCARPETA ,CARPETA = @CARPETA ,IDTIPO = @IDTIPO ,TIPO = @TIPO ,IDESTADO = @IDESTADO ,ESTADO = @ESTADO,OBSERVACIONES = @OBSERVACIONES WHERE ID = @ID";
                SqlCommand comando = new SqlCommand(query, conn);

                comando.Parameters.AddWithValue("@TITULO", C.TITULO);
                comando.Parameters.AddWithValue("@REVISION", C.REVISION);
                comando.Parameters.AddWithValue("@IDCARPETA", C.IDCARPETA);
                comando.Parameters.AddWithValue("@CARPETA", C.CARPETA);
                comando.Parameters.AddWithValue("@IDTIPO", C.IDTIPO);
                comando.Parameters.AddWithValue("@TIPO", C.TIPO);
                comando.Parameters.AddWithValue("@IDESTADO", C.IDESTADO);
                comando.Parameters.AddWithValue("@ESTADO", C.ESTADO);
                comando.Parameters.AddWithValue("@OBSERVACIONES", string.IsNullOrEmpty(C.OBSERVACIONES) ? DBNull.Value : (object)C.OBSERVACIONES);
                comando.Parameters.AddWithValue("@ID", ID);
               
                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }


        public int EliminarVisibilidad(int IDDOCUMENTO)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "DELETE FROM CDOCUMENTOVISIBILIDAD WHERE IDDOCUMENTO = @idDocumento";
                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@idDocumento", IDDOCUMENTO);
                
                    ret = comando.ExecuteNonQuery();
                }
                conn.Close();
            }
            return ret;
        }

        public int AgregarDocumento(Documentos C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "INSERT INTO CDOCUMENTOS " +
                               "(TITULO, REVISION, IDCARPETA, CARPETA, IDTIPO, TIPO, IDESTADO, ESTADO, OBSERVACIONES, NOMBREARCHIVO) " +
                               "VALUES (@titulo, @revision, @idCarpeta, @carpeta, @idTipo, @tipo, @idEstado, @estado, @observaciones, '')";
                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@titulo", C.TITULO);
                    comando.Parameters.AddWithValue("@revision", C.REVISION);
                    comando.Parameters.AddWithValue("@idCarpeta", C.IDCARPETA);
                    comando.Parameters.AddWithValue("@carpeta", C.CARPETA);
                    comando.Parameters.AddWithValue("@idTipo", C.IDTIPO);
                    comando.Parameters.AddWithValue("@tipo", C.TIPO);
                    comando.Parameters.AddWithValue("@idEstado", C.IDESTADO);
                    comando.Parameters.AddWithValue("@estado", C.ESTADO);
                    comando.Parameters.AddWithValue("@observaciones", C.OBSERVACIONES);

                    conn.Open();
                    ret = comando.ExecuteNonQuery();
                }
                conn.Close();
            }
            return ret;
        }


        public int AgregarDocumentosVisibilidad(DocumentosVisibilidad C)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "INSERT INTO CDOCUMENTOVISIBILIDAD (IDDOCUMENTO, IDPUESTO, PUESTO) " +
                               "VALUES (@iddocumento, @idpuesto, @puesto)";
                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@iddocumento", C.IDDOCUMENTO);
                    comando.Parameters.AddWithValue("@idpuesto", C.IDPUESTO);
                    comando.Parameters.AddWithValue("@puesto", C.PUESTO);
                    conn.Open();
                    ret = comando.ExecuteNonQuery();
                }
                conn.Close();
            }
            return ret;
        }



        public void AGREGARIMAGEN(int ID, HttpPostedFileBase fileUpload)
        {
            // Leer el contenido del archivo en un arreglo de bytes
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(fileUpload.InputStream))
            {
                fileData = binaryReader.ReadBytes(fileUpload.ContentLength);
            }

            using (SqlConnection con = BD.obtenerConexion())
            {
                string query = "UPDATE CDOCUMENTOS SET ARCHIVO = @Pic, NOMBREARCHIVO = @NOMBREARCHIVO WHERE ID = @ID";
                using (SqlCommand com = new SqlCommand(query, con))
                {
                    com.Parameters.AddWithValue("@NOMBREARCHIVO", fileUpload.FileName);
                    com.Parameters.AddWithValue("@ID", ID);
                    com.Parameters.AddWithValue("@Pic", fileData);
                    
                    com.ExecuteNonQuery();
                }
                con.Close();
            }
        }



        public Documentos buscarULTIMO()
        {
            Documentos u = new Documentos();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = "select TOP 1  * from CDOCUMENTOS ORDER BY ID DESC";
                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.TITULO = reader.GetString(1);
                    u.REVISION = reader.GetString(2);
                    u.IDCARPETA = reader.GetInt32(3);
                    u.CARPETA = reader.GetString(4);
                    u.IDTIPO = reader.GetInt32(5);
                    u.TIPO = reader.GetString(6);
                    u.IDESTADO = reader.GetInt32(7);
                    u.ESTADO = reader.GetString(8);
                    u.OBSERVACIONES = reader.GetString(9);
                    u.NOMBREARCHIVO = reader.GetString(11);
                }
                conexion.Close();
            }
            return u;
        }

      

        public int ModificarTipo(DocumentosTipo dc)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "UPDATE CDOCUMENTOTIPO SET TIPO = @tipo WHERE ID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@tipo",dc.TIPO);
                    cmd.Parameters.AddWithValue("@id",dc.ID);
                    ret = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return ret;
        }

        public int EliminarTipo(int id)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "DELETE FROM CDOCUMENTOTIPO WHERE ID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    ret = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return ret;
        }

        public int AgregarTipo(DocumentosTipo dc)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "INSERT INTO CDOCUMENTOTIPO (TIPO) VALUES (@tipo)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@tipo", dc.TIPO);
                   
                    ret = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return ret;
        }

        public int EliminarDocumento(int ID)
        {
            int ret = 0;
            using (SqlConnection conn = BD.obtenerConexion())
            {
                string query = "DELETE FROM CDOCUMENTOS WHERE ID = @ID";

                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@ID", ID); 

                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public int Agregar(DocumentosCarpeta C)
        {
            int ret = 0;
            string query = "INSERT INTO CCARPETAS (CARPETA, TIPO, IDCARPETA) VALUES (@CARPETA, @TIPO, @IDCARPETA)";
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@CARPETA", C.CARPETA);
                comando.Parameters.AddWithValue("@TIPO", C.TIPO);
                comando.Parameters.AddWithValue("@IDCARPETA", C.IDCARPETA);
                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public int Modificar(DocumentosCarpeta C)
        {
            int ret = 0;
            string query = "UPDATE CCARPETAS SET CARPETA = @CARPETA WHERE ID = @ID";
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@CARPETA", C.CARPETA);
                comando.Parameters.AddWithValue("@ID", C.ID);
                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public int Eliminar(int ID)
        {
            int ret = 0;
            string query = "DELETE FROM CCARPETAS WHERE ID = @ID";
            using (SqlConnection conn = BD.obtenerConexion())
            {
                SqlCommand comando = new SqlCommand(query, conn);
                comando.Parameters.AddWithValue("@ID", ID);
                ret = comando.ExecuteNonQuery();
                conn.Close();
            }
            return ret;
        }

        public DocumentosCarpeta TieneSubCarpetasActivas(int IDCARPETA, int TIPO)
        {
            DocumentosCarpeta u = new DocumentosCarpeta();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                string query = " select * from CCARPETAS WHERE IDCARPETA = @IDCARPETA and TIPO = @TIPO";
                SqlCommand comando = new SqlCommand(query, conexion);
                
                comando.Parameters.AddWithValue("@IDCARPETA", IDCARPETA);
                comando.Parameters.AddWithValue("@TIPO", TIPO);

                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    u.ID = reader.GetInt32(0);
                    u.CARPETA = reader.GetString(1);
                    u.TIPO = reader.GetInt32(2);
                    u.IDCARPETA = reader.GetInt32(3);
                }
                conexion.Close();
            }
            return u;
        }

        public List<Documentos> TraerDocumentosPorPuesto(string puesto, string tipoUsuario)
        {
            List<Documentos> lista = new List<Documentos>();
            using (SqlConnection conexion = BD.obtenerConexion())
            {
                // Si el usuario es ADMINISTRADOR, se muestran todos los documentos
                // de lo contrario se filtra por el puesto asignado en la tabla de visibilidad.
                string query = "";
                if (tipoUsuario.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                {
                    query = @"SELECT D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                     D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
              FROM CDOCUMENTOS AS D 
              WHERE D.ESTADO = 'ALTA'
              ORDER BY D.TITULO ASC";
                }
                else
                {
                    query = @"SELECT D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                     D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
              FROM CDOCUMENTOS AS D 
              INNER JOIN CDOCUMENTOVISIBILIDAD AS CD ON CD.IDDOCUMENTO = D.ID
              WHERE CD.PUESTO = @puesto AND D.ESTADO = 'ALTA'
              GROUP BY D.ID, D.TITULO, D.REVISION, D.IDCARPETA, D.CARPETA, 
                       D.IDTIPO, D.TIPO, D.IDESTADO, D.ESTADO, D.OBSERVACIONES, D.NOMBREARCHIVO
              ORDER BY D.TITULO ASC";
                }
                SqlCommand comando = new SqlCommand(query, conexion);
                if (!tipoUsuario.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
                {
                    comando.Parameters.AddWithValue("@puesto", puesto);
                }
                SqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Documentos doc = new Documentos();
                    doc.ID = reader.GetInt32(0);
                    doc.TITULO = reader.GetString(1);
                    doc.REVISION = reader.GetString(2);
                    doc.IDCARPETA = reader.GetInt32(3);
                    doc.CARPETA = reader.GetString(4);
                    doc.IDTIPO = reader.GetInt32(5);
                    doc.TIPO = reader.GetString(6);
                    doc.IDESTADO = reader.GetInt32(7);
                    doc.ESTADO = reader.GetString(8);
                    doc.OBSERVACIONES = reader.IsDBNull(9) ? null : reader.GetString(9);
                    doc.NOMBREARCHIVO = reader.GetString(10);
                    lista.Add(doc);
                }
                conexion.Close();
            }
            return lista;
        }
    }
}