using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class CuentaRepositorio : BaseRepositorio<Cuenta>, ICuenta
    {
        public CuentaRepositorio(DbContext context) : base(context)
        {

        }
    }
}
