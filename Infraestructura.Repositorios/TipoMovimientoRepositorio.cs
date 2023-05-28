using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class TipoMovimientoRepositorio : BaseRepositorio<TipoMovimiento>, ITipoMovimiento
    {
        public TipoMovimientoRepositorio(DbContext context) : base(context)
        {

        }
    }
}
