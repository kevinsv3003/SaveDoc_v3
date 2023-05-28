using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class MovimientoDto
    {
        public int MovimientoId { get; set; }
        public int CuentaId { get; set; }
        public int TipoMovimientoId { get; set; }
        public string Concepto { get; set; }
        public string TipoMovimiento { get; set; }
        public string MontoFormato { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaMovimiento { get; set; }
    }
}
