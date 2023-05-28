using Dominio.Contratos;
using Dominio.ReglaNegocio;
using Infraestructura.Contratos;
using Infraestructura.Repositorios;
using Infraestructura.Transversal;
using Infraestructura.UoW;
using InfraestructuraRepositorios;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.IoC
{
    public class ServicioIoC
    {
        public static IServiceCollection Service(IServiceCollection services)
        {
            //REGISTRAMOS UNIDAD DE TRABAJO Y REPOSITORIOS
            services.AddScoped<IUnidadTrabajo, UnidadTrabajo>();
            services.AddScoped<IArea, AreaRepositorio>();
            services.AddScoped<IDocumento, DocumentoRepositorio>();
            services.AddScoped<IDepartamento, DepartamentoRepositorio>();
            services.AddScoped<IMunicipio, MunicipioRepositorio>();
            services.AddScoped<IBarrio, BarrioRepositorio>();
            services.AddScoped<IRenta, RentaRepositorio>();
            services.AddScoped<IPersonal, PersonalRepositorio>();
            services.AddScoped<IOficina, OficinaRepositorio>();
            services.AddScoped<IObservacion, ObservacionRepositorio>();
            services.AddScoped<IActividad, ActividadRepositorio>();
            services.AddScoped<IDetalleActividad, DetalleActividadRepositorio>();
            services.AddScoped<ICuenta, CuentaRepositorio>();
            services.AddScoped<IMovimiento, MovimientoRepositorio>();
            services.AddScoped<ITipoMovimiento, TipoMovimientoRepositorio>();


            //REGISTRAMOS LOS MODELOS DE NEGOCIO
            services.AddScoped<IUtilitario, Utilitario>();
            services.AddScoped<IDocumentoDominio, DocumentoNegocio>();
            services.AddScoped<IAreaDominio, AreaNegocio>();
            services.AddScoped<IUsuarioNegocio, UsuarioNegocio>();
            services.AddScoped<IDetalleActividadDominio, DetalleActividadNegocio>();
            services.AddScoped<IActividadDominio, ActividadNegocio>();
            services.AddScoped<IRentaDominio, RentaNegocio>();
            services.AddScoped<IOficinaDominio, OficinaNegocio>();
            services.AddScoped<IObservacionDominio, ObservacionNegocio>();
            services.AddScoped<IPersonalDominio, PersonalNegocio>();
            services.AddScoped<IDepartamentoDominio, DepartamentoNegocio>();
            services.AddScoped<IMunicipioDominio, MunicipioNegocio>();
            services.AddScoped<IBarrioDominio, BarrioNegocio>();
            services.AddScoped<ICuentaDominio, CuentaNegocio>();
            services.AddScoped<IMovimientoDominio, MovimientoNegocio>();
            services.AddScoped<ITipoMovimientoDominio, TipoMovimientoNegocio>();

            return services;
        }
    }
}
