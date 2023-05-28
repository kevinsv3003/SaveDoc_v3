using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Infraestructura.Transversal.Utilitario;

namespace Dominio.ReglaNegocio
{
    public class MovimientoNegocio : IMovimientoDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly ICuentaDominio cuentaDominio;
        private readonly ITipoMovimientoDominio tipoMovDominio;
        public MovimientoNegocio(IUnidadTrabajo unidadTrabajo, ICuentaDominio cuentaDominio, ITipoMovimientoDominio tipoMovDominio)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.cuentaDominio = cuentaDominio;
            this.tipoMovDominio = tipoMovDominio;
        }

        public async Task<bool> ActualizarMovimiento(MovimientoDto movimiento)
        {
            try
            {
                var movimientodb = unidadTrabajo.MovimientoRepositorio.BuscarPorId(movimiento.MovimientoId);
                var movimientoEliminar = Mapper.Map<Movimiento, MovimientoDto>(movimientodb);
                var eliminarMovimiento = await this.EliminarMovimiento(movimientoEliminar);

                movimiento.MovimientoId = 0;
                var ingresarMovimiento = await this.AgregarMovimiento(movimiento);

                var retorno = eliminarMovimiento && ingresarMovimiento;
                return retorno;

            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al actualizar el movimiento.");
            }
        }

        public async Task<bool> AgregarMovimiento(MovimientoDto movimiento)
        {
            try
            {
                var movimientodb = Mapper.Map<MovimientoDto, Movimiento>(movimiento);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    cuentaDominio.ActualizarMontoCuenta(movimiento);
                    unidadTrabajo.MovimientoRepositorio.Insertar(movimientodb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception) 
            {
                unidadTrabajo.RollBack();
                throw new Exception("Ocurrió un problema al agregar un nuevo movimiento.");

            }
        }

        public async Task<bool> EliminarMovimiento(MovimientoDto movimiento)
        {
            try
            {
                var retorno = await Task.Factory.StartNew(() =>
                {
                    movimiento.Monto = (-1) * movimiento.Monto; //se múltiplica por -1 para cambiar el tipo de movimiento y se reste de la deuda total
                    cuentaDominio.ActualizarMontoCuenta(movimiento);
                    unidadTrabajo.MovimientoRepositorio.Eliminar(movimiento.MovimientoId);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al eliminar el movimiento.");

            }
        }

        public async Task<List<MovimientoDto>> ObtenerListaMovimiento(int cuentaId)
        {
            var resultado = new List<MovimientoDto>();
            try
            {
                var movimientos = unidadTrabajo.MovimientoRepositorio.Buscar(x => x.CuentaId.Equals(cuentaId));
                foreach (var item in movimientos)
                {
                    var it = Mapper.Map<Movimiento, MovimientoDto>(item);
                    it.TipoMovimiento = tipoMovDominio.ObtenerTipoMovimientoById(it.TipoMovimientoId).Result.TipoMov;
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los movimientos");
            }
        }



        public async Task<MovimientoDto> ObtenerMovimientoById(int movimientoId)
        {
            var resultado = new MovimientoDto();
            try
            {
                var movimientodb = unidadTrabajo.MovimientoRepositorio.BuscarPorId(movimientoId);
                resultado = movimientodb != null ? Mapper.Map<Movimiento, MovimientoDto>(movimientodb) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al obtener el movimiento.");

            }
            return resultado;
        }
    }
}
