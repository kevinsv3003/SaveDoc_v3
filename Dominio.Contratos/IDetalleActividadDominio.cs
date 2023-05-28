using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IDetalleActividadDominio
    {
        Task<List<DetalleActividadDto>> ObtenerListaDetalleActividad();
        Task<List<DetalleActividadDto>> ObtenerListaDetalleActividadByPersonalId(int PersonalId);
        Task<List<DetalleActividadDto>> ObtenerListaDetalleActividadByActividadlId(int ActividadId);
        Task<bool> BorrarDetalleByActividadId(int ActividadId);
        Task<bool> BorrarDetalleByActividadIdYPersonalId(int ActividadId, int PersonalId);
        Task<DetalleActividadDto> ObtenerDetalleActividadById(int DetalleActividadId);
        Task<bool> AgregarDetalleActividad(DetalleActividadDto DetalleActividad);
        Task<bool> ActualizarDetalleActividad(DetalleActividadDto DetalleActividad);
        Task<byte[]> GenerarReporteActividad(int ActividadId);
        Task<byte[]> GenerarReporteParticipante(int ActividadId);
        Task<byte[]> GenerarReporteDetallePersonal(int personalId);


    }
}
