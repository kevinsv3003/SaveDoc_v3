using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class BarrioDto
    {
  
        public int BarrioId { get; set; }
        public string NombreBarrio { get; set; }
        public int? MunicipioId { get; set; }
    }
}
