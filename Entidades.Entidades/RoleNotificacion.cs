using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entidades.Entidades
{
    [Table("RolesNotificaciones", Schema = "SEG")]
    public class RoleNotificacion : IdentityRoleClaim<Guid>
    {
        /// <summary>
        /// Gets or sets
        /// Identificador del log para busqueda en la tabla historica de transacciones.
        /// </summary>
        [Column("LogId")]
        public int LogId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets
        /// Usuario que ha realizado la ultima transaccion en la entidad.
        /// </summary>
        [Column("UsuarioLogId")]
        public int UsuarioLogId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets
        /// Borrado logico del registro.
        /// </summary>
        [Column("EsBorrado")]
        public bool EsBorrado
        {
            get;
            set;
        }
    }
}
