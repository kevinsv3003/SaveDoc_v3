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
    public class RentaNegocio : IRentaDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        public RentaNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> ActualizarRenta(RentaDto Renta)
        {
            try
            {
                var rentadb = Mapper.Map<RentaDto, Renta>(Renta);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.RentaRepositorio.Actualizar(rentadb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar la renta.");

            }
        }

        public async Task<bool> AgregarRenta(RentaDto Renta)
        {
            try
            {
                var rentadb = Mapper.Map<RentaDto, Renta>(Renta);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.RentaRepositorio.Insertar(rentadb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la renta.");

            }
        }


        public async Task<List<RentaDto>> ObtenerListaRenta()
        {
            var resultado = new List<RentaDto>();
            try
            {
                var rentas = unidadTrabajo.RentaRepositorio.ListarTodos();
                foreach (var item in rentas)
                {
                    var it = Mapper.Map<Renta, RentaDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las rentas");
            }
        }

        public async Task<RentaDto> ObtenerRentaById(int RentaId)
        {
            var resultado = new RentaDto();
            try
            {
                var renta = unidadTrabajo.RentaRepositorio.BuscarPorId(RentaId);
                resultado = renta != null ? Mapper.Map<Renta, RentaDto>(renta) : resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las rentas");
            }
            return resultado;
        }
    }
}
