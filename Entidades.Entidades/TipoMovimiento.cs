using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("TipoMovimiento", Schema = "PER")]

    public class TipoMovimiento
    {
        [Key]
        public int TipoMovimientoId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string TipoMov { get; set; }
    }
}
