using Dominio.EntidadesDto;
using Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IUsuarioNegocio
    {
        Task<bool> ActualizarUsuario(UsuarioDto usuariodto);
        Task<string> obtenerRolUsuario(UsuarioApp usuario);
        Task<bool> ResetPass(UsuarioApp user, string pass);
        Task<bool> GuardarUsuario(UsuarioDto usuarioDto);
        Task<bool> EliminarUsuario(string Id);
        List<UsuarioDto> ObtenerUsuarios();
        List<RolDto> ObtenerRoles();
        Task<RolDto> BuscarRolPorIdRol(string IdRol);
        Task<bool> EliminarRol(string Id);
        Task<bool> ActualizarRol(RolDto rolDto);
        Task<bool> GuardarRol(RolDto roldto);
        Task<byte[]> ObtenerUsuariosExcel();
    }
}
