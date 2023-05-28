using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class OficinaRepositorio : BaseRepositorio<Oficina>, IOficina
    {
        public OficinaRepositorio(DbContext context) : base(context)
        {

        }
    }
}
