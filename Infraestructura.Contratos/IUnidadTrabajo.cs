using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Contratos
{
    public interface IUnidadTrabajo
    {
        #region Declaraciones de propiedades y metodos de la interfase
        IArea AreaRepositorio { get; }
        IDocumento DocumentoRepositorio { get; }
        IRenta RentaRepositorio { get; }
        IPersonal PersonalRepositorio { get; }
        IActividad ActividadRepositorio { get; }
        IDetalleActividad DetalleActividadRepositorio { get; }
        IObservacion ObservacionRepositorio { get; }
        IOficina OficinaRepositorio { get; }
        IDepartamento DepartamentoRepositorio { get; }
        IMunicipio MunicipioRepositorio { get; }
        IBarrio BarrioRepositorio { get; }
        ICuenta CuentaRepositorio { get; }
        IMovimiento MovimientoRepositorio { get; }
        ITipoMovimiento TipoMovimientoRepositorio { get; }
        void Commit();
        void RollBack();
        void Disposable();

        #endregion
    }
}
