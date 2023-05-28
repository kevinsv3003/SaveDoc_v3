using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Municipio", Schema = "JS")]

    public class Municipio
    {
        [Key]
        public int MunicipioId { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string DescripcionMunicipio { get; set; }
        [Required]
        public int? DepartamentoId { get; set; }
        public virtual Departamento Departamentos { get; set; }
    }
}
