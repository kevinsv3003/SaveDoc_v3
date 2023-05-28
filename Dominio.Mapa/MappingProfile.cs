using Dominio.EntidadesDto;
using Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Dominio.Mapa
{
    public class MappingProfile :Profile
    {

        public MappingProfile()
        {
            CreateMap<Area, AreaDto>().ReverseMap();
            CreateMap<AreaDto, Area>().ReverseMap();
            
            CreateMap<Documento, DocumentoDto>().ReverseMap();
            CreateMap<DocumentoDto, Documento>().ReverseMap();

            CreateMap<UsuarioApp, UsuarioDto>().ReverseMap();
            CreateMap<UsuarioDto, UsuarioApp>().ReverseMap();

            CreateMap<Renta, RentaDto>().ReverseMap();
            CreateMap<RentaDto, Renta>().ReverseMap();

            CreateMap<DetalleActividad, DetalleActividadDto>().ReverseMap();
            CreateMap<DetalleActividadDto, DetalleActividad>().ReverseMap();

            CreateMap<Actividad, ActividadDto>().ReverseMap();
            CreateMap<ActividadDto, Actividad>().ReverseMap();

            CreateMap<Personal, PersonalDto>().ReverseMap();
            CreateMap<PersonalDto, Personal>().ReverseMap();

            CreateMap<Departamento, DepartamentoDto>().ReverseMap();
            CreateMap<DepartamentoDto, Departamento>().ReverseMap();

            CreateMap<Municipio, MunicipioDto>().ReverseMap();
            CreateMap<MunicipioDto, Municipio>().ReverseMap();

            CreateMap<Barrio, BarrioDto>().ReverseMap();
            CreateMap<BarrioDto, Barrio>().ReverseMap();

            CreateMap<Oficina, OficinaDto>().ReverseMap();
            CreateMap<OficinaDto, Oficina>().ReverseMap();

            CreateMap<Observacion, ObservacionDto>().ReverseMap();
            CreateMap<ObservacionDto, Observacion>().ReverseMap();

            CreateMap<Cuenta, CuentaDto>().ReverseMap();
            CreateMap<CuentaDto, Cuenta>().ReverseMap();

            CreateMap<Movimiento, MovimientoDto>().ReverseMap();
            CreateMap<MovimientoDto, Movimiento>().ReverseMap();

            CreateMap<TipoMovimiento, TipoMovimientoDto>().ReverseMap();
            CreateMap<TipoMovimientoDto, TipoMovimiento>().ReverseMap();

        }
    }
}
