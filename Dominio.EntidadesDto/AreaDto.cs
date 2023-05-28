using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class AreaDto
    {
        public int AreaId { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public byte[] Fondo { get; set; }
        public IFormFile FileImage { get; set; }
        public bool Estado { get; set; }
    }
}
