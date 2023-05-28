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
    public class ActividadRepositorio : BaseRepositorio<Actividad>, IActividad
    {
        public IConfiguration Configuration { get; }
        public ActividadRepositorio(DbContext context, IConfiguration configuration) : base(context)
        {
            this.Configuration = configuration;

        }

        public async Task<List<int>> ObtenerPropuestaPersonal(int cantidad, int renta, string genero, int EsManagua, int EsParticipativo)
        {
            List<int> propuestas = new List<int>();         
            var connection = Configuration.GetConnectionString("SaveDocCore");

            using (var conn = new SqlConnection(connection))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    const string query = @"[JS].[ProponerPersonal]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    cmd.Parameters.Add("@cantidad", SqlDbType.Int).Value = cantidad;
                    cmd.Parameters.Add("@renta", SqlDbType.Int).Value = renta;
                    cmd.Parameters.Add("@municipio", SqlDbType.Int).Value = EsManagua;
                    cmd.Parameters.Add("@participativo", SqlDbType.Int).Value = EsParticipativo;
                    cmd.Parameters.Add("@genero", SqlDbType.VarChar).Value = genero;

                    using (SqlDataReader d = await cmd.ExecuteReaderAsync())
                    {
                        while (d.Read())
                        {

                            var persona = (!string.IsNullOrEmpty(d["PersonalId"].ToString())) ? (int)d["PersonalId"] : 0;
                            propuestas.Add(persona);
                        }

                    }
                }

            }

            return propuestas;
        }

    }
}
