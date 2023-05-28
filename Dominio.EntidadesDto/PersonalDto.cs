using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio.EntidadesDto
{
    public class PersonalDto
    {
        public int PersonalId { get; set; }
        public string PrimerNombre { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SegundoApellido { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public int CodDepartamento { get; set; }
        public int CodMunicipio { get; set; }
        public int CodBarrio { get; set; }
        public string DepartamentoDes { get; set; }
        public string MunicipioDes { get; set; }
        public string BarrioDes { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Direccion { get; set; }
        public int? RentaId { get; set; }
        public int? OficinaId { get; set; }
        public bool EsParticipante { get; set; }
        public bool Disponible { get; set; } = true;
        public RentaDto Renta { get; set; }
        public OficinaDto Oficina { get; set; }
        public List<ObservacionDto> Observaciones { get; set; }
    }
}
