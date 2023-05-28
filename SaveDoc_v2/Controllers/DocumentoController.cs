using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SaveDoc.Controllers
{
    [Authorize]
    public class DocumentoController : Controller
    {
        private readonly IAreaDominio areaDom;
        private readonly IDocumentoDominio documentoDom;

        int _RegistrosPorPagina = 4;
        Paginador<DocumentoDto> _PaginadorDocumentos;

        public DocumentoController(IAreaDominio area, IDocumentoDominio documento)
        {
            this.areaDom = area;
            this.documentoDom = documento;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var listaDoc = await documentoDom.ObtenerTodosDocumentos();
            _PaginadorDocumentos = PaginacionCards(1, listaDoc.Count, listaDoc);
            ViewBag.documentos = _PaginadorDocumentos;

            var areas = await areaDom.ObtenerListaArea();
            ViewBag.Area = areas;


            return View();
        }

        public async Task<IActionResult> _BuscarDocumentoPorArea(int areaId)
        {
            try
            {
                var listaDoc = (areaId == 99) ? await documentoDom.ObtenerTodosDocumentos() : await documentoDom.ObtenerDocumentoArea(areaId);
                if (listaDoc.Count > 0)
                {
                    _PaginadorDocumentos = PaginacionCards(1, listaDoc.Count, listaDoc);
                    ViewBag.documentos = _PaginadorDocumentos;

                    var areas = await areaDom.ObtenerListaArea();
                    ViewBag.Area = areas;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return PartialView("_Card", _PaginadorDocumentos);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var mensaje = "No existen documentos relacionados a esta área.";
                    return Json(mensaje);
                }

            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        public async Task<IActionResult> _BuscarDocumentoPorNombre(string doc)
        {
            try
            {
                var documento = await documentoDom.ObtenerDocumentoPorNombre(doc);
                if (documento.Count > 0)
                {
                    _PaginadorDocumentos = PaginacionCards(1, documento.Count, documento);
                    ViewBag.documentos = _PaginadorDocumentos;
                    var areas = await areaDom.ObtenerListaArea();
                    ViewBag.Area = areas;
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return PartialView("_Card", _PaginadorDocumentos);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var mensaje = "No existen documentos relacionados a esta área.";
                    return Json(mensaje);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        public IActionResult _NuevoDocumento([FromBody] int docId)
        {
            try
            {
                var documento = documentoDom.ObtenerDocumentoPorId(docId).Result;
                var areas = areaDom.ObtenerListaArea().Result;
                List<SelectListItem> lst = new List<SelectListItem>();
                foreach (var item in areas)
                {
                    lst.Add(new SelectListItem() { Text = item.Nombre, Value = item.AreaId.ToString(), Selected = (documento != null) ? (item.AreaId.Equals(documento.AreaId)) : false });
                }
                ViewBag.Area = lst;
                return PartialView(documento);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> _DescargarDocumento(int id)
        {
            var documento = await documentoDom.ObtenerDocumentoByte(id);
            return File(documento.Doc, documento.Extension);
        }

        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public IActionResult SaveDoc(DocumentoDto doc)
        {
            Thread.Sleep(2000);
            var mensaje = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var retorno = (doc.DocumentoId > 0) ? documentoDom.ActualizarDocumento(doc) : documentoDom.AgregarDocumento(doc);
                    if (retorno.Result)
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        mensaje = "Se registró el documento correctamente!!";
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        mensaje = "Se generó un error al registrar el documento!!";
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

        public async Task<IActionResult> _ListaDocCards()
        {
            try
            {
                var listaDoc = await documentoDom.ObtenerTodosDocumentos();
                return PartialView(listaDoc);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> _EliminarDocumento([FromBody] int docId)
        {
            var mensaje = string.Empty;
            try
            {
                var retorno = await documentoDom.EliminarDocumento(docId, out mensaje);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    //mensaje = "Se eliminó el documento correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    //mensaje = "Se generó un error al eliminar el área!!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                mensaje = ex.Message;
            }
            return Json(mensaje);
        }


        private Paginador<DocumentoDto> PaginacionCards(int pagina, int _TotalRegistros, List<DocumentoDto> Listado)
        {
            var _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);
            pagina = pagina == 0 ? 1 :
                     pagina > _TotalPaginas ? _TotalPaginas :
                     pagina;


            var ListFiltrada = Listado.OrderBy(x => x.AreaId)
                 .Skip((pagina - 1) * _RegistrosPorPagina)
                 .Take(_RegistrosPorPagina)
                 .ToList();



            _PaginadorDocumentos = new Paginador<DocumentoDto>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = pagina,
                Resultado = ListFiltrada
            };

            return _PaginadorDocumentos;
        }

        public async Task<IActionResult> _Card(int pagina = 1, int filtro = 0)
        {
            var Listadoc = (filtro == 0 || filtro == 99) ? await documentoDom.ObtenerTodosDocumentos() : await documentoDom.ObtenerDocumentoArea(filtro);

            _PaginadorDocumentos = PaginacionCards(pagina, Listadoc.Count, Listadoc);

            return PartialView(_PaginadorDocumentos);
        }
    }
}