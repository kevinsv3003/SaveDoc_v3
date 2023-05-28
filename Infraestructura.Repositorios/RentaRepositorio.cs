using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class RentaRepositorio : BaseRepositorio<Renta>, IRenta
    {
        public RentaRepositorio(DbContext context) : base(context)
        {

        }
    }
}
