using DimmedAPI.Entidades;

namespace DimmedAPI.Services
{
    public interface IPdfService
    {
        /// <summary>
        /// Genera un PDF de remisión a partir de una solicitud de entrada
        /// </summary>
        /// <param name="entryRequests">Objeto EntryRequests datos del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="lot">Imprimir lote 1: si, 0: no</param>
        /// <param name="price">Imprimir precio 1: si, 0: no</param>
        /// <param name="code">Imprimir codigo corto 1: si, 0: no</param>
        /// <param name="duedate">Imprimir fecha de vencimiento 1: si, 0: no</param>
        /// <param name="option">Imprimir solo lo despachado en el momento 1: si, 0: no</param>
        /// <param name="regSan">Imprimir registro sanitario 1: si, 0: no</param>
        /// <param name="printMethod">Método de impresión (0: flujo automático, 1: HTML, 2: iTextSharp)</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateRemisionPdfAsync(EntryRequests entryRequests, string companyCode, int lot, int price, int code, int duedate, int option, int regSan, int printMethod = 0);

        /// <summary>
        /// Genera el HTML de remisión para visualización en navegador
        /// </summary>
        /// <param name="entryRequests">Objeto EntryRequests datos del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="lot">Imprimir lote 1: si, 0: no</param>
        /// <param name="price">Imprimir precio 1: si, 0: no</param>
        /// <param name="code">Imprimir codigo corto 1: si, 0: no</param>
        /// <param name="duedate">Imprimir fecha de vencimiento 1: si, 0: no</param>
        /// <param name="option">Imprimir solo lo despachado en el momento 1: si, 0: no</param>
        /// <param name="regSan">Imprimir registro sanitario 1: si, 0: no</param>
        /// <returns>HTML de la remisión optimizado para impresión</returns>
        Task<string> GenerateRemisionHtmlForPrintAsync(EntryRequests entryRequests, string companyCode, int lot, int price, int code, int duedate, int option, int regSan);
    }
} 