using Dominio.EntidadesDto;
using Microsoft.AspNetCore.Http;
using MimeKit;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Transversal
{
    public static class General
    {
        public static byte[] GetByteArrayFromDoc(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        public static void EnviarMensaje(string para, string asunto, string mensaje)
        {
            try
            {
                string de = "kevinsv3003@gmail.com";
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(de);
                mail.To.Add(new MailAddress(para));
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(de, "Ksv300399*");
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static string GenerarCodigo()
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

        public static string ObtenerFechaLetra(DateTime Fecha)
        {
            var dia = Fecha.Day;
            var mes = Fecha.Month;
            var año = Fecha.Year;
            var fechaLetra = dia + " de " + MesLetra(mes) + " de " + año;
            return fechaLetra;
        }

        public static int ObtenerEdadActual(DateTime Fecha)
        {
            var restaFechas = (DateTime.Now - Fecha);
            int Edad = new DateTime(restaFechas.Ticks).Year - 1;
            return Edad;
        }

        public static string MesLetra(int mes)
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

        public static string ObtenerHtml(string NameViewDocument, object Model)
        {
            var Reporte = string.Empty;
            var result = string.Empty;
            var Path = "Views\\Reportes\\{0}";
            try
            {
                var PathBodyReport = string.Format(Path, NameViewDocument);
                Reporte = File.ReadAllText(PathBodyReport);
                result = Engine.Razor.RunCompile(Reporte, "templateKey", null, Model);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un problema al conStruir HTML");
            }
            return result;
        }
    }
}
