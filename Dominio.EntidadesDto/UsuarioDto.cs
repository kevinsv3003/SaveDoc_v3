using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class UsuarioDto : IdentityUser
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string Sexo { get; set; }
        public string Direccion { get; set; }
        public int RentaId { get; set; }
        public List<string> ListaDireccion { get; set; }
        [Required]
        public string Rol { get; set; }
        public int OrigenForm { get; set; }
        public string  contra { get; set; }
        
        public string contraConfirm { get; set; }
    }
}
