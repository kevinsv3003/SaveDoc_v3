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
    public class PersonalRepositorio : BaseRepositorio<Personal>, IPersonal
    {
        public IConfiguration Configuration { get; }

        public PersonalRepositorio(DbContext context, IConfiguration configuration) : base(context)
        {
            this.Configuration = configuration;
        }

        public async Task<List<int>> ObtenerReportePersonal(int tipoReporte)
        {
            List<int> reportePersona = new List<int>();
            var connection = Configuration.GetConnectionString("SaveDocCore");

            using (var conn = new SqlConnection(connection))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    const string query = @"[PER].[ObtenerReportePersonal]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    cmd.Parameters.Add("@tipoReporte", SqlDbType.Int).Value = tipoReporte;

                    using (SqlDataReader d = await cmd.ExecuteReaderAsync())
                    {
                        while (d.Read())
                        {
                            var persona = (!string.IsNullOrEmpty(d["PersonalId"].ToString())) ? (int)d["PersonalId"] : 0;
                            reportePersona.Add(persona);
                        }

                    }
                }

            }

            return reportePersona;
        }
    }
}
