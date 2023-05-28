
using Entidades.Entidades;
using System.Threading.Tasks;

namespace Infraestructura.Contratos
{
    public interface IDocumento : IBaseRepositorio<Documento>
    {
        Task<Documento> ObtenerDocumentoByte(int documentoId);
        bool ActualizarDocByte(int docId, string extension, byte[] documento);
        bool InsertarDocumento(Documento doc);
    }
}
