using Infraestructura.AccesoDatos;
using Infraestructura.Contratos;
using Infraestructura.Repositorios;
using InfraestructuraRepositorios;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.UoW
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly DocumentoContext documentoContext;
        private readonly IConfiguration _configuration;
        public UnidadTrabajo(DocumentoContext documentoContext, IConfiguration _configuration)
        {
            this.documentoContext = documentoContext;
            this._configuration = _configuration;
        }

        ICuenta IUnidadTrabajo.CuentaRepositorio => new CuentaRepositorio(documentoContext);
        IMovimiento IUnidadTrabajo.MovimientoRepositorio => new MovimientoRepositorio(documentoContext);
        ITipoMovimiento IUnidadTrabajo.TipoMovimientoRepositorio => new TipoMovimientoRepositorio(documentoContext);
        IRenta IUnidadTrabajo.RentaRepositorio => new RentaRepositorio(documentoContext);
        IDepartamento IUnidadTrabajo.DepartamentoRepositorio => new DepartamentoRepositorio(documentoContext);
        IMunicipio IUnidadTrabajo.MunicipioRepositorio => new MunicipioRepositorio(documentoContext);
        IBarrio IUnidadTrabajo.BarrioRepositorio => new BarrioRepositorio(documentoContext);
        IPersonal IUnidadTrabajo.PersonalRepositorio => new PersonalRepositorio(documentoContext, _configuration);
        IObservacion IUnidadTrabajo.ObservacionRepositorio => new ObservacionRepositorio(documentoContext);
        IDetalleActividad IUnidadTrabajo.DetalleActividadRepositorio => new DetalleActividadRepositorio(documentoContext);
        IActividad IUnidadTrabajo.ActividadRepositorio => new ActividadRepositorio(documentoContext, _configuration);
        IOficina IUnidadTrabajo.OficinaRepositorio => new OficinaRepositorio(documentoContext);
        IArea IUnidadTrabajo.AreaRepositorio => new AreaRepositorio(documentoContext);
        IDocumento IUnidadTrabajo.DocumentoRepositorio => new DocumentoRepositorio(documentoContext, _configuration);


        public void Commit()
        {
            documentoContext.SaveChanges();
            if (documentoContext.Database.CurrentTransaction != null)
            {
                documentoContext.Database.CurrentTransaction.Commit();
            }
        }

        public void Disposable()
        {
            GC.KeepAlive(this);
            documentoContext.Dispose();
        }

        public void RollBack()
        {
            documentoContext.Database.CurrentTransaction.Rollback();
        }
    }
}
