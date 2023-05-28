using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Oficina", Schema = "JS")]
    public class Oficina
    {
        [Key]
        public int OficinaId { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Nombre { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
