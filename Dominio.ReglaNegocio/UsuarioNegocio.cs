using AutoMapper;
using ClosedXML.Excel;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class UsuarioNegocio : IUsuarioNegocio
    {
        private SignInManager<UsuarioApp> _signManager;
        private UserManager<UsuarioApp> _userManager;
        private RoleManager<RolApp> _roleManager;
        private IUtilitario _utilitario;

        public UsuarioNegocio(SignInManager<UsuarioApp> _signManager, UserManager<UsuarioApp> _userManager, RoleManager<RolApp> _roleManager, IUtilitario _utilitario)
        {
            this._signManager = _signManager;
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._utilitario = _utilitario;
        }

        public List<UsuarioDto> ObtenerUsuarios()
        {
            var resultado = new List<UsuarioDto>();
            try
            {
                var users = _userManager.Users.ToList();
                foreach (var item in users)
                {
                    var user = Mapper.Map<UsuarioApp, UsuarioDto>(item);
                    user.Rol = obtenerRolUsuario(item).Result;
                    resultado.Add(user);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return resultado;
        }

        public List<RolDto> ObtenerRoles()
        {
            var resultado = new List<RolDto>();
            try
            {
                var roles = _roleManager.Roles.ToList();
                foreach (var item in roles)
                {
                    var rol = new RolDto()
                    {
                        Nombre = item.Name,
                        Descripcion = item.Descripcion,
                        IdRol = item.Id.ToString()
                    };
                    resultado.Add(rol);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return resultado;
        }

        public async Task<RolDto> BuscarRolPorIdRol(string IdRol)
        {
            var resultado = new RolDto();
            try
            {
                var roles = await _roleManager.FindByIdAsync(IdRol);

                resultado = new RolDto()
                {
                    Nombre = roles.Name,
                    Descripcion = roles.Descripcion,
                    IdRol = roles.Id.ToString()
                };

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return resultado;
        }

        public async Task<bool> ActualizarUsuario(UsuarioDto usuariodto)
        {
            bool respuesta = false;
            try
            {
                var usuario = await _userManager.FindByIdAsync(usuariodto.Id);

                usuario.Nombres = usuariodto.Nombres;
                usuario.Apellidos = usuariodto.Apellidos;
                usuario.UserName = usuariodto.UserName;
                usuario.FechaNacimiento = usuariodto.FechaNacimiento;
                usuario.Sexo = usuariodto.Sexo;
                usuario.Direccion = usuariodto.Direccion;
                usuario.PhoneNumber = usuariodto.PhoneNumber;
                usuario.Email = usuariodto.Email;
                usuario.Edad = _utilitario.ObtenerEdadActual(usuariodto.FechaNacimiento);

                var rolUsuario = await obtenerRolUsuario(usuario);
                var borrarRol = (rolUsuario != string.Empty) ? await _userManager.RemoveFromRoleAsync(usuario, rolUsuario) : new IdentityResult();
                var AsignarRol = (borrarRol.Succeeded || rolUsuario == string.Empty) ? await _userManager.AddToRoleAsync(usuario, usuariodto.Rol) : new IdentityResult();
                var Actualizado = await _userManager.UpdateAsync(usuario);

                respuesta = Actualizado.Succeeded && AsignarRol.Succeeded;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<bool> ActualizarRol(RolDto rolDto)
        {
            bool respuesta = false;
            try
            {
                var Rol = await _roleManager.FindByIdAsync(rolDto.IdRol);
                Rol.Name = rolDto.Nombre;
                Rol.Descripcion = rolDto.Descripcion;
                var Actualizado = await _roleManager.UpdateAsync(Rol);
                respuesta = Actualizado.Succeeded;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<bool> GuardarRol(RolDto roldto)
        {
            bool respuesta = false;
            try
            {
                var rol = new RolApp() { Name = roldto.Nombre, Descripcion = roldto.Descripcion };
                var guardoRol = await _roleManager.CreateAsync(rol);
                respuesta = guardoRol.Succeeded;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<bool> GuardarUsuario(UsuarioDto usuarioDto)
        {
            bool respuesta = false;
            try
            {
                usuarioDto.Edad = _utilitario.ObtenerEdadActual(usuarioDto.FechaNacimiento);
                var usuario = Mapper.Map<UsuarioDto, UsuarioApp>(usuarioDto);
                var guardoUsuario = await _userManager.CreateAsync(usuario, usuarioDto.contra);
                if (guardoUsuario.Succeeded)
                {
                    var rolAsignado = await _userManager.AddToRoleAsync(usuario, usuarioDto.Rol);
                    respuesta = guardoUsuario.Succeeded && rolAsignado.Succeeded;
                }
                else if (guardoUsuario.Errors.FirstOrDefault().Code.Equals("PasswordTooShort"))
                {
                    var mensaje = ("Se produjo un error al restaurar su contraseña!! \nDebe de ser mayor de 5 caracteres.");
                    throw new ArgumentException(mensaje);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<bool> EliminarUsuario(string Id)
        {
            bool respuesta = false;
            try
            {
                var usuario = await _userManager.FindByIdAsync(Id);
                var EliminoUsuario = await _userManager.DeleteAsync(usuario);

                respuesta = EliminoUsuario.Succeeded;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<bool> EliminarRol(string Id)
        {
            bool respuesta = false;
            try
            {
                var rol = await _roleManager.FindByIdAsync(Id);
                var EliminoRol = await _roleManager.DeleteAsync(rol);

                respuesta = EliminoRol.Succeeded;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return respuesta;
        }

        public async Task<string> obtenerRolUsuario(UsuarioApp usuario)
        {
            var rolList = await _userManager.GetRolesAsync(usuario);
            var rol = rolList.Count > 0 ? new List<string>(rolList).ToArray()[0] : "";
            return rol;
        }

        public async Task<bool> ResetPass(UsuarioApp user, string pass)
        {
            //var removePass = await _userManager.RemovePasswordAsync(user);
            //var resetPass = await _userManager.AddPasswordAsync(user, contraseña);
            var tokenResetPass = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPass = await _userManager.ResetPasswordAsync(user, tokenResetPass, pass);

            return resetPass.Succeeded;
        }

        private DataTable ObtenerUsuariosTB()
        {
            DataTable tabla = new DataTable();
            DataRow row;
            tabla.Columns.Add("Usuario");
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Apellidos");
            tabla.Columns.Add("Correo");
            tabla.Columns.Add("Fecha Nacimiento");
            tabla.Columns.Add("Sexo");
            tabla.Columns.Add("Rol");
            tabla.Columns.Add("Direccion");

            try
            {
                var detalle = ObtenerUsuarios();
                int contador = 1;
                foreach (var item in detalle)
                {
                    row = tabla.NewRow();
                    row["Usuario"] = item.UserName;
                    row["Nombre"] = item.Nombres;
                    row["Apellidos"] = item.Apellidos;
                    row["Correo"] = item.Email;
                    row["Fecha Nacimiento"] = item.FechaNacimiento.ToShortDateString().ToString();
                    row["Sexo"] = item.Sexo;
                    row["Rol"] = item.Rol;
                    row["Direccion"] = item.Direccion;

                    tabla.Rows.Add(row);
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return tabla;
        }

        public async Task<byte[]> ObtenerUsuariosExcel()
        {
            var usuarios = ObtenerUsuarios().GroupBy(x => new { x.Nombres, x.Apellidos })
                    .Select(y => new UsuarioDto()
                    {
                        UserName = y.Select(d => d.UserName).FirstOrDefault(),
                        Nombres = y.Key.Nombres,
                        Apellidos = y.Key.Apellidos,
                        Email = y.Select(d => d.Email).FirstOrDefault(),
                        FechaNacimiento = y.Select(d => d.FechaNacimiento).FirstOrDefault(),
                        Sexo = y.Select(d => d.Sexo).FirstOrDefault(),
                        Rol = y.Select(d => d.Rol).FirstOrDefault(),
                        ListaDireccion = y.Select(d => d.Direccion).ToList(),
                    }); 
            //var usuariosDT = ObtenerUsuariosTB();

            var EncabezadoTabla = "B2:I2";
            var registrosTotales = usuarios.Sum(x => x.ListaDireccion.Count());
            var CabeceraTabla = "B3:I3";
            var ContenidoTabla = "B4:I" + (3 + registrosTotales).ToString();
            var TablaCompleta = "B2:I" + (3 + registrosTotales).ToString();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Usuarios");

                worksheet.Range(EncabezadoTabla).Merge().SetValue("LISTADO DE USUARIOS ACTIVOS").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48,84,150))
                    .Fill.SetBackgroundColor(XLColor.White);


                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompleta).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTabla).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68,114,196));

                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTabla).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);

                //worksheet.Cell(CeldaInicialCabecera).InsertTable(usuariosDT.AsEnumerable());

                worksheet.Cell("B3").Value = "Usuario";
                worksheet.Cell("C3").Value = "Nombre";
                worksheet.Cell("D3").Value = "Apellidos";
                worksheet.Cell("E3").Value = "Correo";
                worksheet.Cell("F3").Value = "Fecha Nacimiento";
                worksheet.Cell("G3").Value = "Sexo";
                worksheet.Cell("H3").Value = "Rol";
                worksheet.Cell("I3").Value = "Direccion";

                var currentRow = 4;
                foreach (var user in usuarios)
                {
                    
                    var direcciones = user.ListaDireccion.Count();
                    if (direcciones > 1)
                    {
                        worksheet.Range("B" + currentRow + ":B" + (currentRow + direcciones-1)).Merge().SetValue(user.UserName);
                        worksheet.Range("C" + currentRow + ":C" + (currentRow + direcciones-1)).Merge().SetValue(user.Nombres);
                        worksheet.Range("D" + currentRow + ":D" + (currentRow + direcciones-1)).Merge().SetValue(user.Apellidos);
                        worksheet.Range("E" + currentRow + ":E" + (currentRow + direcciones-1)).Merge().SetValue(user.Email);
                        worksheet.Range("F" + currentRow + ":F" + (currentRow + direcciones-1)).Merge().SetValue(user.FechaNacimiento);
                        worksheet.Range("G" + currentRow + ":G" + (currentRow + direcciones-1)).Merge().SetValue(user.Sexo);
                        worksheet.Range("H" + currentRow + ":H" + (currentRow + direcciones-1)).Merge().SetValue(user.Rol);
                        var celdaDir = currentRow;
                        foreach (var d in user.ListaDireccion)
                        {
                            worksheet.Cell("I" + celdaDir).Value = d.ToString();
                            celdaDir ++;
                        }
                        currentRow= currentRow + direcciones;
                    }
                    else
                    {
                        worksheet.Cell("B" + currentRow).Value = user.UserName;
                        worksheet.Cell("C" + currentRow).Value = user.Nombres;
                        worksheet.Cell("D" + currentRow).Value = user.Apellidos;
                        worksheet.Cell("E" + currentRow).Value = user.Email;
                        worksheet.Cell("F" + currentRow).Value = user.FechaNacimiento;
                        worksheet.Cell("G" + currentRow).Value = user.Sexo;
                        worksheet.Cell("H" + currentRow).Value = user.Rol;
                        worksheet.Cell("I" + currentRow).Value = user.ListaDireccion.FirstOrDefault().ToString();
                    }
                }



                worksheet.Columns().AdjustToContents();
                worksheet.Rows().AdjustToContents();
                worksheet.Columns("B").Width = 14;
                worksheet.Columns("C").Width = 14;
                worksheet.Columns("D").Width = 14;
                worksheet.Columns("E").Width = 24;
                worksheet.Columns("F").Width = 23;
                worksheet.Columns("G").Width = 11;
                worksheet.Columns("H").Width = 14;
                worksheet.Columns("I").Width = 60;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
    }
}
