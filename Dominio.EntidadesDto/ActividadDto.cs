using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
   public class ActividadDto
    {
        public int ActividadId { get; set; }

        public string NombreActividad { get; set; }
        public string Lugar { get; set; }
        public DateTime? Fecha { get; set; }
        public bool EsEspecial { get; set; }
        public int Participantes { get; set; }
        public string Descripcion { get; set; }
        public List<PersonalDto> ListaParticipantes { get; set; }
        public int RentaId { get; set; }
    }
}
