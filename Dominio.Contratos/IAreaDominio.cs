using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IAreaDominio
    {
        Task<List<AreaDto>> ObtenerListaArea();
        Task<AreaDto> ObtenerAreaById(int areaId);
        Task<bool> AgregarArea(AreaDto area);
        Task<bool> ActualizarArea(AreaDto area);
        Task<bool> EliminarArea(int areaId);
    }
}
