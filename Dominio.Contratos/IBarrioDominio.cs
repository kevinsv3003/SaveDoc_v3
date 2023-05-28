using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IBarrioDominio
    {
        Task<List<BarrioDto>> ObtenerListaBarrio();
        Task<BarrioDto> ObtenerBarrioById(int BarrioId);
        Task<List<BarrioDto>> ListaBarriosByMunicipioId(int MunicipioId);
        Task<bool> AgregarBarrio(BarrioDto Barrio);
        Task<bool> ActualizarBarrio(BarrioDto Barrio);
    }
}
