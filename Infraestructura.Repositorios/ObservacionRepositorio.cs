using Entidades.Entidades;
using Infraestructura.Contratos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraestructuraRepositorios
{
    public class ObservacionRepositorio : BaseRepositorio<Observacion>, IObservacion
    {
        public ObservacionRepositorio(DbContext context) : base(context)
        {

        }
    }
}
