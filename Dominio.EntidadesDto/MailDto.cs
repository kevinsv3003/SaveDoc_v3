using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.EntidadesDto
{
    public class MailDto
    {
        public string Correo { get; set; }
        public string NombreCompleto { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public byte[] Archivo { get; set; }
        public List<DestinatarioDto> ListaCopias { get; set; }
    }
}
