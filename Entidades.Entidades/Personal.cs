using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Personal", Schema = "JS")]
    public class Personal
    {
        [Key]
        public int PersonalId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PrimerNombre { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string SegundoNombre { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PrimerApellido { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string SegundoApellido { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Cedula { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        public string Telefono { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string Genero { get; set; }

        [Required]
        public int CodDepartamento { get; set; }

        [Required]
        public int CodMunicipio { get; set; }
        public int? CodBarrio { get; set; }
       
        public bool Disponible { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Direccion { get; set; }
        

        [Required]
        public int? RentaId { get; set; }
        [Required]
        public int? OficinaId { get; set; }

        public virtual Renta Rentas { get; set; }
        public virtual Oficina Oficinas { get; set; }
    }
}
