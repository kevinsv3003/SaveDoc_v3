using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("Roles", Schema = "SEG")]

    public class RolApp: IdentityRole<Guid>
    {
        [Column("Descripcion")]
        [Required]

        public string Descripcion
        {
            get;
            set;
        }

        [Column("LogId")]
        public int LogId
        {
            get;
            set;
        }

        [Column("UsuarioLogId")]
        public int UsuarioLogId
        {
            get;
            set;
        }

        [Column("EsBorrado")]
        public bool EsBorrado
        {
            get;
            set;
        }
    }
}
