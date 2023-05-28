using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace InfraestructuraRepositorios
{
    public class DocumentoRepositorio : BaseRepositorio<Documento>, IDocumento
    {
        public IConfiguration Configuration { get; }

        public DocumentoRepositorio(DbContext context, IConfiguration configuration) : base(context)
        {
            this.Configuration = configuration; 
        }

        public bool InsertarDocumento(Documento doc)
        {
            var connection = Configuration.GetConnectionString("SaveDocCore"); // @"Server=(localdb)\MSSQLLocalDB;Database=MiProyectoCoreDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            var retorno = false;
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Procedimiento para actualizar documento
                    const string query = @"[DOC].[InsertarDocumento]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;                    
                    cmd.CommandText = query;
                    cmd.Parameters.Add("@nombreDoc", SqlDbType.VarChar).Value = doc.Nombre;
                    cmd.Parameters.Add("@descripcionDoc", SqlDbType.VarChar).Value = doc.Descripcion != null ? doc.Descripcion : "";
                    cmd.Parameters.Add("@fechaRegistro", SqlDbType.DateTime).Value = doc.FechaRegistro;
                    cmd.Parameters.Add("@areaId", SqlDbType.Int).Value = doc.AreaId;
                    cmd.Parameters.Add("@docByte", SqlDbType.VarBinary).Value = doc.DocumentoByte;
                    cmd.Parameters.Add("@Extension", SqlDbType.VarChar).Value = (doc.Extension == "application/octet-stream") ? "text/plain" : doc.Extension;
                    
                    cmd.ExecuteNonQuery();
                    retorno = true;
                }

                return retorno;
            }
        }

        public bool ActualizarDocByte(int docId, string extension, byte[] documento)
        {
            var connection = Configuration.GetConnectionString("SaveDocCore"); // @"Server=(localdb)\MSSQLLocalDB;Database=MiProyectoCoreDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            var retorno = false;
            using (var conn = new SqlConnection(connection))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Procedimiento para actualizar documento
                    const string query = @"[DOC].[ActualizarDocumentoByte]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    cmd.Parameters.Add("@documentoId", SqlDbType.Int).Value = docId;
                    cmd.Parameters.Add("@docByte", SqlDbType.VarBinary).Value = documento;
                    cmd.Parameters.Add("@Extension", SqlDbType.VarChar).Value = (extension == "application/octet-stream") ? "text/plain" : extension;

                    cmd.ExecuteNonQuery();
                    retorno = true;
                }

                return retorno;
            }
        }

        public async Task<Documento> ObtenerDocumentoByte(int documentoId)
        {
            var doc = new Documento();
            byte[] documento = { };
            var connection = Configuration.GetConnectionString("SaveDocCore"); // @"Server=(localdb)\MSSQLLocalDB;Database=MiProyectoCoreDB;Trusted_Connection=True;MultipleActiveResultSets=true";


            using (var conn = new SqlConnection(connection))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    const string query = @"[DOC].[ObtenerDocumentoByte]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    cmd.Parameters.Add("@documentoId", SqlDbType.Int).Value = documentoId;

                    using (SqlDataReader d = await cmd.ExecuteReaderAsync())
                    {
                        if (d.Read())
                        {
                            doc.DocumentoByte = (!string.IsNullOrEmpty(d["DocumentoByte"].ToString())) ? (byte[])d["DocumentoByte"] : documento;
                            doc.Extension = (!string.IsNullOrEmpty(d["Extension"].ToString())) ? (string)d["Extension"] : string.Empty;
                        }

                    }
                }

            }

            return doc;
        }
    }
}
