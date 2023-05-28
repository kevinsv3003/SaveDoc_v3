using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IMovimientoDominio
    {
        Task<List<MovimientoDto>> ObtenerListaMovimiento(int cuentaId);
        Task<MovimientoDto> ObtenerMovimientoById(int movimientoId);
        Task<bool> AgregarMovimiento(MovimientoDto movimiento);
        Task<bool> ActualizarMovimiento(MovimientoDto movimiento);
        Task<bool> EliminarMovimiento(MovimientoDto movimiento);
    }
}
