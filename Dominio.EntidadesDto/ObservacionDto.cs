using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class ObservacionDto
    {
        public int ObservacionId { get; set; }
        public string ObservacionDet { get; set; }
        public int? PersonalId { get; set; }
    }
}
