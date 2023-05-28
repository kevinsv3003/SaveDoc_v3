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
    public class BarrioNegocio : IBarrioDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public BarrioNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public Task<bool> ActualizarBarrio(BarrioDto Barrio)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AgregarBarrio(BarrioDto Barrio)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BarrioDto>> ListaBarriosByMunicipioId(int MunicipioId)
        {
            var resultado = new List<BarrioDto>();
            try
            {
                var barrios = unidadTrabajo.BarrioRepositorio.Buscar(x => x.MunicipioId.Equals(MunicipioId));
                foreach (var item in barrios)
                {
                    var it = Mapper.Map<Barrio, BarrioDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los barrios");
            }
        }

        public async Task<BarrioDto> ObtenerBarrioById(int BarrioId)
        {
            var resultado = new BarrioDto();
            try
            {
                var barrio = unidadTrabajo.BarrioRepositorio.BuscarPorId(BarrioId);
                resultado = barrio != null ? Mapper.Map<Barrio, BarrioDto>(barrio) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los barrios");
            }
            return resultado;
        }

        public async Task<List<BarrioDto>> ObtenerListaBarrio()
        {
            var resultado = new List<BarrioDto>();
            try
            {
                var barrios = unidadTrabajo.BarrioRepositorio.ListarTodos();
                foreach (var item in barrios)
                {
                    var it = Mapper.Map<Barrio, BarrioDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los barrios");
            }
        }
    }
}
