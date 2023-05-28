using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class DocumentoDto
    {
        public int DocumentoId { get; set; }
        public int AreaId { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Extension { get; set; }
        public string NombreArea { get; set; }
        public IFormFile FileDoc { get; set; }
        public byte[] Doc { get; set; }
        public byte[] Fondo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
