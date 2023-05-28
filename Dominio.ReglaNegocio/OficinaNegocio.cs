using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class OficinaNegocio : IOficinaDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public OficinaNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> ActualizarOficina(OficinaDto Oficina)
        {
            try
            {
                var Oficinadb = Mapper.Map<OficinaDto, Oficina>(Oficina);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.OficinaRepositorio.Actualizar(Oficinadb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar la Oficina.");

            }
        }

        public async Task<bool> AgregarOficina(OficinaDto Oficina)
        {
            try
            {
                var Oficinadb = Mapper.Map<OficinaDto, Oficina>(Oficina);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.OficinaRepositorio.Insertar(Oficinadb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la Oficina.");

            }
        }


        public async Task<List<OficinaDto>> ObtenerListaOficina()
        {
            var resultado = new List<OficinaDto>();
            try
            {
                var Oficinas = unidadTrabajo.OficinaRepositorio.ListarTodos();
                foreach (var item in Oficinas)
                {
                    var it = Mapper.Map<Oficina, OficinaDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las Oficinas");
            }
        }

        public async Task<OficinaDto> ObtenerOficinaById(int OficinaId)
        {
            var resultado = new OficinaDto();
            try
            {
                var Oficina = unidadTrabajo.OficinaRepositorio.BuscarPorId(OficinaId);
                resultado = Oficina != null ? Mapper.Map<Oficina, OficinaDto>(Oficina) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las Oficinas");
            }
            return resultado;
        }
    }
}
