using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SaveDoc.Controllers
{
    [Authorize]
    public class AreaController : Controller
    {
        private readonly IAreaDominio areaDom;

        public AreaController(IAreaDominio areaDom)
        {
            this.areaDom = areaDom;
        }

        [Authorize(Roles ="Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> _ListaAreaCuadro()
        {
            
            try
            {
                var listaArea = await areaDom.ObtenerListaArea();
                return PartialView(listaArea);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> _FormularioArea([FromBody]int AreaId)
        {
            try
            {
                var area = await areaDom.ObtenerAreaById(AreaId);
                return PartialView(area);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> GuardarArea(AreaDto area)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var retorno = (area.AreaId > 0) ? await areaDom.ActualizarArea(area) : await areaDom.AgregarArea(area);
                    if (retorno)
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        mensaje = "Se registró el área correctamente!!";
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        mensaje = "Se generó un error al registrar el área!!";
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Favor introducir correctamente los datos!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = ex.Message;
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> _EliminarArea([FromBody] int AreaId)
        {
            var mensaje = string.Empty;
            try
            {
                var retorno = await areaDom.EliminarArea(AreaId);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se eliminó el área correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se generó un error al eliminar el área!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = ex.Message;
            }
            return Json(mensaje);
        }

    }
}