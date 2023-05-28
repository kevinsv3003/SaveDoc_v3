using Entidades.Entidades;
using Infraestructura.Contratos;
using InfraestructuraRepositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorios
{
  public  class MunicipioRepositorio : BaseRepositorio<Municipio>, IMunicipio
    {
        public MunicipioRepositorio(DbContext context) : base(context)
        {

        }
    }
}
