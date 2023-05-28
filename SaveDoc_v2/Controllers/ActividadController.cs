using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using ExcelDataReader;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SaveDoc_v2.Controllers
{
    public class ActividadController : Controller
    {
        private readonly IActividadDominio _actividadDominio;
        private readonly IPersonalDominio _personalDominio;
        private readonly IRentaDominio _rentaDominio;
        private readonly IObservacionDominio _observacionesDominio;
        private readonly IDetalleActividadDominio _detalleActividadDominio;
        private UserManager<UsuarioApp> _userManager;

        int _RegistrosPorPagina = 4;
        Paginador<ActividadDto> _PaginadorActividades;


        public ActividadController(IActividadDominio _actividadDominio, IDetalleActividadDominio _detalleActividadDominio, IPersonalDominio _personalDominio, IRentaDominio _rentaDominio, IObservacionDominio _observacionesDominio, UserManager<UsuarioApp> _userManager)
        {
            this._actividadDominio = _actividadDominio;
            this._detalleActividadDominio = _detalleActividadDominio;
            this._personalDominio = _personalDominio;
            this._rentaDominio = _rentaDominio;
            this._observacionesDominio = _observacionesDominio;
            this._userManager = _userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var usuarioCoordinador = await _userManager.GetUserAsync(User);
            var listadoActividades = await _actividadDominio.ObtenerListaActividad(usuarioCoordinador.RentaId);
            _PaginadorActividades = PaginacionCards(1, listadoActividades.Count, listadoActividades);
            ViewBag.PaginadorActividades = _PaginadorActividades;

            return View();
        }
        public async Task<IActionResult> _CardActividades(int pagina = 1, int filtro = 0)
        {
            var usuarioCoordinador = await _userManager.GetUserAsync(User);
            var listadoActividades = await _actividadDominio.ObtenerListaActividad(usuarioCoordinador.RentaId);
            _PaginadorActividades = PaginacionCards(pagina, listadoActividades.Count, listadoActividades);
            return PartialView(_PaginadorActividades);
        }

        public async Task<IActionResult> _FormActividad()
        {
            return PartialView();
        }

        public async Task<IActionResult> _TablaParticipantes([FromBody] int actividadId)
        {
            var usuarioCoordinador = await _userManager.GetUserAsync(User);
            //var listaPersonal = _personalDominio.ObtenerListaPersonal().Result;
            var listaPersonal = _personalDominio.ObtenerListaPersonal().Result.Where(p => p.RentaId.Equals(usuarioCoordinador.RentaId)).ToList();
            var detalleActividad = await _detalleActividadDominio.ObtenerListaDetalleActividadByActividadlId(actividadId);
            listaPersonal.ForEach(x =>
            {
                x.EsParticipante = detalleActividad.Exists(d => d.PersonalId.Equals(x.PersonalId));
            });
            ViewBag.ActividadId = actividadId;
            return PartialView(listaPersonal.OrderByDescending(x => x.EsParticipante).ThenBy(m => m.OficinaId).ToList());
        }

        public async Task<IActionResult> _FiltroPropuestaPersonal()
        {
            var rentas = await _rentaDominio.ObtenerListaRenta();
            var observaciones = await _observacionesDominio.ObtenerListaObservacion();
            List<SelectListItem> lstRenta = new List<SelectListItem>();
            List<SelectListItem> lstObservaciones = new List<SelectListItem>();
            SelectListItem select = new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0" };
            lstRenta.Add(select);
            //lstObservaciones.Add(select);
            rentas.ForEach(x =>
            {
                lstRenta.Add(new SelectListItem()
                {
                    Text = x.NombreRenta,
                    Value = x.RentaId.ToString(),
                    Selected = x.RentaId.Equals(2)
                });
            });
            observaciones.ForEach(x =>
            {
                lstObservaciones.Add(new SelectListItem()
                {
                    Text = x,
                    Value = x
                });
            });

            ViewBag.Rentas = lstRenta;
            ViewBag.Observaciones = lstObservaciones;
            return PartialView();
        }

        public async Task<IActionResult> PropuestaPersonal(int cantidad, int renta, string genero, bool EsManagua, bool EsParticipativo, string[] observaciones)
        {
            Thread.Sleep(2000);
            var usuarioCoordinador = await _userManager.GetUserAsync(User);
            renta = usuarioCoordinador.RentaId;
            var listaPersonal = _personalDominio.ObtenerListaPersonal().Result.Where(p => p.RentaId.Equals(usuarioCoordinador.RentaId)).ToList();
            var listaPropuesta = _actividadDominio.ObtenerPropuestaPersonalActividad(cantidad, renta, genero, EsManagua, EsParticipativo, observaciones.ToList());
            listaPersonal.ForEach(x =>
            {
                x.EsParticipante = listaPropuesta.Contains(x.PersonalId);
            });
            return PartialView("_TablaParticipantes", listaPersonal.OrderByDescending(x => x.EsParticipante).ThenBy(m => m.OficinaId).ToList());
        }

        public async Task<IActionResult> _AgregarParticipantes([FromBody] int actividadId)
        {
            var actividad = await _actividadDominio.ObtenerActividadById(actividadId);
            ViewBag.ActividadId = actividadId;
            return PartialView(actividad);
        }

        public async Task<IActionResult> _DetalleActividad(int actividadId, int personalId)
        {
            var listaPersonal = new List<PersonalDto>();
            var detalleActividad = await _detalleActividadDominio.ObtenerListaDetalleActividadByActividadlId(actividadId);

            ViewBag.DetalleActividadId = personalId > 0 ? detalleActividad.Where(x => x.PersonalId.Equals(personalId)).FirstOrDefault().DetalleActividadId : 0;
            return PartialView(detalleActividad);
        }
        public async Task<IActionResult> _DetalleParticipante([FromBody] int detalleActividadId)
        {
            var detalleActividad = detalleActividadId > 0 ? await _detalleActividadDominio.ObtenerDetalleActividadById(detalleActividadId) : new DetalleActividadDto();

            return PartialView(detalleActividad);
        }

        public async Task<IActionResult> saveParticipante(int actividadId, string[] personal)
        {
            Thread.Sleep(1000);

            var mensaje = string.Empty;
            try
            {
                var listaBorrar = new List<int>();
                var listaAgregar = new List<int>();
                var detalleActividad = await _detalleActividadDominio.ObtenerListaDetalleActividadByActividadlId(actividadId);

                detalleActividad.ForEach(x =>
                {
                    var existe = personal.ToList().Exists(p => p.Equals(x.PersonalId.ToString()));
                    if (!existe) listaBorrar.Add(x.PersonalId);
                });

                personal.ToList().ForEach(x =>
                {
                    var existe = detalleActividad.Exists(da => da.PersonalId.ToString().Equals(x));
                    if (!existe) listaAgregar.Add(Convert.ToInt32(x));
                });

                listaBorrar.ForEach(b =>
                {
                    _detalleActividadDominio.BorrarDetalleByActividadIdYPersonalId(actividadId, b);
                });

                listaAgregar.ForEach(a =>
                {
                    var detalle = new DetalleActividadDto()
                    {
                        PersonalId = a,
                        ActividadId = actividadId,
                        Asistio = true,
                        Transporte = false
                    };
                    _detalleActividadDominio.AgregarDetalleActividad(detalle);
                });

                Response.StatusCode = (int)HttpStatusCode.OK;
                mensaje = "Se ha actualizado su información correctamente!!";
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }
        public async Task<IActionResult> BorrarDetalleActividad(int actividadId)
        {
            var mensaje = string.Empty;
            try
            {
                var borrado = await _detalleActividadDominio.BorrarDetalleByActividadId(actividadId);
                if (borrado)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se ha borrado el registro correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se ha producido un error al borrar el registro correctamente!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> GuardarActividad(ActividadDto actividadDto)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var usuarioCoordinador = await _userManager.GetUserAsync(User);
                actividadDto.RentaId = usuarioCoordinador.RentaId;
                var guardado = await _actividadDominio.AgregarActividad(actividadDto);
                if (guardado)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se ha guardado la actividad correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se produjo un error al guardar la actividad!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }
        public async Task<IActionResult> GuardarDetalleParticipante(DetalleActividadDto detalleActividadDto)
        {
            Thread.Sleep(700);
            var mensaje = string.Empty;
            try
            {
                var guardado = await _detalleActividadDominio.ActualizarDetalleActividad(detalleActividadDto);
                if (guardado)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se ha guardado la información correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se produjo un error al guardar la información!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> GenerarReporteActividad(int actividadId)
        {

            var reporteActividad = await _detalleActividadDominio.GenerarReporteActividad(actividadId);
            //leerExcel(reporteActividad);
            var nombreActividad = _actividadDominio.ObtenerActividadById(actividadId).Result.NombreActividad;
            HttpContext.Response.Headers.Add("content-disposition", "inline; filename=Reporte_"+nombreActividad+".xlsx");
            return File(reporteActividad, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        public async Task<IActionResult> GenerarReporteParticipante(int actividadId)
        {
            var reporteActividad = await _detalleActividadDominio.GenerarReporteParticipante(actividadId);
            //leerExcel(reporteActividad);
            var nombreActividad = _actividadDominio.ObtenerActividadById(actividadId).Result.NombreActividad;
            HttpContext.Response.Headers.Add("content-disposition", "inline; filename=Reporte_" + nombreActividad + ".xlsx");
            return File(reporteActividad, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        public DataRowCollection leerExcel (byte [] excel)
        {
            Stream stream = new MemoryStream(excel);
            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet resultDataSet = excelDataReader.AsDataSet();
            var Tabla = resultDataSet.Tables[0];
            var Contenido = Tabla.Rows;

            return Contenido;
        }




        private Paginador<ActividadDto> PaginacionCards(int pagina, int _TotalRegistros, List<ActividadDto> Listado)
        {
            var _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);
            pagina = pagina == 0 ? 1 :
                     pagina > _TotalPaginas ? _TotalPaginas :
                     pagina;


            var ListFiltrada = Listado.OrderByDescending(x => x.Fecha)
                 .Skip((pagina - 1) * _RegistrosPorPagina)
                 .Take(_RegistrosPorPagina)
                 .ToList();

            _PaginadorActividades = new Paginador<ActividadDto>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = pagina,
                Resultado = ListFiltrada
            };

            return _PaginadorActividades;
        }
    
    
    
    }
}
