using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface ICuentaDominio
    {
        Task<List<CuentaDto>> ObtenerListaCuenta();
        Task<CuentaDto> ObtenerCuentaById(int cuentaId);
        Task<bool> AgregarCuenta(CuentaDto cuenta);
        Task<bool> ActualizarMontoCuenta(MovimientoDto movimientoDto);
        Task<bool> EliminarCuenta(int cuentaId);
    }
}
