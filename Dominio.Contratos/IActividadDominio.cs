using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IActividadDominio
    {
        Task<List<ActividadDto>> ObtenerListaActividad(int rentaUsuario);
        Task<ActividadDto> ObtenerActividadById(int ActividadId);
        Task<bool> AgregarActividad(ActividadDto Actividad);
        Task<bool> ActualizarActividad(ActividadDto Actividad);
        Task<List<int>> ObtenerPropuestaPersonal(int cantidad, int renta, string genero, bool EsManagua, bool EsParticipativo);
        List<int> ObtenerPropuestaPersonalActividad(int cantidad, int renta, string genero, bool EsManagua, bool EsParticipativo, List<string> observaciones);
    }
}
