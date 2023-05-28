using Entidades.Entidades;
using Infraestructura.Contratos;
using InfraestructuraRepositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorios
{
    public class DepartamentoRepositorio : BaseRepositorio<Departamento>, IDepartamento
    {
        public DepartamentoRepositorio(DbContext context) : base(context)
        {

        }
    }
}
