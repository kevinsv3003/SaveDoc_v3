using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IPersonalDominio
    {
        Task<List<PersonalDto>> ObtenerListaPersonal();
        Task<PersonalDto> ObtenerPersonalById(int PersonalId);
        Task<bool> AgregarPersonal(PersonalDto Personal);
        Task<bool> ActualizarPersonal(PersonalDto Personal);
        Task<byte[]> GenerarReporteDelPersonal(int tipoReporte, string genero, bool EsManagua, List<string> LstObservaciones, List<string> LstRestricciones);
    }
}
