using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        public string NombreCuenta { get; set; }
        public string NumCuenta { get; set; }
        public string simboloMoneda { get; set; }

        public decimal SaldoActual { get; set; }
        public string totalSaldoActual { get; set; }
        public string totalGastos { get; set; }
        public string totalIngresos { get; set; }
    }
}
