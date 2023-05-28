using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class MovimientoRepositorio : BaseRepositorio<Movimiento>, IMovimiento
    {
        public MovimientoRepositorio(DbContext context) : base(context)
        {

        }
    }
}
