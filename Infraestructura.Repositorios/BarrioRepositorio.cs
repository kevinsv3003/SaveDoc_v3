using Entidades.Entidades;
using Infraestructura.Contratos;
using InfraestructuraRepositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorios
{
  public  class BarrioRepositorio : BaseRepositorio<Barrio>, IBarrio
    {
        public BarrioRepositorio(DbContext context) : base(context)
        {

        }
    }
}
