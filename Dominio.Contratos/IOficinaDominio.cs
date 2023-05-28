using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IOficinaDominio
    {
        Task<List<OficinaDto>> ObtenerListaOficina();
        Task<OficinaDto> ObtenerOficinaById(int OficinaId);
        Task<bool> AgregarOficina(OficinaDto Oficina);
        Task<bool> ActualizarOficina(OficinaDto Oficina);
    }
}
