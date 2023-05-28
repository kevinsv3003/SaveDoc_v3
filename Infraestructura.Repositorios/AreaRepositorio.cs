using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class AreaRepositorio : BaseRepositorio<Area>, IArea
    {
        public AreaRepositorio(DbContext context) : base(context)
        {

        }
    }
}
