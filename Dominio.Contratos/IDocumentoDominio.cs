using Dominio.EntidadesDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Contratos
{
    public interface IDocumentoDominio
    {
        Task<List<DocumentoDto>> ObtenerTodosDocumentos();
        Task<List<DocumentoDto>> ObtenerDocumentoArea(int AreaId);
        Task<DocumentoDto> ObtenerDocumentoPorId(int Id);
        Task<List<DocumentoDto>> ObtenerDocumentoPorNombre(string nombreDoc);
        Task<bool> AgregarDocumento(DocumentoDto doc);
        Task<bool> ActualizarDocumento(DocumentoDto doc);
        Task<bool> EliminarDocumento(int id, out string mensaje);
        Task<byte[]> ObtenerDocByteById(int Id);
        Task<DocumentoDto> ObtenerDocumentoByte(int documentoId);
    }
}
