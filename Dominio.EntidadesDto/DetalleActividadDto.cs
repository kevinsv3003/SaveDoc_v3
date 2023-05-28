using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class DetalleActividadDto
    {
        public int DetalleActividadId { get; set; }
        public int ActividadId { get; set; }
        public int PersonalId { get; set; }
        public bool Asistio { get; set; }
        public bool Transporte { get; set; }
        public bool Justificado { get; set; }
        public string Observacion { get; set; }
        public int cantidadActividad { get; set; }
        
        public PersonalDto Personal { get; set; }
        public  ActividadDto Actividad { get; set; }
    }
}
