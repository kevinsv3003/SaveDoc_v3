using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("DetalleActividad", Schema = "JS")]
    public class DetalleActividad
    {
        [Key]
        public int DetalleActividadId { get; set; }
        [Required]
        public int ActividadId { get; set; }
        [Required]
        public int PersonalId { get; set; }
        [Required]
        public bool Asistio { get; set; }
        public bool Justificado { get; set; }
        public bool Transporte { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Observacion { get; set; }

        public virtual Personal Personal { get; set; }
        public virtual Actividad Actividad { get; set; }

    }
}
