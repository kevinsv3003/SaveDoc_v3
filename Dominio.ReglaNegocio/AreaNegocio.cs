using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class AreaNegocio : IAreaDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public AreaNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public async Task<bool> ActualizarArea(AreaDto area)
        {
            try
            {
                var areadb = unidadTrabajo.AreaRepositorio.BuscarPorId(area.AreaId);
                areadb.Nombre = area.Nombre;
                areadb.Descripcion = area.Descripcion;
                //areadb = Mapper.Map<AreaDto, Area>(area);
                areadb.Fondo = (area.FileImage != null) ? General.GetByteArrayFromDoc(area.FileImage) : areadb.Fondo;

                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.AreaRepositorio.Actualizar(areadb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al agregar el área.");
            }
        }
        
        public async Task<bool> AgregarArea(AreaDto areadto)
        {
            try
            {
                var area = Mapper.Map<AreaDto, Area>(areadto);
                area.Fondo = General.GetByteArrayFromDoc(areadto.FileImage);
                area.Estado = true;
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.AreaRepositorio.Insertar(area);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al agregar el área");
            }
        }

        public async Task<bool> EliminarArea(int areaId)
        {
            try
            {
                var area = unidadTrabajo.AreaRepositorio.BuscarPorId(areaId);
                area.Estado = false;
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.AreaRepositorio.Actualizar(area);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al eliminar el área");
            }
        }

        public async Task<AreaDto> ObtenerAreaById(int areaId)
        {
            var resultado = new AreaDto();
            try
            {
                var area = unidadTrabajo.AreaRepositorio.BuscarPorId(areaId);
                resultado = area != null ? Mapper.Map<Area, AreaDto>(area) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las áreas");
            }
            return resultado;
        }

        public async Task<List<AreaDto>> ObtenerListaArea()
        {
            var resultado = new List<AreaDto>();
            try
            {
                var areas = unidadTrabajo.AreaRepositorio.ListarTodos();
                foreach (var item in areas)
                {
                    var it = Mapper.Map<Area, AreaDto>(item);
                    if (it.Estado)
                        resultado.Add(it);
                }
                //resultado = Mapper.Map<List<Area>, List<AreaDto>>(areas);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las áreas");
            }
        }

    }
}
