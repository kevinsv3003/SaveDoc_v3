using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class MunicipioDto
    {
        public int MunicipioId { get; set; }
        public string DescripcionMunicipio { get; set; }
        public int? DepartamentoId { get; set; }
    }
}
