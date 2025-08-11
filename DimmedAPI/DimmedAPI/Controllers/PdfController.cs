using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Services;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public PdfController(
            IPdfService pdfService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _pdfService = pdfService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Genera un PDF de remisión para una solicitud de entrada específica
        /// </summary>
        /// <param name="id">ID de la solicitud de entrada</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="lot">Imprimir lote 1: si, 0: no</param>
        /// <param name="price">Imprimir precio 1: si, 0: no</param>
        /// <param name="code">Imprimir codigo corto 1: si, 0: no</param>
        /// <param name="duedate">Imprimir fecha de vencimiento 1: si, 0: no</param>
        /// <param name="option">Imprimir solo lo despachado en el momento 1: si, 0: no</param>
        /// <param name="regSan">Imprimir registro sanitario 1: si, 0: no</param>
        /// <returns>Archivo PDF de la remisión</returns>
        [HttpGet("remision/{id}")]
        public async Task<IActionResult> GenerateRemisionPdf(
            int id,
            [FromQuery] string companyCode,
            [FromQuery] int lot = 1,
            [FromQuery] int price = 1,
            [FromQuery] int code = 0,
            [FromQuery] int duedate = 1,
            [FromQuery] int option = 0,
            [FromQuery] int regSan = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Obtener la solicitud de entrada con todos sus detalles
                var entryRequest = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.IdPatientNavigation)
                    .Include(er => er.IdMedicNavigation)
                    .Include(er => er.IdAtcNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestAssembly)
                    .Include(er => er.EntryRequestComponents)
                    .FirstOrDefaultAsync(er => er.Id == id);

                if (entryRequest == null)
                {
                    return NotFound($"No se encontró la solicitud de entrada con ID {id}");
                }

                // Generar el PDF
                byte[] pdfBytes = await _pdfService.GenerateRemisionPdfAsync(
                    entryRequest, lot, price, code, duedate, option, regSan);

                // Retornar el PDF como archivo
                string fileName = $"Remision_P-{id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Genera un PDF de remisión para una solicitud de entrada específica (POST)
        /// </summary>
        /// <param name="request">Solicitud para generar PDF</param>
        /// <returns>Archivo PDF de la remisión</returns>
        [HttpPost("remision")]
        public async Task<IActionResult> GenerateRemisionPdfPost([FromBody] GeneratePdfRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CompanyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(request.CompanyCode);

                // Obtener la solicitud de entrada con todos sus detalles
                var entryRequest = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.IdPatientNavigation)
                    .Include(er => er.IdMedicNavigation)
                    .Include(er => er.IdAtcNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.EntryRequestAssembly)
                    .Include(er => er.EntryRequestComponents)
                    .FirstOrDefaultAsync(er => er.Id == request.EntryRequestId);

                if (entryRequest == null)
                {
                    return NotFound($"No se encontró la solicitud de entrada con ID {request.EntryRequestId}");
                }

                // Generar el PDF
                byte[] pdfBytes = await _pdfService.GenerateRemisionPdfAsync(
                    entryRequest, 
                    request.Lot, 
                    request.Price, 
                    request.Code, 
                    request.DueDate, 
                    request.Option, 
                    request.RegSan);

                // Retornar el PDF como archivo
                string fileName = $"Remision_P-{request.EntryRequestId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// DTO para la solicitud de generación de PDF
    /// </summary>
    public class GeneratePdfRequest
    {
        /// <summary>
        /// ID de la solicitud de entrada
        /// </summary>
        public int EntryRequestId { get; set; }

        /// <summary>
        /// Código de la compañía
        /// </summary>
        public string CompanyCode { get; set; } = string.Empty;

        /// <summary>
        /// Imprimir lote 1: si, 0: no
        /// </summary>
        public int Lot { get; set; } = 1;

        /// <summary>
        /// Imprimir precio 1: si, 0: no
        /// </summary>
        public int Price { get; set; } = 1;

        /// <summary>
        /// Imprimir codigo corto 1: si, 0: no
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// Imprimir fecha de vencimiento 1: si, 0: no
        /// </summary>
        public int DueDate { get; set; } = 1;

        /// <summary>
        /// Imprimir solo lo despachado en el momento 1: si, 0: no
        /// </summary>
        public int Option { get; set; } = 0;

        /// <summary>
        /// Imprimir registro sanitario 1: si, 0: no
        /// </summary>
        public int RegSan { get; set; } = 1;
    }
} 