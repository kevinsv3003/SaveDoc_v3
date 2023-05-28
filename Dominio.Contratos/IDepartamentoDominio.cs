using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IDepartamentoDominio
    {
        Task<List<DepartamentoDto>> ObtenerListaDepartamento();
        Task<DepartamentoDto> ObtenerDepartamentoById(int DepartamentoId);
        Task<bool> AgregarDepartamento(DepartamentoDto Departamento);
        Task<bool> ActualizarDepartamento(DepartamentoDto Departamento);
    }
}
