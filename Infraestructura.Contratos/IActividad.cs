using Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Contratos
{
    public interface IActividad : IBaseRepositorio<Actividad>
    {
        Task<List<int>> ObtenerPropuestaPersonal(int cantidad, int renta, string genero, int EsManagua, int EsParticipativo);
    }
}
