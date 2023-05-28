using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SautinSoft;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SaveDoc.Controllers
{

    public class AccountController : Controller
    {
        private SignInManager<UsuarioApp> _signManager;
        private UserManager<UsuarioApp> _userManager;
        private RoleManager<RolApp> _roleManager;
        private IUsuarioNegocio usuarioNegocio;
        private IUtilitario _utilitario;

        public AccountController(SignInManager<UsuarioApp> _signManager, UserManager<UsuarioApp> _userManager,
            RoleManager<RolApp> _roleManager, IUsuarioNegocio usuarioNegocio, IUtilitario _utilitario)
        {
            this._signManager = _signManager;
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._utilitario = _utilitario;
            this.usuarioNegocio = usuarioNegocio;
        }


        [AllowAnonymous]
        // GET: /<controller>/
        public IActionResult Index(string returnUrl = "")
        {
            var model = new LoginViewDto { ReturnUrl = returnUrl };
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
        public IActionResult UsuarioBloqueado()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewDto model)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            if (ModelState.IsValid)
            {
                await _signManager.SignOutAsync();
                var result = await _signManager.PasswordSignInAsync(model.userName,
                   model.Password, model.RememberMe, lockoutOnFailure: true);

                Response.StatusCode = (result.Succeeded) ? (int)HttpStatusCode.OK : (result.IsLockedOut) ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.BadRequest;
                mensaje = (result.Succeeded) ? "Listo" : (result.IsLockedOut) ? "UBT" : "Usuario o Contraseña no validos";

                /*if (result.Succeeded)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Listo!";
                }
                else if (result.IsLockedOut)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "UBT";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Usuario o Contraseña no válidos!!";
                }*/
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = "Favor introducir correctamente los datos!";
            }
            return Json(mensaje);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RestaurarContra([FromBody] LoginViewDto model)
        {
            return await RecordarContrasena(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RecordarContrasena(LoginViewDto model)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var esCorreoUsuarioValido = user != null && (user.UserName == model.userName);

                if (esCorreoUsuarioValido)
                {
                    var contraseña = _utilitario.GenerarCodigo();// General.GenerarCodigo();

                    var ResetContraseña = await usuarioNegocio.ResetPass(user, contraseña);
                    if (ResetContraseña)
                    {
                        var valoresCuerpo = new ValoresCorreoContra()
                        {
                            NombreCompleto = user.Nombres + " " + user.Apellidos,
                            NombreUsuario = user.UserName,
                            Contraseña = contraseña
                        };
                        string cuerpo = _utilitario.ObtenerHtml("CorreoContra.cshtml", valoresCuerpo);// General.ObtenerHtml("CorreoContra.cshtml", valoresCuerpo);
                        var valoresCorreo = new MailDto()
                        {
                            NombreCompleto = valoresCuerpo.NombreCompleto,
                            Correo = model.Email,
                            Asunto = "Restauración Contraseña”",
                            Mensaje = cuerpo
                        };
                        await _utilitario.EnviarEmailAsync(valoresCorreo);//General.EnviarMensaje(model.Email, "Restauración de Contraseña", cuerpo);
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        mensaje = "Listo!";
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        mensaje = "Se produjo un error al restaurar su contraseña!!";
                    }

                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Usuario o Correo no válidos!!";
                }

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = "Se produjo un error al restaurar su contraseña!!";
            }
            return Json(mensaje);
        }

        [Authorize]
        public IActionResult PerfilUsuario()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> _FormEditUsuario([FromBody] UsusarioParametroDto parametros)
        {
            try
            {
                ViewBag.Roles = await obtenerRoles(parametros);
                ViewBag.Origen = parametros.Origen;
                var usuario = (parametros.Origen == 1) ? await _userManager.GetUserAsync(User) :
                              (parametros.Origen == 2) ? null : await _userManager.FindByIdAsync(parametros.IdUser);
                var UsuarioDto = (usuario != null) ? Mapper.Map<UsuarioApp, UsuarioDto>(usuario) : new UsuarioDto();
                ViewBag.Rol = (usuario != null) ? await usuarioNegocio.obtenerRolUsuario(usuario) : "";
                if (parametros.Origen != 2)
                {
                    ViewBag.FechaNacimiento = _utilitario.ObtenerFechaLetra(usuario.FechaNacimiento);// General.ObtenerFechaLetra(usuario.FechaNacimiento);
                    ViewBag.Edad = _utilitario.ObtenerEdadActual(usuario.FechaNacimiento) + " años";

                }
                return PartialView(UsuarioDto);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> _FormEditRol([FromBody] UsusarioParametroDto parametro)
        {
            try
            {
                ViewBag.Origen = parametro.Origen;
                var rolDto = (parametro.Origen != 2) ? await usuarioNegocio.BuscarRolPorIdRol(parametro.IdRol) : new RolDto();

                return PartialView(rolDto);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> EliminarUsuario([FromBody] string Id)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var retorno = await usuarioNegocio.EliminarUsuario(Id);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se eliminó el usuario correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se generó un error al eliminar el usuario!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> EliminarRol([FromBody] string Id)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var retorno = await usuarioNegocio.EliminarRol(Id);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se eliminó el rol correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se generó un error al eliminar el rol!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        public IActionResult _TablaUsuarios()
        {
            try
            {
                var usuarios = usuarioNegocio.ObtenerUsuarios();
                return PartialView(usuarios);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        public IActionResult _TablaRoles()
        {
            try
            {
                var roles = usuarioNegocio.ObtenerRoles();
                return PartialView(roles);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> GuardarUser(UsuarioDto usuario)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (usuario.OrigenForm != 2)
                    {
                        var actualizado = await usuarioNegocio.ActualizarUsuario(usuario);
                        if (actualizado)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            mensaje = "Se ha actualizado su información correctamente!!";
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            mensaje = "Se produjo un error al actualizar su información!!";
                        }
                    }
                    else
                    {
                        if (usuario.contra != usuario.contraConfirm)
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            mensaje = "Las contraseñas no coinciden, intente nuevamente!!";
                        }
                        else
                        {
                            var guardado = await usuarioNegocio.GuardarUsuario(usuario);
                            if (guardado)
                            {
                                Response.StatusCode = (int)HttpStatusCode.OK;
                                mensaje = "Se ha guardado el usuario correctamente!!";
                            }
                            else
                            {
                                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                mensaje = "Se produjo un error al guardar el usuario!!";
                            }
                        }
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Favor verifique todos los campos!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        [Authorize (Roles ="Administrador")]
        public async Task<IActionResult> GuardarRol(RolDto rol)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (rol.IdRol != null)
                    {
                        var actualizado = await usuarioNegocio.ActualizarRol(rol);
                        if (actualizado)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            mensaje = "Se ha actualizado su información correctamente!!";
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            mensaje = "Se produjo un error al actualizar su información!!";
                        }
                    }
                    else
                    {

                        var guardado = await usuarioNegocio.GuardarRol(rol);
                        if (guardado)
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            mensaje = "Se ha guardado el rol correctamente!!";
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            mensaje = "Se produjo un error al guardar el rol!!";
                        }

                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Favor verifique todos los campos!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        [Authorize]
        public async Task<IActionResult> _DatosPersonales()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);
                ViewBag.Rol = await usuarioNegocio.obtenerRolUsuario(usuario);
                ViewBag.FechaNacimiento = _utilitario.ObtenerFechaLetra(usuario.FechaNacimiento);
                ViewBag.Edad = _utilitario.ObtenerEdadActual(usuario.FechaNacimiento) + " años";

                return PartialView(usuario);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult AdministrarCuentas()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult AdministrarRoles()
        {
            return View();
        }

        [Authorize]
        public IActionResult _CambiarContra()
        {
            return PartialView();
        }

        [Authorize]
        public async Task<IActionResult> CambiarContra(LoginViewDto login)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var usuario = await _userManager.GetUserAsync(User);
                var resetPass = await _userManager.ChangePasswordAsync(usuario, login.Password, login.NewPassword);
                if (resetPass.Succeeded)
                {
                    var valoresCuerpo = new ValoresCorreoContra()
                    {
                        NombreCompleto = usuario.Nombres + " " + usuario.Apellidos,
                        NombreUsuario = usuario.UserName,
                        Contraseña = login.NewPassword
                    };
                    string cuerpo = _utilitario.ObtenerHtml("CorreoContra.cshtml", valoresCuerpo);// General.ObtenerHtml("CorreoContra.cshtml", valoresCuerpo);
                    var valoresCorreo = new MailDto()
                    {
                        NombreCompleto = valoresCuerpo.NombreCompleto,
                        Correo = usuario.Email,
                        Asunto = "Restauración Contraseña”",
                        Mensaje = cuerpo
                    };
                    await _utilitario.EnviarEmailAsync(valoresCorreo);//General.EnviarMensaje(model.Email, "Restauración de Contraseña", cuerpo);

                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se ha actualizado su contraseña con éxito!";
                }
                else if (resetPass.Errors.FirstOrDefault().Code.Equals("PasswordMismatch"))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se produjo un error al restaurar su contraseña!! \nNo coincide la contraseña actual introducida.";
                }
                else if (resetPass.Errors.FirstOrDefault().Code.Equals("PasswordTooShort"))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se produjo un error al restaurar su contraseña!! \nDebe de ser mayor de 5 caracteres.";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = "Se produjo un error al restaurar su contraseña!!";
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> Exportar()
        {
            var usuarios = await usuarioNegocio.ObtenerUsuariosExcel();
            HttpContext.Response.Headers.Add("content-disposition", "inline; filename=UsuariosActivos.xlsx");
            return File(usuarios, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        public async Task<IActionResult> ExportarPdf()
        {

            var usuarios = usuarioNegocio.ObtenerUsuarios();
            var file = await _utilitario.ObtenerByteDocumento("R01_Usuarios.cshtml", usuarios, HttpContext, ControllerContext);
            return File(file,"application/pdf");
        }

        private async Task<List<SelectListItem>> obtenerRoles(UsusarioParametroDto param)
        {
            var usuario = (param.Origen == 1) && (User.Identity.IsAuthenticated) ? await _userManager.GetUserAsync(User) :
                          (param.Origen == 2) ? null : await _userManager.FindByIdAsync(param.IdUser);

            var rolUser = (usuario != null) ? await usuarioNegocio.obtenerRolUsuario(usuario) : null;

            var rolList = _roleManager.Roles.ToList();
            List<SelectListItem> lst = new List<SelectListItem>();
            //var seleccion = new SelectListItem() { Text = "..:: Seleccione un Rol ::..", Value = " ", Selected = true };
            //lst.Add(seleccion);
            foreach (var item in rolList)
            {
                lst.Add(new SelectListItem() { Text = item.Name, Value = item.Name.ToString(), Selected = (rolUser != null) ? (item.Name.Equals(rolUser)) : false });
            }

            return lst;
        }
    }
}


