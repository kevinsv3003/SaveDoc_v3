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
    public class CuentaNegocio : ICuentaDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public CuentaNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }


        public async Task<bool> ActualizarMontoCuenta(MovimientoDto movimientoDto)
        {
            var cuentaDb = unidadTrabajo.CuentaRepositorio.BuscarPorId(movimientoDto.CuentaId);
            cuentaDb.SaldoActual = movimientoDto.TipoMovimientoId.Equals((int)TIPO_MOVIMIENTO.GASTOS) ? cuentaDb.SaldoActual - movimientoDto.Monto : cuentaDb.SaldoActual + movimientoDto.Monto;

            unidadTrabajo.CuentaRepositorio.Actualizar(cuentaDb);
            unidadTrabajo.Commit();

            return true;

        }

        public Task<bool> AgregarCuenta(CuentaDto cuenta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarCuenta(int cuentaId)
        {
            throw new NotImplementedException();
        }

        public async Task<CuentaDto> ObtenerCuentaById(int cuentaId)
        {
            var resultado = new CuentaDto();
            try
            {
                var cuentadb = unidadTrabajo.CuentaRepositorio.BuscarPorId(cuentaId);
                resultado = cuentadb != null ? Mapper.Map<Cuenta, CuentaDto>(cuentadb) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al obtener cuenta.");

            }
            return resultado;
        }

        public async Task<List<CuentaDto>> ObtenerListaCuenta()
        {
            var resultado = new List<CuentaDto>();
            try
            {
                var cuentadb = unidadTrabajo.CuentaRepositorio.ListarTodos();
                foreach (var item in cuentadb)
                {
                    var it = Mapper.Map<Cuenta, CuentaDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al obtener cuenta.");

            }
        }
    }
}
