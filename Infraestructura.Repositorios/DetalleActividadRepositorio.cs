using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class DetalleActividadRepositorio : BaseRepositorio<DetalleActividad>, IDetalleActividad
    {
        public DetalleActividadRepositorio(DbContext context) : base(context)
        {

        }
    }
}
