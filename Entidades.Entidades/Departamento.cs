using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Departamento", Schema = "JS")]

    public class Departamento
    {
        [Key]
        public int DepartamentoId { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string DescripcionDepartamento { get; set; }
    }
}
