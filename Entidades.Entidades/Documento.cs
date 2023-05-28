using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Documento", Schema = "DOC")]

    public class Documento
    {
        [Key]
        public int DocumentoId { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string Nombre { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar(500)")]
        public string Descripcion { get; set; }

        [NotMapped]
        public byte[] DocumentoByte { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Extension { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FechaRegistro { get; set; }

        [Required]
        public int? AreaId { get; set; }

        public virtual Area Areas { get; set; }
    }
}
