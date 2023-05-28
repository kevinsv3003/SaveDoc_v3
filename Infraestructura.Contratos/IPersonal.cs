using Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Contratos
{
  public  interface IPersonal : IBaseRepositorio<Personal>
    {
        Task<List<int>> ObtenerReportePersonal(int tipoReporte);
    }
}
