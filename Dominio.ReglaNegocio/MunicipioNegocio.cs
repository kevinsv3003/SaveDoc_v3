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
    public class MunicipioNegocio : IMunicipioDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public MunicipioNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public Task<bool> ActualizarMunicipio(MunicipioDto Municipio)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AgregarMunicipio(MunicipioDto Municipio)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MunicipioDto>> ListaMunicipiosByDepartamentoId(int DepartamentoId)
        {
            var resultado = new List<MunicipioDto>();
            try
            {
                var municipios = unidadTrabajo.MunicipioRepositorio.Buscar(x => x.DepartamentoId.Equals(DepartamentoId));
                foreach (var item in municipios)
                {
                    var it = Mapper.Map<Municipio, MunicipioDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los municipios");
            }
        }

        public async Task<List<MunicipioDto>> ObtenerListaMunicipio()
        {
            var resultado = new List<MunicipioDto>();
            try
            {
                var municipios = unidadTrabajo.MunicipioRepositorio.ListarTodos();
                foreach (var item in municipios)
                {
                    var it = Mapper.Map<Municipio, MunicipioDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los municipios");
            }
        }

        public async Task<MunicipioDto> ObtenerMunicipioById(int MunicipioId)
        {
            var resultado = new MunicipioDto();
            try
            {
                var municipio = unidadTrabajo.MunicipioRepositorio.BuscarPorId(MunicipioId);
                resultado = municipio != null ? Mapper.Map<Municipio, MunicipioDto>(municipio) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los municipios");
            }
            return resultado;
        }
    }
}
