using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IMunicipioDominio
    {
        Task<List<MunicipioDto>> ObtenerListaMunicipio();
        Task<MunicipioDto> ObtenerMunicipioById(int MunicipioId);
        Task<List<MunicipioDto>> ListaMunicipiosByDepartamentoId(int DepartamentoId);
        Task<bool> AgregarMunicipio(MunicipioDto Municipio);
        Task<bool> ActualizarMunicipio(MunicipioDto Municipio);
    }
}
