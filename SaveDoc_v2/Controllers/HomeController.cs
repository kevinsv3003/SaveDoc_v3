using Dominio.Contratos;
using Entidades.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SaveDoc_v2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SaveDoc_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<UsuarioApp> _userManager;
        private IUsuarioNegocio usuarioNegocio;

        public HomeController(ILogger<HomeController> logger, UserManager<UsuarioApp> _userManager, IUsuarioNegocio usuarioNegocio)
        {
            _logger = logger;
            this._userManager = _userManager;
            this.usuarioNegocio = usuarioNegocio;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var usuario = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(User) : null;
            ViewBag.RolActual = (usuario != null) ? await usuarioNegocio.obtenerRolUsuario(usuario) : "";
            return View();
        }
    }
}
