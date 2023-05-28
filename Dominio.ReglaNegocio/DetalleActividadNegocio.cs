using AutoMapper;
using ClosedXML.Excel;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class DetalleActividadNegocio : IDetalleActividadDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly IActividadDominio _actividadDominio;
        private readonly IPersonalDominio _personalDominio;
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

        public DetalleActividadNegocio(IUnidadTrabajo unidadTrabajo, IActividadDominio _actividadDominio, IPersonalDominio _personalDominio)
        {
            this.unidadTrabajo = unidadTrabajo;
            this._actividadDominio = _actividadDominio;
            this._personalDominio = _personalDominio;
        }
        public async Task<bool> ActualizarDetalleActividad(DetalleActividadDto DetalleActividad)
        {
            try
            {
                var DetalleActividaddb = Mapper.Map<DetalleActividadDto, DetalleActividad>(DetalleActividad);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.DetalleActividadRepositorio.Actualizar(DetalleActividaddb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar la DetalleActividad.");

            }
        }

        public async Task<bool> AgregarDetalleActividad(DetalleActividadDto DetalleActividad)
        {
            try
            {
                var DetalleActividaddb = Mapper.Map<DetalleActividadDto, DetalleActividad>(DetalleActividad);

                unidadTrabajo.DetalleActividadRepositorio.Insertar(DetalleActividaddb);
                unidadTrabajo.Commit();


                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al agregar la DetalleActividad.");

            }
        }


        public async Task<List<DetalleActividadDto>> ObtenerListaDetalleActividad()
        {
            var resultado = new List<DetalleActividadDto>();
            try
            {
                var DetalleActividads = unidadTrabajo.DetalleActividadRepositorio.ListarTodos();
                foreach (var item in DetalleActividads)
                {
                    var it = Mapper.Map<DetalleActividad, DetalleActividadDto>(item);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las DetalleActividads");
            }
        }

        public async Task<DetalleActividadDto> ObtenerDetalleActividadById(int DetalleActividadId)
        {
            var resultado = new DetalleActividadDto();
            try
            {
                var DetalleActividad = unidadTrabajo.DetalleActividadRepositorio.BuscarPorId(DetalleActividadId);
                DetalleActividad.Observacion = DetalleActividad.Observacion;
                resultado = DetalleActividad != null ? Mapper.Map<DetalleActividad, DetalleActividadDto>(DetalleActividad) : resultado;
                resultado.Personal = await _personalDominio.ObtenerPersonalById(resultado.PersonalId);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar las DetalleActividads");
            }
            return resultado;
        }

        public async Task<List<DetalleActividadDto>> ObtenerListaDetalleActividadByPersonalId(int PersonalId)
        {
            var resultado = new List<DetalleActividadDto>();
            try
            {
                var DetalleActividads = unidadTrabajo.DetalleActividadRepositorio.Buscar(x => x.PersonalId.Equals(PersonalId));
                foreach (var item in DetalleActividads)
                {
                    var it = Mapper.Map<DetalleActividad, DetalleActividadDto>(item);
                    it.Personal = await _personalDominio.ObtenerPersonalById(PersonalId);
                    it.Actividad = await _actividadDominio.ObtenerActividadById(it.ActividadId);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las DetalleActividads");
            }
        }

        public async Task<List<DetalleActividadDto>> ObtenerListaDetalleActividadByActividadlId(int ActividadId)
        {
            var resultado = new List<DetalleActividadDto>();
            try
            {
                var DetalleActividads = unidadTrabajo.DetalleActividadRepositorio.Buscar(x => x.ActividadId.Equals(ActividadId));
                foreach (var item in DetalleActividads)
                {
                    var it = Mapper.Map<DetalleActividad, DetalleActividadDto>(item);
                    it.Personal = await _personalDominio.ObtenerPersonalById(it.PersonalId);
                    it.Actividad = await _actividadDominio.ObtenerActividadById(it.ActividadId);
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar las DetalleActividads");
            }
        }

        public async Task<bool> BorrarDetalleByActividadId(int ActividadId)
        {
            try
            {
                var detalle = await ObtenerListaDetalleActividadByActividadlId(ActividadId);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    detalle.ForEach(x =>
                    {
                        unidadTrabajo.DetalleActividadRepositorio.Eliminar(x.DetalleActividadId);
                    });
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la DetalleActividad.");

            }
        }

        public async Task<byte[]> GenerarReporteParticipante(int ActividadId)
        {
            var retorno = new byte[0];

            var detalleActividad = await ObtenerListaDetalleActividadByActividadlId(ActividadId);
            detalleActividad = detalleActividad.OrderBy(x => x.Personal.OficinaId).ThenByDescending(x => x.Transporte).ToList();
            var actividad = detalleActividad.FirstOrDefault().Actividad;

            var EncabezadoTablaActividad = "B2:C2";
            var EncabezadoTablaParticipante = "B8:J8";
            var registrosTotalesParticipantes = detalleActividad.Count();
            var CabeceraTablaActividad = "B3:B6";
            var CabeceraTablaParticipante = "B9:J9";
            var ContenidoTablaActividad = "C3:C6";
            var ContenidoTablaParticipante = "B10:J" + (9 + registrosTotalesParticipantes).ToString();
            var TablaCompletaActividad = "B2:C6";
            var TablaCompletaParticipante = "B8:J" + (9 + registrosTotalesParticipantes).ToString();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Actividad");

                //ENCABEZADO TABLA ACTIVDAD
                worksheet.Range(EncabezadoTablaActividad).Merge().SetValue("DATOS ACTIVIDAD").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaActividad).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("B3").Value = "Actividad:";
                worksheet.Cell("B4").Value = "Lugar:";
                worksheet.Cell("B5").Value = "Fecha:";
                worksheet.Cell("B6").Value = "Descripción:";
                worksheet.Range("C3").SetValue(actividad.NombreActividad);
                worksheet.Range("C4").SetValue(actividad.Lugar);
                worksheet.Range("C5").SetValue(actividad.Fecha);
                worksheet.Range("C6").SetValue(actividad.Descripcion);


                //ENCABEZADO TABLA ACTIVDAD
                worksheet.Range(EncabezadoTablaParticipante).Merge().SetValue("DATOS DE PARTICIPANTES").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaParticipante).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaParticipante).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaParticipante).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("B9").Value = "N°";
                worksheet.Cell("C9").Value = "Nombre Completo";
                worksheet.Cell("D9").Value = "Cédula";
                worksheet.Cell("E9").Value = "Teléfono";
                worksheet.Cell("F9").Value = "Oficina";
                worksheet.Cell("G9").Value = "Req. Transporte";
                worksheet.Cell("H9").Value = "Municipio";
                worksheet.Cell("I9").Value = "Barrio";
                worksheet.Cell("J9").Value = "Dirección";

                var currentRow = 10;
                foreach (var det in detalleActividad)
                {
                    var barrio = det.Personal.BarrioDes != null && !det.Personal.BarrioDes.Equals("") ? !(det.Personal.BarrioDes.Contains("Comarca") || det.Personal.BarrioDes.Contains("Carretera")) ? "B° " + det.Personal.BarrioDes : det.Personal.BarrioDes : "";
                    worksheet.Cell("B" + currentRow).Value = currentRow - 9;
                    worksheet.Cell("C" + currentRow).Value = det.Personal.PrimerNombre + " " + det.Personal.SegundoNombre + " " + det.Personal.PrimerApellido + " " + det.Personal.SegundoApellido;
                    worksheet.Cell("D" + currentRow).Value = det.Personal.Cedula;
                    worksheet.Cell("E" + currentRow).Value = det.Personal.Telefono;
                    worksheet.Cell("F" + currentRow).Value = det.Personal.Oficina.Nombre;
                    worksheet.Cell("G" + currentRow).Value = det.Transporte ? "Sí" : "No";
                    worksheet.Cell("H" + currentRow).Value = det.Personal.MunicipioDes;
                    worksheet.Cell("I" + currentRow).Value = barrio;
                    worksheet.Cell("J" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(det.Personal.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(det.Personal.Direccion.ToLower());
                    currentRow++;

                }

                worksheet.Columns().AdjustToContents();
                worksheet.Columns("B").Width = 12;
                worksheet.Columns("C").Width = 50;
                worksheet.Columns("D").Width = 17;
                worksheet.Columns("E").Width = 17;
                worksheet.Columns("F").Width = 17;
                worksheet.Columns("G").Width = 17;
                worksheet.Columns("H").Width = 18;
                worksheet.Columns("I").Width = 22;
                worksheet.Columns("J").Width = 80;
                worksheet.Range(CabeceraTablaParticipante).SetAutoFilter();


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    retorno = stream.ToArray();
                }
            }

            return retorno;
        }

        public async Task<byte[]> GenerarReporteDetallePersonal(int personalId)
        {
            var retorno = new byte[0];
            var personal = personalId > 0 ? await _personalDominio.ObtenerPersonalById(personalId) : new PersonalDto();
            //personal.Direccion = personal.BarrioDes != null && !personal.BarrioDes.Equals("") ? !(personal.BarrioDes.Contains("Comarca") || personal.BarrioDes.Contains("Carretera")) ? "B° " + personal.BarrioDes + ", " + personal.Direccion : personal.BarrioDes + ", " + personal.Direccion : personal.Direccion;
            var nombreCompletoPersona = personal.PrimerNombre + " " + personal.SegundoNombre + " " + personal.PrimerApellido + " " + personal.SegundoApellido;
            var observaciones = personal.Observaciones.OrderByDescending(x => x.ObservacionId).ToList();
            if(observaciones.Count == 0)
            {
                observaciones.Add(new ObservacionDto() { ObservacionDet="No tiene ninguna observación"});
            }
            var actividades = await ObtenerListaDetalleActividadByPersonalId(personalId);
            var actividadesEsteAño = actividades.Where(x => x.Actividad.Fecha.Value.Year.Equals(DateTime.Now.Year)).ToList();
            var actividadesAsistio = actividades.Where(x => x.Asistio).Count();
            var porcAsistencia = actividades.Count() != 0 ? (Convert.ToDouble(actividadesAsistio) / Convert.ToDouble(actividades.Count())) * 100 : 0;

            var EncabezadoTablaActividad = "I2:N2";
            var EncabezadoTablaParticipante = "B2:G2";
            var EncabezadoTablaDetalle = "B8:G8";

            var CabeceraTablaActividad = "I3:N3";
            var CabeceraTablaParticipantePrincipal = "B3:B6";
            var CabeceraTablaParticipanteSecund = "E4:E5";
            var CabeceraTablaDetalle = "B9:B10";

            var ContenidoTablaActividad = "I4:N" + (3 + actividades.Count).ToString();
            var ContenidoTablaParticipantePrincipal = "C3:C6";
            var ContenidoTablaParticipanteSecund = "F4:F5";
            var ContenidoTablaDetalle = "B11:B" + (10 + observaciones.Count).ToString();
            var columnaFechaActividad = "K4:K" + (3 + actividades.Count).ToString();

            var TablaCompletaActividad = "I2:N" + (3 + actividades.Count).ToString();
            var TablaCompletaParticipante = "B2:G6";
            var TablaCompletaDetalle = "B8:G" + (10 + observaciones.Count).ToString();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Detalle Personal");

                #region TABLA PARTICIPANTE
                //ENCABEZADO TABLA PARTICIPANTE
                worksheet.Range(EncabezadoTablaParticipante).Merge().SetValue("DATOS GENERALES").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaParticipantePrincipal).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                worksheet.Range(CabeceraTablaParticipanteSecund).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaParticipantePrincipal).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                worksheet.Range(ContenidoTablaParticipanteSecund).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaParticipante).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("B3").Value = "Nombre:";
                worksheet.Cell("B4").Value = "Cédula:";
                worksheet.Cell("B5").Value = "Municipio:";
                worksheet.Cell("B6").Value = "Dirección:";
                worksheet.Cell("E4").Value = "Teléfono:";
                worksheet.Cell("E5").Value = "Barrio:";

                worksheet.Range("C3:G3").Merge().SetValue(nombreCompletoPersona).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range("C4:D4").Merge().SetValue(personal.Cedula).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range("C5:D5").Merge().SetValue(personal.MunicipioDes).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range("C6:G6").Merge().SetValue(personal.Direccion).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range("F4:G4").Merge().SetValue(personal.Telefono).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Range("F5:G5").Merge().SetValue(personal.BarrioDes).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                #endregion

                #region TABLA DETALLE

                //ENCABEZADO TABLA DETALLE
                worksheet.Range(EncabezadoTablaDetalle).Merge().SetValue("DETALLES").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaDetalle).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaDetalle).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaDetalle).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                //DATOS DE TABLA DETALLE
                worksheet.Cell("B9").Value = "Participación";
                worksheet.Range("C9:G9").Merge().SetValue(porcAsistencia + "% de participación en actividades.");
                worksheet.Range("B10:G10").Merge().SetValue("Observaciones");

                var filaDetalle = 11;
                foreach (var item in observaciones)
                {
                    worksheet.Range("B" + filaDetalle + ":G" + filaDetalle).Merge().SetValue(item.ObservacionDet).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    filaDetalle++;
                }

                #endregion

                #region TABLA ACTIVIDADES

                //ENCABEZADO TABLA ACTIVDAD
                worksheet.Range(EncabezadoTablaActividad).Merge().SetValue("DETALLE DE ACTIVIDADES").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                worksheet.Range(columnaFechaActividad).SetDataType(XLDataType.DateTime).Style.DateFormat.SetFormat("dd/mm/yyyy hh:mm AM/PM");
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaActividad).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("I3").Value = "N°";
                worksheet.Cell("J3").Value = "Actividad";
                worksheet.Cell("K3").Value = "Fecha";
                worksheet.Cell("L3").Value = "Tipo";
                worksheet.Cell("M3").Value = "Asistió?";
                worksheet.Cell("N3").Value = "Observaciones";


                var currentRow = 4;
                foreach (var det in actividades)
                {
                    worksheet.Cell("I" + currentRow).Value = currentRow - 3;
                    worksheet.Cell("J" + currentRow).Value = det.Actividad.NombreActividad;
                    worksheet.Cell("K" + currentRow).Value = det.Actividad.Fecha.Value.ToString("dd/MM/yyyy hh:mm tt");
                    worksheet.Cell("L" + currentRow).Value = det.Actividad.EsEspecial ? "Especial" : "Común";
                    worksheet.Cell("M" + currentRow).Value = det.Asistio ? "Sí" : "No";
                    worksheet.Cell("N" + currentRow).Value = det.Observacion;
                    currentRow++;

                }

                #endregion

                worksheet.Columns().AdjustToContents();    
                worksheet.Columns("B:G").Width = 13;
                worksheet.Columns("I").Width = 6;
                worksheet.Columns("J").Width = 36;
                worksheet.Columns("K").Width = 20;
                worksheet.Columns("L").Width = 10;
                worksheet.Columns("M").Width = 10;
                worksheet.Columns("N").Width = 70;
                worksheet.Range(CabeceraTablaActividad).SetAutoFilter();            

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    retorno = stream.ToArray();
                }
            }

            return retorno;

        }

        public async Task<byte[]> GenerarReporteActividad(int ActividadId)
        {
            var retorno = new byte[0];

            var detalleActividad = await ObtenerListaDetalleActividadByActividadlId(ActividadId);
            detalleActividad = detalleActividad.OrderBy(x => x.Asistio).ThenByDescending(x => x.Personal.OficinaId).ThenByDescending(x => x.Transporte).ToList();
            var actividad = detalleActividad.FirstOrDefault().Actividad;
            
            var EncabezadoTablaActividad = "B2:C2";
            var EncabezadoTablaParticipante = "B8:H8";
            var registrosTotalesParticipantes = detalleActividad.Count();
            var CabeceraTablaActividad = "B3:B6";
            var CabeceraTablaParticipante = "B9:H9";
            var ContenidoTablaActividad = "C3:C6";
            var ContenidoTablaParticipante = "B10:H" + (9 + registrosTotalesParticipantes).ToString();
            var TablaCompletaActividad = "B2:C6";
            var TablaCompletaParticipante = "B8:H" + (9 + registrosTotalesParticipantes).ToString();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Actividad");

                //ENCABEZADO TABLA ACTIVDAD
                worksheet.Range(EncabezadoTablaActividad).Merge().SetValue("DATOS ACTIVIDAD").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaActividad).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaActividad).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("B3").Value = "Actividad:";
                worksheet.Cell("B4").Value = "Lugar:";
                worksheet.Cell("B5").Value = "Fecha:";
                worksheet.Cell("B6").Value = "Descripción:";
                worksheet.Range("C3").SetValue(actividad.NombreActividad);
                worksheet.Range("C4").SetValue(actividad.Lugar);
                worksheet.Range("C5").SetValue(actividad.Fecha);
                worksheet.Range("C6").SetValue(actividad.Descripcion);


                //ENCABEZADO TABLA ACTIVDAD
                worksheet.Range(EncabezadoTablaParticipante).Merge().SetValue("DATOS DE PARTICIPANTES").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                    .Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222));
                //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                worksheet.Range(CabeceraTablaParticipante).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Font.SetBold()
                    .Font.SetFontColor(XLColor.White)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));
                //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                worksheet.Range(ContenidoTablaParticipante).Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                      .Alignment.SetWrapText(true)
                      .Font.SetBold(false)
                      .Font.SetFontColor(XLColor.Black)
                      .Fill.SetBackgroundColor(XLColor.White);
                //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                worksheet.Range(TablaCompletaParticipante).Style
                    .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                worksheet.Cell("B9").Value = "N°";
                worksheet.Cell("C9").Value = "Nombre Completo";
                worksheet.Cell("D9").Value = "Oficina";
                worksheet.Cell("E9").Value = "Asistió";
                worksheet.Cell("F9").Value = "Req. Transporte";
                worksheet.Cell("G9").Value = "Dirección";
                worksheet.Cell("H9").Value = "Observación";

                var currentRow = 10;
                foreach (var det in detalleActividad)
                {
                    var barrio = det.Personal.BarrioDes != null && !det.Personal.BarrioDes.Equals("") ? !(det.Personal.BarrioDes.Contains("Comarca") || det.Personal.BarrioDes.Contains("Carretera")) ? "B° " + det.Personal.BarrioDes : det.Personal.BarrioDes : "";
                    worksheet.Cell("B" + currentRow).Value = currentRow - 9;
                    worksheet.Cell("C" + currentRow).Value = det.Personal.PrimerNombre + " " + det.Personal.SegundoNombre + " " + det.Personal.PrimerApellido + " " + det.Personal.SegundoApellido;
                    worksheet.Cell("D" + currentRow).Value = det.Personal.Oficina.Nombre;
                    worksheet.Cell("E" + currentRow).Value = det.Asistio ? "Sí" : "No";
                    worksheet.Cell("F" + currentRow).Value = det.Transporte ? "Sí" : "No";
                    worksheet.Cell("G" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(det.Personal.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(det.Personal.Direccion.ToLower());
                    worksheet.Cell("H" + currentRow).Value = det.Justificado ? det.Observacion + " - Justificado" : det.Observacion;
                    if (det.Observacion != null && det.Observacion.Length > 0) { worksheet.Range("B" + currentRow + ":H" + currentRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(249, 176, 153)); }
                    if (det.Observacion != null && det.Observacion.Length > 0 && det.Justificado) { worksheet.Range("B" + currentRow + ":H" + currentRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(235, 241, 222)); }
                    currentRow++;

                }

                worksheet.Columns().AdjustToContents();
                worksheet.Columns("B").Width = 12;
                worksheet.Columns("C").Width = 50;
                worksheet.Columns("D").Width = 17;
                worksheet.Columns("E").Width = 16;
                worksheet.Columns("F").Width = 16;
                worksheet.Columns("G").Width = 80;
                worksheet.Columns("H").Width = 50;
                worksheet.Range(CabeceraTablaParticipante).SetAutoFilter();


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    retorno = stream.ToArray();
                }
            }

            return retorno;
        }

        public async Task<bool> BorrarDetalleByActividadIdYPersonalId(int ActividadId, int PersonalId)
        {
            try
            {
                var listaDetalle = await ObtenerListaDetalleActividadByActividadlId(ActividadId);
                var detalle = listaDetalle.Where(d => d.PersonalId.Equals(PersonalId)).FirstOrDefault();

                unidadTrabajo.DetalleActividadRepositorio.Eliminar(detalle.DetalleActividadId);
                return true;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al agregar la DetalleActividad.");

            }
        }
    }
}
