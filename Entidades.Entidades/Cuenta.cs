using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Cuenta", Schema = "PER")]
    public class Cuenta
    {
        [Key]
        public int CuentaId { get; set; }
        [Column(TypeName = "varchar(200)")]

        public string NombreCuenta { get; set; }
        [Column(TypeName = "varchar(200)")]

        public string NumCuenta { get; set; }
        [Column(TypeName = "varchar(5)")]
        public string simboloMoneda { get; set; }
        public decimal SaldoActual { get; set; }
    }
}
