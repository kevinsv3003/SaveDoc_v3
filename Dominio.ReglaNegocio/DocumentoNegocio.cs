using AutoMapper;
using Dominio.Contratos;
using Dominio.EntidadesDto;
using Entidades.Entidades;
using Infraestructura.Contratos;
using Infraestructura.Transversal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ReglaNegocio
{
    public class DocumentoNegocio : IDocumentoDominio
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public DocumentoNegocio(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        public async Task<bool> AgregarDocumento(DocumentoDto doc)
        {
            try
            {
                var documento = Mapper.Map<DocumentoDto, Documento>(doc);
                documento.Extension = doc.FileDoc.ContentType;
                documento.DocumentoByte = General.GetByteArrayFromDoc(doc.FileDoc);
                var retorno = await Task.Factory.StartNew(() =>
                {
                    //unidadTrabajo.DocumentoRepositorio.Insertar(documento);
                    unidadTrabajo.DocumentoRepositorio.InsertarDocumento(documento);
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al agregar el documento");
            }

        }

        public async Task<bool> ActualizarDocumento(DocumentoDto doc)
        {
            try
            {
                var documento = unidadTrabajo.DocumentoRepositorio.BuscarPorId(doc.DocumentoId);

                documento.Nombre = doc.Nombre;
                documento.Descripcion = doc.Descripcion;
                documento.AreaId = doc.AreaId;

                var retorno = await Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.DocumentoRepositorio.Actualizar(documento);

                    if (doc.FileDoc != null)
                        unidadTrabajo.DocumentoRepositorio.ActualizarDocByte(doc.DocumentoId, doc.FileDoc.ContentType, General.GetByteArrayFromDoc(doc.FileDoc));
                    
                    unidadTrabajo.Commit();
                    return true;
                });

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al actualizar el documento");
            }

        }

        public async Task<byte[]> ObtenerDocByteById(int Id)
        {
            try
            {
                var retorno = await Task.Factory.StartNew(() =>
                {
                    var documento = unidadTrabajo.DocumentoRepositorio.BuscarPorId(Id).DocumentoByte;
                    unidadTrabajo.Commit();
                    return documento;
                });
                return retorno;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un problema al consultar el documento"); throw;
            }
        }

        public async Task<DocumentoDto> ObtenerDocumentoPorId(int Id)
        {
            var resultado = new DocumentoDto();
            try
            {
                resultado = (from d in unidadTrabajo.DocumentoRepositorio.ListarTodos()
                             join a in unidadTrabajo.AreaRepositorio.ListarTodos()
                             on d.AreaId equals a.AreaId
                             where d.DocumentoId == Id && a.Estado
                             select new DocumentoDto
                             {
                                 DocumentoId = d.DocumentoId,
                                 AreaId = a.AreaId,
                                 Nombre = d.Nombre,
                                 Descripcion = d.Descripcion,
                                 NombreArea = a.Nombre,
                                 //Doc = d.DocumentoByte,
                                 //Extension = d.Extension
                             }).FirstOrDefault();

                return (resultado != null) ? resultado : new DocumentoDto();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los documentos");
            }
        }

        public async Task<List<DocumentoDto>> ObtenerDocumentoPorNombre(string nombreDoc)
        {
            var resultado = new List<DocumentoDto>();
            try
            {
                resultado = (from d in unidadTrabajo.DocumentoRepositorio.ListarTodos()
                             join a in unidadTrabajo.AreaRepositorio.ListarTodos()
                             on d.AreaId equals a.AreaId
                             where (d.Nombre ?? String.Empty).ToLower().Contains((nombreDoc ?? String.Empty).ToLower()) && a.Estado
                             select new DocumentoDto
                             {
                                 DocumentoId = d.DocumentoId,
                                 AreaId = a.AreaId,
                                 Nombre = d.Nombre,
                                 Descripcion = d.Descripcion,
                                 NombreArea = a.Nombre,
                                 //Doc = d.DocumentoByte,
                                 //Extension = d.Extension,
                                 Fondo = a.Fondo
                             }).ToList();

                return (resultado != null) ? resultado : new List<DocumentoDto>();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los documentos");
            }
        }

        public async Task<List<DocumentoDto>> ObtenerTodosDocumentos()
        {
            var resultado = new List<DocumentoDto>();
            try
            {
                resultado = (from d in unidadTrabajo.DocumentoRepositorio.ListarTodos()
                             join a in unidadTrabajo.AreaRepositorio.ListarTodos()
                             on d.AreaId equals a.AreaId
                             orderby d.AreaId
                             where a.Estado
                             select new DocumentoDto
                             {
                                 DocumentoId = d.DocumentoId,
                                 AreaId = a.AreaId,
                                 Nombre = d.Nombre,
                                 Descripcion = d.Descripcion,
                                 NombreArea = a.Nombre,
                                 //Doc = d.DocumentoByte,
                                 //Extension = d.Extension,
                                 Fondo = a.Fondo
                             }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los documentos");
            }
        }

        public async Task<List<DocumentoDto>> ObtenerDocumentoArea(int AreaId)
        {
            var resultado = new List<DocumentoDto>();
            try
            {
                resultado = (from d in unidadTrabajo.DocumentoRepositorio.ListarTodos()
                             join a in unidadTrabajo.AreaRepositorio.ListarTodos()
                             on d.AreaId equals a.AreaId
                             where a.AreaId == AreaId && a.Estado
                             orderby d.AreaId
                             select new DocumentoDto
                             {
                                 DocumentoId = d.DocumentoId,
                                 AreaId = a.AreaId,
                                 Nombre = d.Nombre,
                                 Descripcion = d.Descripcion,
                                 NombreArea = a.Nombre,
                                 //Doc = d.DocumentoByte,
                                 //Extension = d.Extension,
                                 Fondo = a.Fondo
                             }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los documentos");
            }
        }

        public Task<bool> EliminarDocumento(int id, out string mensaje)
        {
            try
            {
                var retorno = Task.Factory.StartNew(() =>
                {
                    unidadTrabajo.DocumentoRepositorio.Eliminar(id);
                    unidadTrabajo.Commit();
                    return true;
                });
                mensaje = ("Se eliminó el documento correctamente!!");
                return retorno;
            }
            catch (Exception)
            {
                mensaje = ("Ocurrió un problema al eliminar el documento");
                throw new Exception("Ocurrió un problema al eliminar el documento"); throw;
            }
        }

        public async Task<DocumentoDto> ObtenerDocumentoByte(int documentoId)
        {
            var resultado = new DocumentoDto();
            try
            {
                var doc = await unidadTrabajo.DocumentoRepositorio.ObtenerDocumentoByte(documentoId);

                if (doc != null)
                {
                    resultado.Extension = doc.Extension;
                    resultado.Doc = doc.DocumentoByte;
                }
                return (resultado != null) ? resultado : new DocumentoDto();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un problema al consultar los documentos");
            }
        }
    }
}
