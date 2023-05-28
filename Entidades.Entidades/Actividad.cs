using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Actividad", Schema = "JS")]
    public class Actividad
    {
        [Key]
        public int ActividadId { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string NombreActividad { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Lugar { get; set; }
        [Required]
        public DateTime? Fecha { get; set; }
        [Required]
        public bool EsEspecial { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Descripcion { get; set; }
        public int RentaId { get; set; }
    }
}
