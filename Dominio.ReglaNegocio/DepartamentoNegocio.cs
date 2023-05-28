using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class DepartamentoNegocio : IDepartamentoDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public DepartamentoNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> ActualizarDepartamento(DepartamentoDto Departamento)
        {
            try
            {
                var departamentodb = Mapper.Map<DepartamentoDto, Departamento>(Departamento);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.DepartamentoRepositorio.Actualizar(departamentodb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar el departamento.");

            }
        }

        public async Task<bool> AgregarDepartamento(DepartamentoDto Departamento)
        {
            try
            {
                var departamentodb = Mapper.Map<DepartamentoDto, Departamento>(Departamento);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.DepartamentoRepositorio.Insertar(departamentodb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al insertar el departamento.");

            }
        }

        public async Task<DepartamentoDto> ObtenerDepartamentoById(int DepartamentoId)
        {
            var resultado = new DepartamentoDto();
            try
            {
                var departamento = unidadTrabajo.DepartamentoRepositorio.BuscarPorId(DepartamentoId);
                resultado = departamento != null ? Mapper.Map<Departamento, DepartamentoDto>(departamento) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los departamentos");
            }
            return resultado;
        }

        public async Task<List<DepartamentoDto>> ObtenerListaDepartamento()
        {

            var resultado = new List<DepartamentoDto>();
            try
            {
                var departamentos = unidadTrabajo.DepartamentoRepositorio.ListarTodos();
                foreach (var item in departamentos)
                {
                    var it = Mapper.Map<Departamento, DepartamentoDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar los departamentos");
            }
        }
    }
}
