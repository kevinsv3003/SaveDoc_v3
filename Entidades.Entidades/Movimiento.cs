using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Movimiento", Schema = "PER")]

    public class Movimiento
    {
        [Key]
        public int MovimientoId { get; set; }
        public int CuentaId { get; set; }
        public int TipoMovimientoId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaMovimiento { get; set; }

        public virtual Cuenta Cuenta { get; set; }
        public virtual TipoMovimiento TipoMovimiento { get; set; }
    }
}
