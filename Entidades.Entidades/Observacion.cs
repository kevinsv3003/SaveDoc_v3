using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Observacion", Schema = "JS")]
    public class Observacion
    {
        [Key]
        public int ObservacionId { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string ObservacionDet { get; set; }
        

        [Required]
        public int? PersonalId { get; set; }
        public virtual Personal Personals { get; set; }


    }
}
