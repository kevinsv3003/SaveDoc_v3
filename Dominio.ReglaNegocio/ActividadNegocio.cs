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
using static Infraestructura.Transversal.Utilitario;

namespace Dominio.ReglaNegocio
{
    public class ActividadNegocio : IActividadDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public ActividadNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> ActualizarActividad(ActividadDto Actividad)
        {
            try
            {
                var Actividaddb = Mapper.Map<ActividadDto, Actividad>(Actividad);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.ActividadRepositorio.Actualizar(Actividaddb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar la Actividad.");

            }
        }

        public async Task<bool> AgregarActividad(ActividadDto Actividad)
        {
            try
            {
                var Actividaddb = Mapper.Map<ActividadDto, Actividad>(Actividad);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.ActividadRepositorio.Insertar(Actividaddb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la Actividad.");

            }
        }


        public async Task<List<ActividadDto>> ObtenerListaActividad(int rentaUsuario)
        {
            var resultado = new List<ActividadDto>();
            try
            {
                var Actividads = unidadTrabajo.ActividadRepositorio.ListarTodos().Where(a => a.RentaId.Equals(rentaUsuario));
                Actividads = Actividads.Skip(Math.Max(0, Actividads.Count() - 30));
                foreach (var item in Actividads)
                {
                    var detalle = unidadTrabajo.DetalleActividadRepositorio.Buscar(a => a.ActividadId.Equals(item.ActividadId));
                    var it = Mapper.Map<Actividad, ActividadDto>(item);
                    it.Participantes = detalle.GroupBy(x => x.PersonalId).Count();
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las Actividads");
            }
        }

        public async Task<ActividadDto> ObtenerActividadById(int ActividadId)
        {
            var resultado = new ActividadDto();
            try
            {
                var Actividad = unidadTrabajo.ActividadRepositorio.BuscarPorId(ActividadId);
                resultado = Actividad != null ? Mapper.Map<Actividad, ActividadDto>(Actividad) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las Actividads");
            }
            return resultado;
        }

        public async Task<List<int>> ObtenerPropuestaPersonal(int cantidad, int renta, string genero, bool EsManagua, bool EsParticipativo)
        {
            var resultado = new List<int>();
            try
            {
                var esManagua = EsManagua ? 10 : 0;
                var esParticipativo = EsParticipativo ? 1 : 0;
                genero = genero == null ? "" : genero;
                resultado = await unidadTrabajo.ActividadRepositorio.ObtenerPropuestaPersonal(cantidad, renta, genero, esManagua, esParticipativo);

            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las Actividads");
            }
            return resultado;
        }


        public List<int> ObtenerPropuestaPersonalActividad(int cantidad, int renta, string genero, bool EsManagua, bool EsParticipativo, List<string> observaciones)
        {
            List<int> propuestas = new List<int>();
            try
            {
                genero = genero == null ? "" : genero;
                var usuariosDisponibles = EsManagua ? (from p in unidadTrabajo.PersonalRepositorio.ListarTodos()
                                                       join o in unidadTrabajo.OficinaRepositorio.ListarTodos()
                                                       on p.OficinaId equals o.OficinaId
                                                       where
                                                       p.RentaId.Equals(renta) &&
                                                       p.Genero.Contains(genero) &&
                                                       p.CodMunicipio.Equals(10) &&
                                                       !(p.OficinaId.Equals((int)OFICINA.OSIN) && !p.Disponible)
                                                       select p
                                           ).ToList()
                                                   : (from p in unidadTrabajo.PersonalRepositorio.ListarTodos()
                                                      join o in unidadTrabajo.OficinaRepositorio.ListarTodos()
                                                      on p.OficinaId equals o.OficinaId
                                                      where
                                                      p.RentaId.Equals(renta) &&
                                                      p.Genero.Contains(genero) &&
                                                      !(p.OficinaId.Equals((int)OFICINA.OSIN) && !p.Disponible)
                                                      select p).ToList();
                var personalFiltrado = new List<Personal>();
                usuariosDisponibles.ForEach(x =>
                {
                    var observacionesPersona = unidadTrabajo.ObservacionRepositorio.Buscar(o => o.PersonalId.Equals(x.PersonalId));
                    var tieneObservacion = observacionesPersona.ToList().Exists(o => observaciones.Contains(o.ObservacionDet));
                    if (!tieneObservacion)
                        personalFiltrado.Add(x);
                });
                var detalleUsuario = unidadTrabajo.DetalleActividadRepositorio.Buscar(x => x.Asistio)
                                    .GroupBy(x => x.PersonalId)
                                    .Select(x => new DetalleActividadDto()
                                    {
                                        PersonalId = x.Key,
                                        cantidadActividad = x.Count()
                                    }).OrderBy(p => p.PersonalId).ToList();

                var propuestaPersonal = (from p in personalFiltrado
                                         join d in detalleUsuario
                                         on p.PersonalId equals d.PersonalId into personal
                                         from per in personal.DefaultIfEmpty()
                                         select new DetalleActividadDto
                                         {
                                             PersonalId = p.PersonalId,
                                             cantidadActividad = per != null ? per.cantidadActividad : 0
                                         }).ToList();


                propuestaPersonal = EsParticipativo ? propuestaPersonal.OrderByDescending(x => x.cantidadActividad).ThenBy(x => Guid.NewGuid()).Take(cantidad).ToList()
                                             : propuestaPersonal.OrderBy(x => x.cantidadActividad).ThenBy(x => Guid.NewGuid()).Take(cantidad).ToList();
                propuestaPersonal.ForEach(p =>
                {
                    propuestas.Add(p.PersonalId);
                });
            }
            catch (Exception ex)
            {

                throw;
            }

            return propuestas;
        }
    }
}
