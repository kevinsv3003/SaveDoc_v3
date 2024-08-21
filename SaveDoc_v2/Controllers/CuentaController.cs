using Dominio.Contratos;
using Dominio.EntidadesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Infraestructura.Transversal.Utilitario;

namespace SaveDoc_v2.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IUtilitario _utilitario;
        private readonly ICuentaDominio _cuentaDominio;
        private readonly IMovimientoDominio _movimientoDominio;
        private readonly ITipoMovimientoDominio _tipoMovimientoDominio;
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        public CuentaController(ICuentaDominio _cuentaDominio, IMovimientoDominio _movimientoDominio, ITipoMovimientoDominio _tipoMovimientoDominio, IUtilitario _utilitario)
        {
            this._cuentaDominio = _cuentaDominio;
            this._movimientoDominio = _movimientoDominio;
            this._tipoMovimientoDominio = _tipoMovimientoDominio;
            this._utilitario = _utilitario;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            var cuentas = _cuentaDominio.ObtenerListaCuenta();
            var cuentaPrincipal = cuentas.Result.FirstOrDefault();
            var mesesAnio = _utilitario.ListaMesesDelAnio();
            var filtroAnio = _utilitario.ListaFiltroAnios();

            List<SelectListItem> lstCuentas = new List<SelectListItem>();
            SelectListItem select = new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0" };
            lstCuentas.Add(select);
            cuentas.Result.ForEach(x => {
                lstCuentas.Add(new SelectListItem()
                {
                    Text = string.Format("{0} - {1}",x.NombreCuenta,x.NumCuenta),
                    Value = x.CuentaId.ToString(),
                    Selected = x.CuentaId.Equals(cuentaPrincipal.CuentaId)
                });
            });

            ViewBag.Cuentas = lstCuentas;
            ViewBag.Meses = mesesAnio;
            ViewBag.FiltroAnios = filtroAnio;
            return View();
        }

        public async Task<IActionResult> _TablaMovimiento(int cuentaId, int mes, int anio)
        {
            try
            {
                nfi = (NumberFormatInfo)nfi.Clone();
                nfi.CurrencySymbol = string.Format("{0} ",_cuentaDominio.ObtenerCuentaById(cuentaId).Result.simboloMoneda);
                var listaMovimientoTotal = await _movimientoDominio.ObtenerListaMovimiento(cuentaId);
                var listaMovimiento = listaMovimientoTotal != null && listaMovimientoTotal.Count > 0 
                    ? listaMovimientoTotal.Where(x => x.FechaMovimiento.Value.Month.Equals(mes) && x.FechaMovimiento.Value.Year.Equals(anio)).ToList() : new List<MovimientoDto>();
                listaMovimiento.ForEach(x => { x.MontoFormato = string.Format(nfi, "{0:C2}", x.Monto); });

                var listaOrdenada = listaMovimiento.OrderByDescending(x => x.FechaMovimiento.Value).ToList();

                return PartialView(listaOrdenada);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }


        public async Task<IActionResult> _EstadoCuenta(int cuentaId, int mes, int anio)
        {
            try
            {
                nfi = (NumberFormatInfo)nfi.Clone();
                nfi.CurrencySymbol = string.Format("{0} ", _cuentaDominio.ObtenerCuentaById(cuentaId).Result.simboloMoneda);
                var cuentaAhorro = await _cuentaDominio.ObtenerCuentaById(cuentaId);
                var totalMovimientos = await _movimientoDominio.ObtenerListaMovimiento(cuentaId);
                var gastosMovimientos = totalMovimientos.Where(x => x.FechaMovimiento.Value.Month.Equals(mes) && x.FechaMovimiento.Value.Year.Equals(anio) && x.TipoMovimientoId.Equals((int)TIPO_MOVIMIENTO.GASTOS)).Sum(x => x.Monto);
                var ingresosMovimientos = totalMovimientos.Where(x => x.FechaMovimiento.Value.Month.Equals(mes) && x.FechaMovimiento.Value.Year.Equals(anio) && x.TipoMovimientoId.Equals((int)TIPO_MOVIMIENTO.INGRESOS)).Sum(x => x.Monto);

                cuentaAhorro.totalSaldoActual = string.Format(nfi, "{0:C2}", cuentaAhorro.SaldoActual);
                cuentaAhorro.totalGastos = string.Format(nfi, "{0:C2}", gastosMovimientos);
                cuentaAhorro.totalIngresos = string.Format(nfi, "{0:C2}", ingresosMovimientos);

                return PartialView(cuentaAhorro);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> _FormMovimiento(int cuentaId, int movimientoId)
        {
            var simboloCuenta = string.Format("{0} ", _cuentaDominio.ObtenerCuentaById(cuentaId).Result.simboloMoneda);
            var movimiento = await _movimientoDominio.ObtenerMovimientoById(movimientoId);
            var tipoMovimientos = await _tipoMovimientoDominio.ObtenerListaTipoMovimiento();
            List<SelectListItem> lstTipoMovimientos = new List<SelectListItem>();
            SelectListItem select = new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0" };
            lstTipoMovimientos.Add(select);
            tipoMovimientos.ForEach(x =>
            {
                lstTipoMovimientos.Add(new SelectListItem()
                {
                    Text = x.TipoMov,
                    Value = x.TipoMovimientoId.ToString()                    
                });
            });

            ViewBag.TipoMovimientos = lstTipoMovimientos;
            ViewBag.CuentaId = cuentaId;
            ViewBag.SimboloCuenta = simboloCuenta;
            return PartialView(movimiento);
        }

        public async Task<IActionResult> GuardarMovimiento(MovimientoDto movimiento)
        {
            Thread.Sleep(1500);
            var mensaje = string.Empty;
            try
            {
                var guardado = movimiento.MovimientoId >0 ? await _movimientoDominio.ActualizarMovimiento(movimiento) : await _movimientoDominio.AgregarMovimiento(movimiento);                
                if (guardado)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se ha guardado el movimiento correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se produjo un error al guardar el movimiento!!";
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            return Json(mensaje);
        }

        public async Task<IActionResult> EliminarMovimiento([FromBody] int movimientoId)
        {
            var mensaje = string.Empty;
            try
            {
                var movimiento = await _movimientoDominio.ObtenerMovimientoById(movimientoId);
                var retorno = await _movimientoDominio.EliminarMovimiento(movimiento);
                if (retorno)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    mensaje = "Se eliminó el movimiento correctamente!!";
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    mensaje = "Se generó un error al eliminar el movimiento!!";
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
