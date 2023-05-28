using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Infraestructura.Contratos;
using System;
using System.Collections.Generic;
using System.Text;
using Entidades.Entidades;
using System.Threading.Tasks;
using static Infraestructura.Transversal.Utilitario;
using System.Linq;
using System.IO;
using ClosedXML.Excel;
using System.Globalization;
using System.Data;

namespace Dominio.ReglaNegocio
{
    public class PersonalNegocio : IPersonalDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly IUtilitario _utilitario;
        private readonly IMunicipioDominio _municipioDominio;
        private readonly IBarrioDominio _barrioDominio;
        private readonly IDepartamentoDominio _departamentoDominio;

        private readonly IRentaDominio _rentaDominio;
        private readonly IOficinaDominio _OficinaDominio;
        private readonly IObservacionDominio _ObservacionDominio;
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public PersonalNegocio(IUnidadTrabajo unidadTrabajo, IBarrioDominio _barrioDominio, IMunicipioDominio _municipioDominio, IDepartamentoDominio _departamentoDominio, IRentaDominio _rentaDominio,
            IOficinaDominio _OficinaDominio, IObservacionDominio _ObservacionDominio, IUtilitario _utilitario)
        {
            this.unidadTrabajo = unidadTrabajo;
            this._municipioDominio = _municipioDominio;
            this._barrioDominio = _barrioDominio;
            this._departamentoDominio = _departamentoDominio;
            this._rentaDominio = _rentaDominio;
            this._OficinaDominio = _OficinaDominio;
            this._ObservacionDominio = _ObservacionDominio;
            this._utilitario = _utilitario;
        }
        public async Task<bool> ActualizarPersonal(PersonalDto Personal)
        {
            try
            {

                //var personaldb = unidadTrabajo.PersonalRepositorio.BuscarPorId(Personal.PersonalId); 
                var personaldb = Mapper.Map<PersonalDto, Personal>(Personal);

                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.PersonalRepositorio.Actualizar(personaldb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar el personal.");

            }
        }

        public async Task<bool> AgregarPersonal(PersonalDto Personal)
        {
            try
            {
                var personaldb = Mapper.Map<PersonalDto, Personal>(Personal);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.PersonalRepositorio.Insertar(personaldb);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al actualizar el personal.");

            }
        }

        public async Task<byte[]> GenerarReporteDelPersonal(int tipoReporte, string genero, bool EsManagua, List<string> LstObservaciones, List<string> LstRestricciones)
        {
            var retorno = new byte[0];
            var personal = new List<PersonalDto>();
            var personal_estudiando = new List<PersonalDto>();
            var personal_TrabajaIR = new List<PersonalDto>();

            var CeldaInicial = 2;
            var EncabezadoTabla = "";
            var registrosTotales = 0;
            var CabeceraTabla = "";
            var ContenidoTabla = "";
            var TablaCompleta = "";


            var Oficina = "";

            genero = genero == "Ambos" ? "" : genero;
            switch ((TIPO_REPORTE)tipoReporte)
            {
                case TIPO_REPORTE.PERSONAL_DISPONIBLE:
                    #region REPORTE DE TODO EL PERSONAL DISPONIBLE DE LA DIS

                    var juventud_disponible = await unidadTrabajo.PersonalRepositorio.ObtenerReportePersonal((int)TIPO_REPORTE.JUVENTUD_DISPONIBLE);
                    var cls_disponible = await unidadTrabajo.PersonalRepositorio.ObtenerReportePersonal((int)TIPO_REPORTE.CLS_DISPONIBLE);
                    var pesonalDisponible = juventud_disponible.Concat(cls_disponible).ToList();
                    var juventud_No_Disponible = await unidadTrabajo.PersonalRepositorio.ObtenerReportePersonal((int)TIPO_REPORTE.JUVENTUD_NO_DISPONIBLE);
                    var cls_No_Disponible = await unidadTrabajo.PersonalRepositorio.ObtenerReportePersonal((int)TIPO_REPORTE.CLS_NO_DISPONIBLE);

                    var JS_Disponible = EsManagua
                                   ? ObtenerListaPersonal().Result.Where(p => juventud_disponible.Contains(p.PersonalId) && p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).ToList()
                                   : ObtenerListaPersonal().Result.Where(p => juventud_disponible.Contains(p.PersonalId) && p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).ToList();
                    var JS_No_Disponible = EsManagua
                                 ? ObtenerListaPersonal().Result.Where(p => juventud_No_Disponible.Contains(p.PersonalId) && p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).ToList()
                                 : ObtenerListaPersonal().Result.Where(p => juventud_No_Disponible.Contains(p.PersonalId) && p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).ToList();
                    JS_No_Disponible.ForEach(x =>
                    {
                        var observacion = _ObservacionDominio.ObtenerListaObservacionByPersonal(x.PersonalId).Result.Where(o => o.ObservacionDet.Contains("Estudia") || o.ObservacionDet.Contains("Problemas físicos")).ToList();
                        x.Observaciones = observacion;
                    });


                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Reporte Juventud DIS");


                        worksheet.Columns("B").Width = 8;
                        worksheet.Columns("C").Width = 17;
                        worksheet.Columns("D").Width = 45;
                        worksheet.Columns("E").Width = 16;
                        worksheet.Columns("F").Width = 17;
                        worksheet.Columns("G").Width = 15;
                        worksheet.Columns("H").Width = 22;
                        worksheet.Columns("I").Width = 80;
                        worksheet.Columns("J").Width = 40;
                        //worksheet.Columns().AdjustToContents();

                        #region Jovenes Disponibles

                        EncabezadoTabla = "B2:I2";
                        registrosTotales = JS_Disponible.Count();
                        CabeceraTabla = "B3:I3";
                        ContenidoTabla = "B4:I" + (3 + registrosTotales).ToString();
                        TablaCompleta = "B2:I" + (3 + registrosTotales).ToString();
                        worksheet.Range(EncabezadoTabla).Merge().SetValue("JOVENES DISPOBLE PARA ACTIVIDADES").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "No.";
                        worksheet.Cell("C3").Value = "Oficina";
                        worksheet.Cell("D3").Value = "Nombre Completo";
                        worksheet.Cell("E3").Value = "Cédula";
                        worksheet.Cell("F3").Value = "Teléfono";
                        worksheet.Cell("G3").Value = "Municipio";
                        worksheet.Cell("H3").Value = "Barrio";
                        worksheet.Cell("I3").Value = "Dirección";
                        worksheet.Range(CabeceraTabla).SetAutoFilter();

                        var currentRow = 4;
                        var numeracionJsDisponible = 1;
                        foreach (var user in JS_Disponible)
                        {
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";
                            worksheet.Cell("B" + currentRow).Value = numeracionJsDisponible;
                            worksheet.Cell("C" + currentRow).Value = user.Oficina.Nombre;
                            worksheet.Cell("D" + currentRow).Value = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                            worksheet.Cell("E" + currentRow).Value = user.Cedula;
                            worksheet.Cell("F" + currentRow).Value = user.Telefono;
                            worksheet.Cell("G" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                            worksheet.Cell("H" + currentRow).Value = barrio;
                            worksheet.Cell("I" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());
                            currentRow++;
                            numeracionJsDisponible++;
                        }

                        #endregion

                        #region Jovenes No Disponibles

                        CeldaInicial = CeldaInicial + registrosTotales + 4;
                        EncabezadoTabla = "B" + CeldaInicial + ":J" + CeldaInicial;
                        CabeceraTabla = "B" + (CeldaInicial + 1) + ":J" + (CeldaInicial + 1);
                        registrosTotales = JS_No_Disponible.Sum(x => x.Observaciones.Count());
                        TablaCompleta = "B" + CeldaInicial + ":J" + ((1 + registrosTotales) + CeldaInicial).ToString();
                        ContenidoTabla = "B" + (CeldaInicial + 2) + ":J" + ((1 + registrosTotales) + CeldaInicial).ToString();

                        worksheet.Range(EncabezadoTabla).Merge().SetValue("JOVENES NO DISPOBLE PARA ACTIVIDADES DE CAMINATAS (SABADOS POR LA TARDE)").Style
                           .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                           .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                           .Font.SetBold()
                           .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                           .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);


                        worksheet.Cell("B" + (CeldaInicial + 1)).Value = "No.";
                        worksheet.Cell("C" + (CeldaInicial + 1)).Value = "Oficina";
                        worksheet.Cell("D" + (CeldaInicial + 1)).Value = "Nombre Completo";
                        worksheet.Cell("E" + (CeldaInicial + 1)).Value = "Cédula";
                        worksheet.Cell("F" + (CeldaInicial + 1)).Value = "Teléfono";
                        worksheet.Cell("G" + (CeldaInicial + 1)).Value = "Municipio";
                        worksheet.Cell("H" + (CeldaInicial + 1)).Value = "Barrio";
                        worksheet.Cell("I" + (CeldaInicial + 1)).Value = "Dirección";
                        worksheet.Cell("J" + (CeldaInicial + 1)).Value = "Observación";

                        var currentRow_JS_ND = 2 + CeldaInicial;
                        var numeracionJsNoDisponible = 1;
                        foreach (var user in JS_No_Disponible)
                        {
                            var Observaciones = user.Observaciones.Count();
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";

                            if (Observaciones > 1)
                            {
                                worksheet.Range("B" + currentRow_JS_ND + ":B" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(numeracionJsNoDisponible);
                                worksheet.Range("C" + currentRow_JS_ND + ":C" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(user.Oficina.Nombre);
                                worksheet.Range("D" + currentRow_JS_ND + ":D" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido);
                                worksheet.Range("E" + currentRow_JS_ND + ":E" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(user.Cedula);
                                worksheet.Range("F" + currentRow_JS_ND + ":F" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(user.Telefono);
                                worksheet.Range("G" + currentRow_JS_ND + ":G" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.MunicipioDes.ToLower()));
                                worksheet.Range("H" + currentRow_JS_ND + ":H" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(barrio);
                                worksheet.Range("I" + currentRow_JS_ND + ":I" + (currentRow_JS_ND + Observaciones - 1)).Merge().SetValue(barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower()));
                                var celdaObservacion = currentRow_JS_ND;
                                foreach (var obs in user.Observaciones)
                                {
                                    worksheet.Cell("J" + celdaObservacion).Value = obs.ObservacionDet;
                                    celdaObservacion++;
                                }
                                currentRow_JS_ND = currentRow_JS_ND + Observaciones;
                                numeracionJsNoDisponible++;
                            }
                            else
                            {
                                worksheet.Cell("B" + currentRow_JS_ND).Value = numeracionJsNoDisponible;
                                worksheet.Cell("C" + currentRow_JS_ND).Value = user.Oficina.Nombre;
                                worksheet.Cell("D" + currentRow_JS_ND).Value = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                                worksheet.Cell("E" + currentRow_JS_ND).Value = user.Cedula;
                                worksheet.Cell("F" + currentRow_JS_ND).Value = user.Telefono;
                                worksheet.Cell("G" + currentRow_JS_ND).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                                worksheet.Cell("H" + currentRow_JS_ND).Value = barrio;
                                worksheet.Cell("I" + currentRow_JS_ND).Value = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());
                                worksheet.Cell("J" + currentRow_JS_ND).Value = user.Observaciones.Count > 0 ? user.Observaciones.FirstOrDefault().ObservacionDet : "";

                                currentRow_JS_ND++;
                                numeracionJsNoDisponible++;
                            }
                        }

                        #endregion

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;
                #endregion

                case TIPO_REPORTE.MENORES_30:
                    #region REPORTE DE TODO EL PERSONAL DISPONIBLE DE LA DIS MENOR A 35


                    var JS_Menores = EsManagua
                                   ? ObtenerListaPersonal().Result.Where(p => _utilitario.ObtenerEdadActualCedula(p.Cedula)<=35   && p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).ToList()
                                   : ObtenerListaPersonal().Result.Where(p => _utilitario.ObtenerEdadActualCedula(p.Cedula) <= 35 && p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).ToList();
                 
                  


                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Reporte Juventud DIS");


                        worksheet.Columns("B").Width = 8;
                        worksheet.Columns("C").Width = 17;
                        worksheet.Columns("D").Width = 45;
                        worksheet.Columns("E").Width = 16;
                        worksheet.Columns("F").Width = 17;
                        worksheet.Columns("G").Width = 15;
                        worksheet.Columns("H").Width = 22;
                        worksheet.Columns("I").Width = 80;
                        worksheet.Columns("J").Width = 40;
                        //worksheet.Columns().AdjustToContents();

                        #region Jovenes Disponibles

                        EncabezadoTabla = "B2:I2";
                        registrosTotales = JS_Menores.Count();
                        CabeceraTabla = "B3:I3";
                        ContenidoTabla = "B4:I" + (3 + registrosTotales).ToString();
                        TablaCompleta = "B2:I" + (3 + registrosTotales).ToString();
                        worksheet.Range(EncabezadoTabla).Merge().SetValue("JOVENES DISPOBLE PARA ACTIVIDADES").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "No.";
                        worksheet.Cell("C3").Value = "Oficina";
                        worksheet.Cell("D3").Value = "Nombre Completo";
                        worksheet.Cell("E3").Value = "Cédula";
                        worksheet.Cell("F3").Value = "Teléfono";
                        worksheet.Cell("G3").Value = "Municipio";
                        worksheet.Cell("H3").Value = "Barrio";
                        worksheet.Cell("I3").Value = "Dirección";
                        worksheet.Range(CabeceraTabla).SetAutoFilter();

                        var currentRow = 4;
                        var numeracionJsDisponible = 1;
                        foreach (var user in JS_Menores)
                        {
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";
                            worksheet.Cell("B" + currentRow).Value = numeracionJsDisponible;
                            worksheet.Cell("C" + currentRow).Value = user.Oficina.Nombre;
                            worksheet.Cell("D" + currentRow).Value = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                            worksheet.Cell("E" + currentRow).Value = user.Cedula;
                            worksheet.Cell("F" + currentRow).Value = user.Telefono;
                            worksheet.Cell("G" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                            worksheet.Cell("H" + currentRow).Value = barrio;
                            worksheet.Cell("I" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());
                            currentRow++;
                            numeracionJsDisponible++;
                        }

                        #endregion


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;
                #endregion
                
                case TIPO_REPORTE.ARMADO:
                    #region REPOTE ARMADO
                    var todo_personal = EsManagua
                                   ? ObtenerListaPersonal().Result.Where(p => p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).OrderBy(p => p.OficinaId).ThenBy(p=> p.PrimerNombre).ToList()
                                   : ObtenerListaPersonal().Result.Where(p => p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).OrderBy(p => p.OficinaId).ThenBy(p=> p.PrimerNombre).ToList();
                    todo_personal.ForEach(x =>
                    {
                        var observacionesPersona = _ObservacionDominio.ObtenerListaObservacionByPersonal(x.PersonalId).Result;
                        var tieneObservacion = LstObservaciones.Count > 0 ? observacionesPersona.ToList().Exists(o => LstObservaciones.Contains(o.ObservacionDet)) : true;
                        var tieneRestriccion = observacionesPersona.ToList().Exists(o => LstRestricciones.Contains(o.ObservacionDet));
                        x.Observaciones = observacionesPersona.Where(o => LstObservaciones.Contains(o.ObservacionDet)).ToList();
                        if (tieneObservacion && !tieneRestriccion)
                            personal.Add(x);
                    });

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Reporte Armado");


                        worksheet.Columns("B").Width = 8;
                        worksheet.Columns("C").Width = 17;
                        worksheet.Columns("D").Width = 45;
                        worksheet.Columns("E").Width = 16;
                        worksheet.Columns("F").Width = 17;
                        worksheet.Columns("G").Width = 15;
                        worksheet.Columns("H").Width = 22;
                        worksheet.Columns("I").Width = 80;
                        worksheet.Columns("J").Width = 40;

                        EncabezadoTabla = "B2:J2";
                        registrosTotales = LstObservaciones.Count > 0  ? personal.Sum(x => x.Observaciones.Count()) : personal.Count();
                        CabeceraTabla = "B3:J3";
                        ContenidoTabla = "B4:J" + (3 + registrosTotales).ToString();
                        TablaCompleta = "B2:J" + (3 + registrosTotales).ToString();


                        worksheet.Range(EncabezadoTabla).Merge().SetValue("PERSONAL DIS - REPORTE").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "No.";
                        worksheet.Cell("C3").Value = "Oficina";
                        worksheet.Cell("D3").Value = "Nombre Completo";
                        worksheet.Cell("E3").Value = "Cédula";
                        worksheet.Cell("F3").Value = "Teléfono";
                        worksheet.Cell("G3").Value = "Municipio";
                        worksheet.Cell("H3").Value = "Barrio";
                        worksheet.Cell("I3").Value = "Dirección";
                        worksheet.Cell("J3").Value = "Observación";
                        worksheet.Range(CabeceraTabla).SetAutoFilter();

                        var currentRow = 4;
                        var numeracion = 1;
                        foreach (var user in personal)
                        {
                            var Observaciones = user.Observaciones.Count();
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";

                            if (Observaciones > 1)
                            {
                                worksheet.Range("B" + currentRow + ":B" + (currentRow + Observaciones - 1)).Merge().SetValue(numeracion);
                                worksheet.Range("C" + currentRow + ":C" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Oficina.Nombre);
                                worksheet.Range("D" + currentRow + ":D" + (currentRow + Observaciones - 1)).Merge().SetValue(user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido);
                                worksheet.Range("E" + currentRow + ":E" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Cedula);
                                worksheet.Range("F" + currentRow + ":F" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Telefono);
                                worksheet.Range("G" + currentRow + ":G" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.MunicipioDes.ToLower()));
                                worksheet.Range("H" + currentRow + ":H" + (currentRow + Observaciones - 1)).Merge().SetValue(barrio);
                                worksheet.Range("I" + currentRow + ":I" + (currentRow + Observaciones - 1)).Merge().SetValue(barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower()));
                                var celdaObservacion = currentRow;
                                foreach (var obs in user.Observaciones)
                                {
                                    worksheet.Cell("J" + celdaObservacion).Value = obs.ObservacionDet;
                                    celdaObservacion++;
                                }
                                currentRow = currentRow + Observaciones;
                                numeracion++;
                            }
                            else
                            {
                                worksheet.Cell("B" + currentRow).Value = numeracion;
                                worksheet.Cell("C" + currentRow).Value = user.Oficina.Nombre;
                                worksheet.Cell("D" + currentRow).Value = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                                worksheet.Cell("E" + currentRow).Value = user.Cedula;
                                worksheet.Cell("F" + currentRow).Value = user.Telefono;
                                worksheet.Cell("G" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                                worksheet.Cell("H" + currentRow).Value = barrio;
                                worksheet.Cell("I" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());
                                worksheet.Cell("J" + currentRow).Value = user.Observaciones.Count > 0 ? user.Observaciones.FirstOrDefault().ObservacionDet : "";
                                currentRow++;
                                numeracion++;
                            }
                        }

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;
#endregion

                case TIPO_REPORTE.PERSONAL_ESTUDIANDO:
                    #region REPORTE DE PERSONAL DIS ESTUDIANDO

                    personal_estudiando = EsManagua
                                   ? ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("Estudia")) && p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).ToList()
                                   : ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("Estudia")) && p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).ToList();

                    personal_estudiando.ForEach(x =>
                    {
                        var estudio = _ObservacionDominio.ObtenerListaObservacionByPersonal(x.PersonalId).Result.Where(o => o.ObservacionDet.Contains("Estudia")).ToList();
                        x.Observaciones = estudio;
                    });


                    EncabezadoTabla = "B2:I2";
                    registrosTotales = personal_estudiando.Sum(x => x.Observaciones.Count());
                    CabeceraTabla = "B3:I3";
                    ContenidoTabla = "B4:I" + (3 + registrosTotales).ToString();
                    TablaCompleta = "B2:I" + (3 + registrosTotales).ToString();

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Reporte Personal Estudiando");

                        worksheet.Range(EncabezadoTabla).Merge().SetValue("JOVENES ESTUDIANDO").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "Primer Nombre";
                        worksheet.Cell("C3").Value = "Segundo Nombre";
                        worksheet.Cell("D3").Value = "Primer Apellido";
                        worksheet.Cell("E3").Value = "Segundo Apellido";
                        worksheet.Cell("F3").Value = "Cédula";
                        worksheet.Cell("G3").Value = "Teléfono";
                        worksheet.Cell("H3").Value = "Municipio";
                        worksheet.Cell("I3").Value = "Observaciones";

                        var currentRow = 4;
                        foreach (var user in personal_estudiando)
                        {

                            var Observaciones = user.Observaciones.Count();
                            if (Observaciones > 1)
                            {
                                worksheet.Range("B" + currentRow + ":B" + (currentRow + Observaciones - 1)).Merge().SetValue(user.PrimerNombre);
                                worksheet.Range("C" + currentRow + ":C" + (currentRow + Observaciones - 1)).Merge().SetValue(user.SegundoNombre);
                                worksheet.Range("D" + currentRow + ":D" + (currentRow + Observaciones - 1)).Merge().SetValue(user.PrimerApellido);
                                worksheet.Range("E" + currentRow + ":E" + (currentRow + Observaciones - 1)).Merge().SetValue(user.SegundoApellido);
                                worksheet.Range("F" + currentRow + ":F" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Cedula);
                                worksheet.Range("G" + currentRow + ":G" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Telefono);
                                worksheet.Range("H" + currentRow + ":H" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.MunicipioDes.ToLower()));
                                var celdaObservacion = currentRow;
                                foreach (var obs in user.Observaciones)
                                {
                                    worksheet.Cell("I" + celdaObservacion).Value = obs.ObservacionDet;
                                    celdaObservacion++;
                                }
                                currentRow = currentRow + Observaciones;
                            }
                            else
                            {
                                worksheet.Cell("B" + currentRow).Value = user.PrimerNombre;
                                worksheet.Cell("C" + currentRow).Value = user.SegundoNombre;
                                worksheet.Cell("D" + currentRow).Value = user.PrimerApellido;
                                worksheet.Cell("E" + currentRow).Value = user.SegundoApellido;
                                worksheet.Cell("F" + currentRow).Value = user.Cedula;
                                worksheet.Cell("G" + currentRow).Value = user.Telefono;
                                worksheet.Cell("H" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                                worksheet.Cell("I" + currentRow).Value = user.Observaciones.Count > 0 ? user.Observaciones.FirstOrDefault().ObservacionDet : "";
                                currentRow++;
                            }
                        }



                        //worksheet.Rows().AdjustToContents();
                        worksheet.Columns("B").Width = 20;
                        worksheet.Columns("C").Width = 20;
                        worksheet.Columns("D").Width = 20;
                        worksheet.Columns("E").Width = 20;
                        worksheet.Columns("F").Width = 20;
                        worksheet.Columns("G").Width = 16;
                        worksheet.Columns("H").Width = 18;
                        worksheet.Columns("I").Width = 40;
                        worksheet.Range("B3:I3").SetAutoFilter();
                        worksheet.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;

                #endregion

                case TIPO_REPORTE.PERSONAL_TRABAJANDO_IR:
                    #region REPORTE DE PERSONAL DIS TRABAJANDO EN IR

                    personal_TrabajaIR = EsManagua
                                   ? ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("IR")) && p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).ToList()
                                   : ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("IR")) && p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).ToList();
                    personal_TrabajaIR.ForEach(x =>
                    {
                        var trabajaMesa = _ObservacionDominio.ObtenerListaObservacionByPersonal(x.PersonalId).Result.Where(o => o.ObservacionDet.Contains("IR")).ToList();
                        x.Observaciones = trabajaMesa;
                    });

                    EncabezadoTabla = "B2:I2";
                    registrosTotales = personal_TrabajaIR.Sum(x => x.Observaciones.Count());
                    CabeceraTabla = "B3:I3";
                    ContenidoTabla = "B4:I" + (3 + registrosTotales).ToString();
                    TablaCompleta = "B2:I" + (3 + registrosTotales).ToString();

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("ReportePeronalMesaElectoral");

                        worksheet.Range(EncabezadoTabla).Merge().SetValue("JOVENES TRABAJANDO IR").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "Primer Nombre";
                        worksheet.Cell("C3").Value = "Segundo Nombre";
                        worksheet.Cell("D3").Value = "Primer Apellido";
                        worksheet.Cell("E3").Value = "Segundo Apellido";
                        worksheet.Cell("F3").Value = "Cédula";
                        worksheet.Cell("G3").Value = "Teléfono";
                        worksheet.Cell("H3").Value = "Municipio";
                        worksheet.Cell("I3").Value = "Observaciones";

                        var currentRow = 4;
                        foreach (var user in personal_TrabajaIR)
                        {

                            var Observaciones = user.Observaciones.Count();
                            if (Observaciones > 1)
                            {
                                worksheet.Range("B" + currentRow + ":B" + (currentRow + Observaciones - 1)).Merge().SetValue(user.PrimerNombre);
                                worksheet.Range("C" + currentRow + ":C" + (currentRow + Observaciones - 1)).Merge().SetValue(user.SegundoNombre);
                                worksheet.Range("D" + currentRow + ":D" + (currentRow + Observaciones - 1)).Merge().SetValue(user.PrimerApellido);
                                worksheet.Range("E" + currentRow + ":E" + (currentRow + Observaciones - 1)).Merge().SetValue(user.SegundoApellido);
                                worksheet.Range("F" + currentRow + ":F" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Cedula);
                                worksheet.Range("G" + currentRow + ":G" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Telefono);
                                worksheet.Range("H" + currentRow + ":H" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.MunicipioDes.ToLower()));
                                var celdaObservacion = currentRow;
                                foreach (var obs in user.Observaciones)
                                {
                                    worksheet.Cell("I" + celdaObservacion).Value = obs.ObservacionDet;
                                    celdaObservacion++;
                                }
                                currentRow = currentRow + Observaciones;
                            }
                            else
                            {
                                worksheet.Cell("B" + currentRow).Value = user.PrimerNombre;
                                worksheet.Cell("C" + currentRow).Value = user.SegundoNombre;
                                worksheet.Cell("D" + currentRow).Value = user.PrimerApellido;
                                worksheet.Cell("E" + currentRow).Value = user.SegundoApellido;
                                worksheet.Cell("F" + currentRow).Value = user.Cedula;
                                worksheet.Cell("G" + currentRow).Value = user.Telefono;
                                worksheet.Cell("H" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                                worksheet.Cell("I" + currentRow).Value = user.Observaciones.Count > 0 ? user.Observaciones.FirstOrDefault().ObservacionDet : "";
                                currentRow++;
                            }
                        }



                        //worksheet.Rows().AdjustToContents();
                        worksheet.Columns("B").Width = 20;
                        worksheet.Columns("C").Width = 20;
                        worksheet.Columns("D").Width = 20;
                        worksheet.Columns("E").Width = 20;
                        worksheet.Columns("F").Width = 20;
                        worksheet.Columns("G").Width = 16;
                        worksheet.Columns("H").Width = 18;
                        worksheet.Columns("I").Width = 40;
                        worksheet.Range("B3:I3").SetAutoFilter();
                        worksheet.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;

                #endregion

                case TIPO_REPORTE.PERSONAL_RENTA:
                    #region REPORTE DE TODO EL PERSONAL DE LA RENTA
                    personal = EsManagua
                                  ? ObtenerListaPersonal().Result.Where(p => p.Genero.Contains(genero) && p.CodMunicipio == 10 && p.RentaId == (int)RENTA.DIS).OrderBy(p => p.OficinaId).ToList()
                                  : ObtenerListaPersonal().Result.Where(p => p.Genero.Contains(genero) && p.RentaId == (int)RENTA.DIS).OrderBy(p => p.OficinaId).ToList();

                    EncabezadoTabla = "B2:K2";
                    registrosTotales = personal.Sum(x => x.Observaciones.Count());
                    CabeceraTabla = "B3:K3";
                    ContenidoTabla = "B4:K" + (3 + registrosTotales).ToString();
                    TablaCompleta = "B2:K" + (3 + registrosTotales).ToString();

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Reporte Personal DIS");

                        worksheet.Range(EncabezadoTabla).Merge().SetValue("PERSONAL DIVISION DE INFORMATICA Y SISTEMA").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);


                        worksheet.Cell("B3").Value = "No.";
                        worksheet.Cell("C3").Value = "Nombre Completo";
                        worksheet.Cell("D3").Value = "Cédula";
                        worksheet.Cell("E3").Value = "Teléfono";
                        worksheet.Cell("F3").Value = "Oficina";
                        worksheet.Cell("G3").Value = "Departamento";
                        worksheet.Cell("H3").Value = "Municipio";
                        worksheet.Cell("I3").Value = "Barrio";
                        worksheet.Cell("J3").Value = "Dirección";
                        worksheet.Cell("K3").Value = "Observaciones";

                        var currentRow = 4;
                        var numeracion = 1;
                        foreach (var user in personal)
                        {

                            var nombreCompleto = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";
                            var direccion = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());

                            var Observaciones = user.Observaciones.Count();
                            if (Observaciones > 1)
                            {
                                worksheet.Range("B" + currentRow + ":B" + (currentRow + Observaciones - 1)).Merge().SetValue(numeracion);
                                worksheet.Range("C" + currentRow + ":C" + (currentRow + Observaciones - 1)).Merge().SetValue(nombreCompleto);
                                worksheet.Range("D" + currentRow + ":D" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Cedula);
                                worksheet.Range("E" + currentRow + ":E" + (currentRow + Observaciones - 1)).Merge().SetValue(user.Telefono);
                                worksheet.Range("F" + currentRow + ":F" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.Oficina.Nombre));
                                worksheet.Range("G" + currentRow + ":G" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.DepartamentoDes.ToLower()));
                                worksheet.Range("H" + currentRow + ":H" + (currentRow + Observaciones - 1)).Merge().SetValue(ti.ToTitleCase(user.MunicipioDes.ToLower()));
                                worksheet.Range("I" + currentRow + ":I" + (currentRow + Observaciones - 1)).Merge().SetValue(barrio);
                                worksheet.Range("J" + currentRow + ":J" + (currentRow + Observaciones - 1)).Merge().SetValue(direccion);
                                var celdaObservacion = currentRow;
                                foreach (var obs in user.Observaciones)
                                {
                                    worksheet.Cell("K" + celdaObservacion).Value = obs.ObservacionDet;
                                    celdaObservacion++;
                                }
                                currentRow = currentRow + Observaciones;
                            }
                            else
                            {
                                worksheet.Cell("B" + currentRow).Value = numeracion;
                                worksheet.Cell("C" + currentRow).Value = nombreCompleto;
                                worksheet.Cell("D" + currentRow).Value = user.Cedula;
                                worksheet.Cell("E" + currentRow).Value = user.Telefono;
                                worksheet.Cell("F" + currentRow).Value = ti.ToTitleCase(user.Oficina.Nombre);
                                worksheet.Cell("G" + currentRow).Value = ti.ToTitleCase(user.DepartamentoDes.ToLower());
                                worksheet.Cell("H" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                                worksheet.Cell("I" + currentRow).Value = barrio;
                                worksheet.Cell("J" + currentRow).Value = direccion;
                                worksheet.Cell("K" + currentRow).Value = user.Observaciones.Count > 0 ? user.Observaciones.FirstOrDefault().ObservacionDet : "";
                                currentRow++;
                            }
                            numeracion++;
                        }

                        worksheet.Columns().AdjustToContents();
                        worksheet.Columns("B").Width = 20;
                        worksheet.Columns("C").Width = 35;
                        worksheet.Columns("D").Width = 20;
                        worksheet.Columns("E").Width = 15;
                        worksheet.Columns("F").Width = 17;
                        worksheet.Columns("G").Width = 20;
                        worksheet.Columns("H").Width = 20;
                        worksheet.Columns("I").Width = 22;
                        worksheet.Columns("J").Width = 80;
                        worksheet.Columns("K").Width = 40;
                        worksheet.Range("B3:I3").SetAutoFilter();


                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }
                    break;
                #endregion

                case TIPO_REPORTE.COORDINADORES_JS:
                    #region REPORTE DE LOS COORDINADORES DE RENTAS

                    personal = EsManagua
                             ? ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("Coordinador de Juventud Sandinista")) && p.Genero.Contains(genero) && p.CodMunicipio == 10).ToList()
                             : ObtenerListaPersonal().Result.Where(p => p.Observaciones.Exists(o => o.ObservacionDet.Contains("Coordinador de Juventud Sandinista")) && p.Genero.Contains(genero)).ToList();
                    personal.ForEach(x =>
                    {
                        var observacion = _ObservacionDominio.ObtenerListaObservacionByPersonal(x.PersonalId).Result.Where(o => o.ObservacionDet.Contains("Coordinador de Juventud Sandinista")).ToList();
                        x.Observaciones = observacion;
                    });


                    EncabezadoTabla = "B2:H2";
                    registrosTotales = personal.Sum(x => x.Observaciones.Count());
                    CabeceraTabla = "B3:H3";
                    ContenidoTabla = "B4:H" + (3 + registrosTotales).ToString();
                    TablaCompleta = "B2:H" + (3 + registrosTotales).ToString();

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("COORDINADORES - JS");

                        worksheet.Range(EncabezadoTabla).Merge().SetValue("COORDINADORES DE JUVENTUD SANDINISTA POR RENTA").Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.FromArgb(48, 84, 150))
                            .Fill.SetBackgroundColor(XLColor.White);


                        //ESTABLECEMOS EL RANGO DE LA TABLA COMPLETA
                        worksheet.Range(TablaCompleta).Style
                            .Border.SetInsideBorder(XLBorderStyleValues.Thin).Border.SetInsideBorderColor(XLColor.Black)
                            .Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.Black);

                        //ESTABLECEMOS EL RANGO DE LA CABECERA DE LA TABLA
                        worksheet.Range(CabeceraTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Font.SetBold()
                            .Font.SetFontColor(XLColor.White)
                            .Fill.SetBackgroundColor(XLColor.FromArgb(68, 114, 196));

                        //ESTABLECEMOS EL RANGO DEL CONTENIDO DE LA TABLA
                        worksheet.Range(ContenidoTabla).Style
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                              .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                              .Font.SetBold(false)
                              .Font.SetFontColor(XLColor.Black)
                              .Fill.SetBackgroundColor(XLColor.White);

                        worksheet.Cell("B3").Value = "Renta";
                        worksheet.Cell("C3").Value = "Nombre Completo";
                        worksheet.Cell("D3").Value = "Cédula";
                        worksheet.Cell("E3").Value = "Teléfono";
                        worksheet.Cell("F3").Value = "Municipio";
                        worksheet.Cell("G3").Value = "Barrio";
                        worksheet.Cell("H3").Value = "Dirección";

                        var currentRow = 4;
                        foreach (var user in personal)
                        {
                            var barrio = user.BarrioDes != null && !user.BarrioDes.Equals("") ? !(user.BarrioDes.Contains("Comarca") || user.BarrioDes.Contains("Carretera")) ? "B° " + user.BarrioDes : user.BarrioDes : "";
                            worksheet.Cell("B" + currentRow).Value = user.Renta.NombreRenta;
                            worksheet.Cell("C" + currentRow).Value = user.PrimerNombre + " " + user.SegundoNombre + " " + user.PrimerApellido + " " + user.SegundoApellido;
                            worksheet.Cell("D" + currentRow).Value = user.Cedula;
                            worksheet.Cell("E" + currentRow).Value = user.Telefono;
                            worksheet.Cell("F" + currentRow).Value = ti.ToTitleCase(user.MunicipioDes.ToLower());
                            worksheet.Cell("G" + currentRow).Value = barrio;
                            worksheet.Cell("H" + currentRow).Value = barrio.Equals("") ? ti.ToTitleCase(user.Direccion.ToLower()) : barrio + ", " + ti.ToTitleCase(user.Direccion.ToLower());
                            currentRow++;
                        }



                        worksheet.Columns().AdjustToContents();
                        worksheet.Columns("B").Width = 13;
                        worksheet.Columns("C").Width = 40;
                        worksheet.Columns("D").Width = 20;
                        worksheet.Columns("E").Width = 20;
                        worksheet.Columns("F").Width = 20;
                        worksheet.Columns("G").Width = 22;
                        worksheet.Columns("H").Width = 80;
                        worksheet.Range("B3:H3").SetAutoFilter();

                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            retorno = stream.ToArray();
                        }
                    }

                    break;
                #endregion

                default:
                    break;
            }

            return retorno;
        }

        public async Task<List<PersonalDto>> ObtenerListaPersonal()
        {
            var resultado = new List<PersonalDto>();
            try
            {
                var personals = unidadTrabajo.PersonalRepositorio.ListarTodos();
                foreach (var item in personals)
                {
                    var it = Mapper.Map<Personal, PersonalDto>(item);
                    var depar = await _departamentoDominio.ObtenerDepartamentoById(it.CodDepartamento);
                    var muni = await _municipioDominio.ObtenerMunicipioById(it.CodMunicipio);
                    var barrio = await _barrioDominio.ObtenerBarrioById(it.CodBarrio);
                    var renta = await _rentaDominio.ObtenerRentaById(it.RentaId.Value);
                    var oficina = await _OficinaDominio.ObtenerOficinaById(it.OficinaId.Value);
                    var observaciones = await _ObservacionDominio.ObtenerListaObservacionByPersonal(item.PersonalId);
                    it.DepartamentoDes = depar.DescripcionDepartamento;
                    it.MunicipioDes = muni.DescripcionMunicipio;
                    it.BarrioDes = barrio.NombreBarrio;
                    //it.Direccion = barrio.NombreBarrio != null && !barrio.NombreBarrio.Equals("") ? "B° " + barrio.NombreBarrio + ", " + it.Direccion : it.Direccion;
                    it.Renta = renta;
                    it.Oficina = oficina;
                    it.Observaciones = observaciones.Count == 0 ? new List<ObservacionDto>() { new ObservacionDto() { ObservacionDet = "" } } : observaciones;
                    resultado.Add(it);
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al actualizar el personal.");

            }
        }


        public async Task<PersonalDto> ObtenerPersonalById(int PersonalId)
        {
            var resultado = new PersonalDto();
            try
            {
                var personal = unidadTrabajo.PersonalRepositorio.BuscarPorId(PersonalId);
                resultado = personal != null ? Mapper.Map<Personal, PersonalDto>(personal) : resultado;
                var depar = await _departamentoDominio.ObtenerDepartamentoById(resultado.CodDepartamento);
                var muni = await _municipioDominio.ObtenerMunicipioById(resultado.CodMunicipio);
                var barrio = await _barrioDominio.ObtenerBarrioById(resultado.CodBarrio);
                var renta = await _rentaDominio.ObtenerRentaById(resultado.RentaId.Value);
                var oficina = await _OficinaDominio.ObtenerOficinaById(resultado.OficinaId.Value);
                var observaciones = await _ObservacionDominio.ObtenerListaObservacionByPersonal(resultado.PersonalId);
                resultado.DepartamentoDes = depar.DescripcionDepartamento;
                resultado.MunicipioDes = muni.DescripcionMunicipio;
                resultado.BarrioDes = barrio.NombreBarrio;
                //resultado.Direccion = barrio.NombreBarrio != null && !barrio.NombreBarrio.Equals("") ? "B° " + barrio.NombreBarrio + ", " + resultado.Direccion : resultado.Direccion;
                resultado.Renta = renta;
                resultado.Oficina = oficina;
                resultado.Observaciones = observaciones;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al actualizar el personal.");

            }
            return resultado;

        }
    }
}
