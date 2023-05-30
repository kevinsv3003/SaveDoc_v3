
using Dominio.Contratos;
using Dominio.EntidadesDto;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngine;
using RazorEngine.Templating;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Transversal
{
    public class Utilitario : IUtilitario
    {
        private readonly SmtpSettingsDto _smtpSetting;
        public IConfiguration Configuration { get; }

        public Utilitario(IOptions<SmtpSettingsDto> smtpSettingsDto, IConfiguration configuration)
        {
            this._smtpSetting = smtpSettingsDto.Value;
            this.Configuration = configuration;
        }

        public async Task EnviarEmailAsync(MailDto mail)
        {
            try
            {
                var message = CuerpoMensaje(mail);
                await SendMensaje(message);
                //EnviarMensaje(mail.Correo, mail.Asunto, mail.Mensaje);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void EnviarMensaje(string para, string asunto, string mensaje)
        {
            try
            {
                string de = "dgienlineaprueba@dgi.gob.ni";//"kevinsv3003@gmail.com";
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(de);
                mail.To.Add(new MailAddress(para));
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("new.dgi.gob.ni", 25))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    //smtp.Credentials = new NetworkCredential(de, "Ksv300399*");
                    smtp.Send(mail);
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                var cuerpoCorreo = ObtenerHtml("CorreoError.cshtml", new { TituloError = "Correo Del Destinatario Inválido.", MensajeError = ex.Message });
                EnviarMensaje("ksilva@dgi.gob.ni", "Error De Envío de Correo", cuerpoCorreo);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public string GenerarCodigo()
        {
            Random obj = new Random();
            string sCadena = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_,.<>+-*/¡!#$%&()=¿?";
            int longitud = sCadena.Length;
            char cletra;
            int nlongitud = 6;
            string sNuevacadena = string.Empty;

            for (int i = 0; i < nlongitud; i++)
            {
                cletra = sCadena[obj.Next(longitud)];
                sNuevacadena += cletra.ToString();
            }
            return sNuevacadena;
        }

        public string MesLetra(int mes)
        {
            string mesLetra = "";
            switch (mes)
            {
                case 1:
                    mesLetra = "Enero";
                    break;
                case 2:
                    mesLetra = "Febrero";
                    break;
                case 3:
                    mesLetra = "Marzo";
                    break;
                case 4:
                    mesLetra = "Abril";
                    break;
                case 5:
                    mesLetra = "Mayo";
                    break;
                case 6:
                    mesLetra = "Junio";
                    break;
                case 7:
                    mesLetra = "Julio";
                    break;
                case 8:
                    mesLetra = "Agosto";
                    break;
                case 9:
                    mesLetra = "Septiembre";
                    break;
                case 10:
                    mesLetra = "Obtubre";
                    break;
                case 11:
                    mesLetra = "Noviembre";
                    break;
                case 12:
                    mesLetra = "Diciembre";
                    break;
                default: break;
            }
            return mesLetra;
        }

        public List<SelectListItem> ListaMesesDelAnio()
        {
            var meses = new List<SelectListItem>();
            SelectListItem select = new SelectListItem() { Text = "..:: Seleccione una Opción ::..", Value = "0" };
            meses.Add(select);
            for (int i = 0; i < 12; i++)
            {
                meses.Add(new SelectListItem() { Text = MesLetra(i + 1) , Value=(i+1).ToString(), Selected=DateTime.Now.Month.Equals(i+1)});
            }
            return meses;
        }

        public int ObtenerEdadActual(DateTime Fecha)
        {
            var restaFechas = (DateTime.Now - Fecha);
            int Edad = new DateTime(restaFechas.Ticks).Year - 1;
            return Edad;
        }

        public string ObtenerFechaLetra(DateTime Fecha)
        {
            var dia = Fecha.Day;
            var mes = Fecha.Month;
            var año = Fecha.Year;
            var fechaLetra = dia + " de " + MesLetra(mes) + " de " + año;
            return fechaLetra;
        }

        public string ObtenerHtml(string NameViewDocument, object Model)
        {
            var Reporte = string.Empty;
            var result = string.Empty;
            var PathDir = "Views\\Reportes\\{0}";
            var key = GenerarTemplateKey();
            try
            {
                var PathBodyReport = string.Format(PathDir, NameViewDocument);
                var sjj = Path.GetFullPath(PathBodyReport);
                Reporte = File.ReadAllText(PathBodyReport);
                Reporte = File.ReadAllText(sjj);
                result = Engine.Razor.RunCompile(Reporte, key, null, Model);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un problema al construir HTML:" + ex.Message);
            }
            return result;
        }

        public async Task<byte[]> ObtenerByteDocumento(string reporte, object Model, HttpContext httpContext, ControllerContext controllerContext)
        {
            var retorno = new byte[0];
            var Path = "Views\\Reportes\\{0}";
            try
            {
                var Protocolo = (httpContext.Request.IsHttps) ? "https" : "http";
                var host = httpContext.Request.Host.Value;
                string _headerUrl = Protocolo + "://" + host + "/Home/_headerReporte";
                string _footerUrl = Protocolo + "://" + host + "/Home/_footerReporte";

                var viewName = string.Format(Path, reporte);
                var viewDocumento = new ViewAsPdf();

                viewDocumento = new ViewAsPdf()
                {
                    FileName = "ContratoCorreoContribuyente.pdf",
                    UserName = "dgi",
                    IsLowQuality = true,
                    Password = "dgi",
                    ViewName = viewName,
                    Model = Model,
                    PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageMargins = new Margins(30, 25, 40, 25)
                };

                retorno = await viewDocumento.BuildFile(controllerContext);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return retorno;
        }

        #region METODOS PRIVADOS
        private MimeMessage CuerpoMensaje(MailDto mailDto)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(_smtpSetting.SenderName, _smtpSetting.SenderEmail));
                message.To.Add(new MailboxAddress(mailDto.NombreCompleto, mailDto.Correo));
                message.Subject = mailDto.Asunto;
                //message.Headers[HeaderId.DispositionNotificationTo] = "ksilva@dgi.gob.ni";
                if (mailDto.ListaCopias != null)
                {
                    foreach (var cc in mailDto.ListaCopias)
                    {
                        message.Cc.Add(new MailboxAddress(cc.Nombre, cc.Correo));
                    }
                }
                BodyBuilder body = new BodyBuilder();
                body.HtmlBody = mailDto.Mensaje;
                if (mailDto.Archivo != null)
                    body.Attachments.Add("Reporte", mailDto.Archivo, ContentType.Parse("application/pdf"));

                message.Body = body.ToMessageBody();

                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<bool> SendMensaje(MimeMessage message)
        {
            try
            {
                using (MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtp.ConnectAsync(_smtpSetting.Server, _smtpSetting.Port);
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpCommandException ex)
            {
                var mailError = new MailDto() { NombreCompleto = "Kevin Silva", Correo = "kevinsv3003@gmail.com", Asunto = "Error De Envío de Correo." };
                string cuerpoCorreo = String.Empty;
                switch (ex.ErrorCode)
                {
                    case SmtpErrorCode.RecipientNotAccepted:
                        cuerpoCorreo = ObtenerHtml("CorreoError.cshtml", new { TituloError = "Correo Del Destinatario Inválido.", MensajeError = ex.Message });
                        mailError.Mensaje = cuerpoCorreo;
                        await SendMensaje(CuerpoMensaje(mailError));
                        break;
                    case SmtpErrorCode.SenderNotAccepted:
                        cuerpoCorreo = ObtenerHtml("CorreoError.cshtml", new { TituloError = "Correo Del Emisor Es Inválido.", MensajeError = ex.Message });
                        mailError.Mensaje = cuerpoCorreo;
                        await SendMensaje(CuerpoMensaje(mailError));
                        break;
                    case SmtpErrorCode.MessageNotAccepted:
                        cuerpoCorreo = ObtenerHtml("CorreoError.cshtml", new { TituloError = "Mensaje No Aceptado!!", MensajeError = ex.Message });
                        mailError.Mensaje = cuerpoCorreo;
                        await SendMensaje(CuerpoMensaje(mailError));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return true;
        }
        private string GenerarTemplateKey()
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var Charsarr = new char[4];
            var random = new Random();

            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }

            var resultString = "TemplateKey" + new String(Charsarr);
            return resultString;
        }

        public int ObtenerEdadActualCedula(string Cedula)
        {
            try
            {
                var cedulaCod = Cedula.Split('-');
                var FechaArray = cedulaCod[1].ToString().ToCharArray();
                var fechaString = string.Format("{0}/{1}/{2}", (FechaArray[0].ToString() + FechaArray[1].ToString()), (FechaArray[2].ToString() + FechaArray[3].ToString()), (FechaArray[4].ToString() + FechaArray[5].ToString()));
                var Fecha = Convert.ToDateTime(fechaString);
                var restaFechas = (DateTime.Now - Fecha);
                int Edad = new DateTime(restaFechas.Ticks).Year - 1;
                return Edad;
            }
            catch (Exception ex)
            {
                throw(ex);
            }
            
        }

        #endregion




        #region ENUMERADOS
        public enum TIPO_REPORTE : byte
        {
            JUVENTUD_DISPONIBLE = 1,
            CLS_DISPONIBLE=2,
            PERSONAL_DISPONIBLE = 3,
            JUVENTUD_NO_DISPONIBLE = 4,
            CLS_NO_DISPONIBLE = 5,
            PERSONAL_ESTUDIANDO = 6,
            PERSONAL_TRABAJANDO_IR = 7,
            PERSONAL_RENTA = 8,
            COORDINADORES_JS = 9,
            ARMADO = 10,
            MENORES_30 = 11,
        }

        public enum OFICINA : byte
        {
            UST = 2,
            UCC = 4,
            USAT = 5,
            DBA = 6,
            USA = 7,
            COMUNICACIONES = 8,
            SOPORTE_TECNICO = 9,
            OSIN = 10,
            DIRECCION = 11,
            RRHH = 12,
            SIN_ESPECIFICAR = 13
        }

        public enum RENTA : byte
        {
            GRACOS = 1,
            DIS = 2,
            CCM = 3,
            SAJONIA = 4,
            LINDA_VISTA = 5,
            LA_VIRGEN = 6,
            CENTRAL = 7,
        }

        public enum TIPO_MOVIMIENTO : byte
        {
            GASTOS = 1,
            INGRESOS = 2,
        }

        #endregion

    }
}
