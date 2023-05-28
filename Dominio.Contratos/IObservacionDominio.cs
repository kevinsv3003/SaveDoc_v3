using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IObservacionDominio
    {
        Task<List<string>> ObtenerListaObservacion();
        Task<List<ObservacionDto>> ObtenerListaObservacionByPersonal(int PersonalId);
        Task<ObservacionDto> ObtenerObservacionById(int ObservacionId);
        Task<bool> AgregarObservacion(ObservacionDto Observacion);
        Task<bool> ActualizarObservacion(ObservacionDto Observacion);
        Task<bool> EliminarObservacion(int ObservacionId);
    }
}
