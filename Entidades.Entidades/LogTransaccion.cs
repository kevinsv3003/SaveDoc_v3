using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("LogTransaccion",Schema ="AUDI")]
    public class LogTransaccion
    {
        [Key]
        public int LogTransaccionID { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Usuario { get; set; }

        [Required]
        public DateTime? FechaProceso { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Accion { get; set; }

        [Required]
        [Column(TypeName = "varchar(800)")]
        public string Variables { get; set; }
    }
}
