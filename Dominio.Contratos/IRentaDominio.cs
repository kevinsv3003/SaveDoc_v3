using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IRentaDominio
    {
        Task<List<RentaDto>> ObtenerListaRenta();
        Task<RentaDto> ObtenerRentaById(int RentaId);
        Task<bool> AgregarRenta(RentaDto Renta);
        Task<bool> ActualizarRenta(RentaDto Renta);
    }
}
