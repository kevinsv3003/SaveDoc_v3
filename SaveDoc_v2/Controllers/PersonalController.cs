using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Infraestructura.Transversal.Utilitario;

namespace SaveDoc_v2.Controllers
{
    public class PersonalController : Controller
    {
        private readonly IPersonalDominio _personalDominio;
        private readonly IMunicipioDominio _municipioDominio;
        private readonly IBarrioDominio _barrioDominio;
        private readonly IDepartamentoDominio _departamentoDominio;
        private readonly IRentaDominio _rentaDominio;
        private readonly IOficinaDominio _oficinaDominio;
        private readonly IDetalleActividadDominio _DetalleActividadDominio;
        private readonly IObservacionDominio _ObservacionDominio;
        private UserManager<UsuarioApp> _userManager;


        public PersonalController(IPersonalDominio _personalDominio, IBarrioDominio _barrioDominio, IRentaDominio _rentaDominio, IMunicipioDominio _municipioDominio, UserManager<UsuarioApp> _userManager,
            IOficinaDominio _oficinaDominio, IDepartamentoDominio _departamentoDominio, IDetalleActividadDominio _DetalleActividadDominio, IObservacionDominio _ObservacionDominio)
        {
            this._personalDominio = _personalDominio;
            this._rentaDominio = _rentaDominio;
            this._municipioDominio = _municipioDominio;
            this._barrioDominio = _barrioDominio;
            this._departamentoDominio = _departamentoDominio;
            this._oficinaDominio = _oficinaDominio;
            this._ObservacionDominio = _ObservacionDominio;
            this._DetalleActividadDominio = _DetalleActividadDominio;
            this._userManager = _userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> _TablaPersonal()
        {
            try
            {
                var usuarioCoordinador = await _userManager.GetUserAsync(User);
                //var personal = _personalDominio.ObtenerListaPersonal().Result;
                var personal = _personalDominio.ObtenerListaPersonal().Result.Where(p => p.RentaId.Equals(usuarioCoordinador.RentaId)).ToList();
                personal.ForEach(p =>
                {
                    p.Direccion = p.BarrioDes != null && !p.BarrioDes.Equals("") ? !(p.BarrioDes.Contains("Comarca") || p.BarrioDes.Contains("Carretera")) ? "B° " + p.BarrioDes + ", " + p.Direccion : p.BarrioDes + ", " + p.Direccion : p.Direccion;
                });

                return PartialView(personal.OrderByDescending(x => x.Oficina.Nombre).ToList());
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> _FormEditPersonal([FromBody] UsusarioParametroDto parametros)
        {
            try
            {
                var personalId = !(parametros.IdUser.Equals("")) ? Int32.Parse(parametros.IdUser) : 0;
                var origen = parametros.Origen;
                var personal = personalId > 0 ? await _personalDominio.ObtenerPersonalById(personalId) : new PersonalDto();
                //personal.Direccion = personal.BarrioDes != null && !personal.BarrioDes.Equals("") ? "B° " + personal.BarrioDes + ", " + personal.Direccion : personal.Direccion;
                var departamentos = await _departamentoDominio.ObtenerListaDepartamento();
                var municipios = await _municipioDominio.ListaMunicipiosByDepartamentoId(personal.CodDepartamento);
                var barrios = await _barrioDominio.ListaBarriosByMunicipioId(personal.CodMunicipio);
                var rentas = await _rentaDominio.ObtenerListaRenta();
                var oficinas = await _oficinaDominio.ObtenerListaOficina();
                List<SelectListItem> lstDep = new List<SelectListItem>();
                List<SelectListItem> lstMun = new List<SelectListItem>();
                List<SelectListItem> lstBarrios = new List<SelectListItem>();
                List<SelectListItem> lstRenta = new List<SelectListItem>();
                List<SelectListItem> lstOficina = new List<SelectListItem>();
                SelectListItem select = new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0" };

                lstDep.Add(select);
                lstMun.Add(select);
                lstBarrios.Add(select);
                lstRenta.Add(select);
                lstOficina.Add(select);

                departamentos.ForEach(x =>
                {
                    lstDep.Add(new SelectListItem()
                    {
                        Text = x.DescripcionDepartamento,
                        Value = x.DepartamentoId.ToString(),
                        Selected = (personal != null) ? x.DepartamentoId.Equals(personal.CodDepartamento) : false
                    });
                });
                municipios.ForEach(x =>
                {
                    lstMun.Add(new SelectListItem()
                    {
                        Text = x.DescripcionMunicipio,
                        Value = x.MunicipioId.ToString(),
                        Selected = (personal != null) ? x.MunicipioId.Equals(personal.CodMunicipio) : false
                    });
                });
                barrios.ForEach(x =>
                {
                    lstBarrios.Add(new SelectListItem()
                    {
                        Text = x.NombreBarrio,
                        Value = x.BarrioId.ToString(),
                        Selected = (personal != null) ? x.BarrioId.Equals(personal.CodBarrio) : false
                    });
                });
                rentas.ForEach(x =>
                {
                    lstRenta.Add(new SelectListItem()
                    {
                        Text = x.NombreRenta,
                        Value = x.RentaId.ToString(),
                        Selected = (personal != null) ? x.RentaId.Equals(personal.RentaId) : false
                    });
                });
                oficinas.ForEach(x =>
                {
                    lstOficina.Add(new SelectListItem()
                    {
                        Text = x.Nombre,
                        Value = x.OficinaId.ToString(),
                        Selected = (personal != null) ? x.OficinaId.Equals(personal.OficinaId) : false
                    });
                });


                ViewBag.Departamentos = lstDep;
                ViewBag.Municipios = lstMun;
                ViewBag.Barrios = lstBarrios;
                ViewBag.Rentas = lstRenta;
                ViewBag.Oficinas = lstOficina;
                ViewBag.Origen = origen;
                return PartialView(personal);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> DetalleInformePersonal(int personalId)
        {
            var personal = personalId > 0 ? await _personalDominio.ObtenerPersonalById(personalId) : new PersonalDto();
            personal.Direccion = personal.BarrioDes != null && !personal.BarrioDes.Equals("") ? !(personal.BarrioDes.Contains("Comarca") || personal.BarrioDes.Contains("Carretera")) ? "B° " + personal.BarrioDes + ", " + personal.Direccion : personal.BarrioDes + ", " + personal.Direccion : personal.Direccion;
            var actividades = await _DetalleActividadDominio.ObtenerListaDetalleActividadByPersonalId(personalId);
            var actividadesUltimoMes = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(DateTime.Now.Year) && x.Actividad.Fecha.Value.Month.Equals(DateTime.Now.Month)).Count();
            var actividadesEsteAño = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(DateTime.Now.Year)).ToList();
            var actividadesAsistio = actividadesEsteAño.Where(x => x.Asistio || x.Justificado).Count();
            var porcAsistencia = actividadesEsteAño.Count() != 0 ? (Convert.ToDouble(actividadesAsistio) / Convert.ToDouble(actividadesEsteAño.Count())) * 100 : 0;
            ViewBag.PorcentajeAsistencia = ((int)porcAsistencia);
            return View(personal);
        }

        public async Task<IActionResult> _FormObservaciones([FromBody] int personalId)
        {
            var personal = personalId > 0 ? await _personalDominio.ObtenerPersonalById(personalId) : new PersonalDto();
            ViewBag.PersonalId = personalId;
            return PartialView(personal.Observaciones.OrderByDescending(x => x.ObservacionId).ToList());
        }
        
        public async Task<IActionResult> _ActividadesPersonal([FromBody] int personalId)
        {
            var anioActual = DateTime.Now.Year;
            var actividades = await _DetalleActividadDominio.ObtenerListaDetalleActividadByPersonalId(personalId);
            actividades = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(anioActual)).ToList();

            var actividadesAsistio = actividades.Where(x => x.Asistio || x.Justificado).Count();
            var porcAsistencia = actividades.Count() != 0 ? (Convert.ToDouble(actividadesAsistio) / Convert.ToDouble(actividades.Count())) * 100 : 0;
            ViewBag.PersonalId = personalId;
            ViewBag.CantActividades = actividades.Count;
            ViewBag.PorcentajeAsistencia = ((int)porcAsistencia);

            return PartialView(actividades);
        }

        public async Task<IActionResult> _CardActividades(int personalId, int anio, int mes)
        {
            var actividades = await _DetalleActividadDominio.ObtenerListaDetalleActividadByPersonalId(personalId);
            if (anio > 1 && (mes > 0 && mes < 13))
                actividades = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(anio) && x.Actividad.Fecha.Value.Month.Equals(mes)).ToList();
            else if (anio == 1 && (mes > 0 && mes < 13))
                actividades = actividades.Where(x => x.Actividad.Fecha.Value.Month.Equals(mes)).ToList();
            else if (anio > 1 && mes == 13)
                actividades = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(anio)).ToList();


            var actividadesAsistio = actividades.Where(x => x.Asistio || x.Justificado).Count();
            var porcAsistencia = actividades.Count() != 0 ? (Convert.ToDouble(actividadesAsistio) / Convert.ToDouble(actividades.Count())) * 100 : 0;
            ViewBag.PersonalId = personalId;
            ViewBag.CantActividades = actividades.Count;
            ViewBag.PorcentajeAsistencia = ((int)porcAsistencia);

            return PartialView(actividades);
        }

