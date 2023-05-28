using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Barrio", Schema = "JS")]
    public class Barrio
    {
        [Key]
        public int BarrioId { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string NombreBarrio { get; set; }
        [Required]
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipios { get; set; }
    }
}
