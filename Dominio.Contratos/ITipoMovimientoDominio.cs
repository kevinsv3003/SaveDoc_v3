using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface ITipoMovimientoDominio
    {
        Task<List<TipoMovimientoDto>> ObtenerListaTipoMovimiento();
        Task<TipoMovimientoDto> ObtenerTipoMovimientoById(int TipoMovimientoId);
        Task<bool> AgregarTipoMovimiento(TipoMovimientoDto TipoMovimiento);
        Task<bool> ActualizarTipoMovimiento(TipoMovimientoDto TipoMovimiento);
        Task<bool> EliminarTipoMovimiento(int TipoMovimientoId);
    }
}
