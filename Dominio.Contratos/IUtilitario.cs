using Dominio.EntidadesDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IUtilitario
    {
        Task EnviarEmailAsync(MailDto mail);
        string GenerarCodigo();
        string ObtenerFechaLetra(DateTime Fecha);
        int ObtenerEdadActual(DateTime Fecha);
        int ObtenerEdadActualCedula(string Cedula);
        string MesLetra(int mes);
        string ObtenerHtml(string NameViewDocument, object Model);
        List<SelectListItem> ListaMesesDelAnio();
        Task<byte[]> ObtenerByteDocumento(string reporte, object Model, HttpContext httpContext, ControllerContext controllerContext);
    }
}
