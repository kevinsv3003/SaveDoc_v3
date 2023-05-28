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
    public class ObservacionNegocio : IObservacionDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public ObservacionNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> ActualizarObservacion(ObservacionDto Observacion)
        {
            try
            {
                var Observaciondb = Mapper.Map<ObservacionDto, Observacion>(Observacion);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.ObservacionRepositorio.Actualizar(Observaciondb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar la Observacion.");

            }
        }

        public async Task<bool> AgregarObservacion(ObservacionDto Observacion)
        {
            try
            {
                var Observaciondb = Mapper.Map<ObservacionDto, Observacion>(Observacion);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.ObservacionRepositorio.Insertar(Observaciondb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la Observacion.");

            }
        }

        public async Task<bool> EliminarObservacion(int ObservacionId)
        {
            try
            {
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.ObservacionRepositorio.Eliminar(ObservacionId);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la Observacion.");

            }
        }

        public async Task<List<string>> ObtenerListaObservacion()
        {
            var resultado = new List<string>();
            try
            {
                var Observacions = unidadTrabajo.ObservacionRepositorio.ListarTodos();
                foreach (var item in Observacions)
                {
                    var existe = resultado.Contains(item.ObservacionDet.Trim().ToString());
                    if (!existe)
                        resultado.Add(item.ObservacionDet);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las Observacions");
            }
        }

        public async Task<List<ObservacionDto>> ObtenerListaObservacionByPersonal(int PersonalId)
        {
            var resultado = new List<ObservacionDto>();
            try
            {
                var Observacions = unidadTrabajo.ObservacionRepositorio.Buscar(x => x.PersonalId.Equals(PersonalId));
                foreach (var item in Observacions)
                {
                    var it = Mapper.Map<Observacion, ObservacionDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las Observacions");
            }
        }

        public async Task<ObservacionDto> ObtenerObservacionById(int ObservacionId)
        {
            var resultado = new ObservacionDto();
            try
            {
                var Observacion = unidadTrabajo.ObservacionRepositorio.BuscarPorId(ObservacionId);
                resultado = Observacion != null ? Mapper.Map<Observacion, ObservacionDto>(Observacion) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las Observacions");
            }
            return resultado;
        }
    }
}
