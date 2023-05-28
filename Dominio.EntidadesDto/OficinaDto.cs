using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class OficinaDto
    {
        public int OficinaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