        public async Task<IActionResult> _FiltroReporte()
        {
            var observaciones = await _ObservacionDominio.ObtenerListaObservacion();
            List<SelectListItem> lstObservaciones = new List<SelectListItem>();
            observaciones.ForEach(x => { lstObservaciones.Add(new SelectListItem() { Text = x, Value = x }); });
            ViewBag.Observaciones = lstObservaciones;

            return PartialView();
        }

        public async Task<IActionResult> GenerarReporte(int TipoReporte, string Genero, bool Managua, string NombreReporte, string Observaciones, string Restricciones)
        {
            NombreReporte = TipoReporte > 0 ? NombreReporte : "ReporteArmadoPersonalDIS";
            TipoReporte = TipoReporte > 0 ? TipoReporte : (int)TIPO_REPORTE.ARMADO;

            var LstObservaciones = Observaciones != null ? Observaciones.Split(',').ToList() : new List<string>();
            var LstRestricciones = Restricciones != null ? Restricciones.Split(',').ToList() : new List<string>();

            var usuarios = await _personalDominio.GenerarReporteDelPersonal(TipoReporte, Genero, Managua, LstObservaciones, LstRestricciones);

            HttpContext.Response.Headers.Add("content-disposition", "inline; filename=" + NombreReporte + ".xlsx");
            return File(usuarios, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IActionResult> GenerarReporteDetallePersonal(int personalId)
        {
            var reporte = await _DetalleActividadDominio.GenerarReporteDetallePersonal(personalId);
            HttpContext.Response.Headers.Add("content-disposition", "inline; filename=ReporteDetallePersonal.xlsx");
            return File(reporte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IActionResult> saveObservacion(int personalId, string observacion)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var observa = new ObservacionDto()
                {
                    PersonalId = personalId,
                    ObservacionDet = observacion
                };
                var guardado = observacion != null ? await _ObservacionDominio.AgregarObservacion(observa) : false;
                if (guardado)
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
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }
       
        public async Task<IActionResult> _EliminarObservacion([FromBody] int ObservacionId)
        {
            var mensaje = string.Empty;
            try
            {
                var retorno = await _ObservacionDominio.EliminarObservacion(ObservacionId);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se eliminó la observación correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se generó un error al eliminar la observación!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = ex.Message;
            }
            return Json(mensaje);
        }
        
        public async Task<IActionResult> GuardarPersonal(PersonalDto personalDto)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (personalDto.PersonalId > 0)
                    {
                        var actualizado = await _personalDominio.ActualizarPersonal(personalDto);
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

                        var guardado = await _personalDominio.AgregarPersonal(personalDto);
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

        public async Task<IActionResult> CargarSegunDepartamento(string iddepartamento)
        {
            List<SelectListItem> lstMun = new List<SelectListItem>();
            lstMun.Add(new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0", Selected = true });
            if (iddepartamento != "0")
            {
                var municipios = await _municipioDominio.ListaMunicipiosByDepartamentoId(int.Parse(iddepartamento));
                municipios.ForEach(x =>
                {
                    lstMun.Add(new SelectListItem()
                    {
                        Text = x.DescripcionMunicipio,
                        Value = x.MunicipioId.ToString(),
                    });
                });
            }
            return new JsonResult(lstMun);
        }

        public async Task<IActionResult> CargarSegunMunicipio(string idmunicipio)
        {
            List<SelectListItem> lstBarrios = new List<SelectListItem>();
            lstBarrios.Add(new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0", Selected = true });
            if (idmunicipio != "0")
            {
                var barrios = await _barrioDominio.ListaBarriosByMunicipioId(int.Parse(idmunicipio));
                barrios.ForEach(x =>
                {
                    lstBarrios.Add(new SelectListItem()
                    {
                        Text = x.NombreBarrio,
                        Value = x.BarrioId.ToString(),
                    });
                });
            }
            return new JsonResult(lstBarrios);
        }


    }
}
