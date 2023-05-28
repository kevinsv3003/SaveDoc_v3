using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infraestructura.Contratos
{
    public interface IBaseRepositorio<T> where T : class
    {
        IList<T> ListarTodos();
        void Insertar(T entidad);
        T BuscarPorId(object identificador);
        IEnumerable<T> Buscar(System.Linq.Expressions.Expression<Func<T, bool>> predicado);
        void Actualizar(T entidad);
        void Eliminar(object identificador);
        IQueryable<T> Consulta();
    }
}
