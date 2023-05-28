using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
   public class TipoMovimientoNegocio : ITipoMovimientoDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public TipoMovimientoNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public Task<bool> ActualizarTipoMovimiento(TipoMovimientoDto TipoMovimiento)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AgregarTipoMovimiento(TipoMovimientoDto TipoMovimiento)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarTipoMovimiento(int TipoMovimientoId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TipoMovimientoDto>> ObtenerListaTipoMovimiento()
        {
            var resultado = new List<TipoMovimientoDto>();
            try
            {
                var listaTipoMovimiento = unidadTrabajo.TipoMovimientoRepositorio.ListarTodos();
                foreach (var item in listaTipoMovimiento)
                {
                    var it = Mapper.Map<TipoMovimiento, TipoMovimientoDto>(item);                    
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al buscar los tipos de movimientos.");

            }
        }

        public async Task<TipoMovimientoDto> ObtenerTipoMovimientoById(int TipoMovimientoId)
        {
            var resultado = new TipoMovimientoDto();
            try
            {
                var tipoMovdb = unidadTrabajo.TipoMovimientoRepositorio.BuscarPorId(TipoMovimientoId);
                resultado = tipoMovdb != null ? Mapper.Map<TipoMovimiento, TipoMovimientoDto>(tipoMovdb) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al obtener cuenta.");

            }
            return resultado;
        }
    }
}
