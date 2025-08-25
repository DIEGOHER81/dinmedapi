using DimmedAPI.Entidades;
using DimmedAPI.BO;
using DimmedAPI.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Services
{
    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ICustomerBO _customerBO;
        private readonly IConverter _converter;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public PdfService(
            IWebHostEnvironment env,
            ICustomerBO customerBO,
            IConverter converter,
            IDynamicConnectionService dynamicConnectionService)
        {
            _env = env;
            _customerBO = customerBO;
            _converter = converter;
            _dynamicConnectionService = dynamicConnectionService;
        }

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
        public async Task<byte[]> GenerateRemisionPdfAsync(EntryRequests entryRequests, string companyCode, int lot, int price, int code, int duedate, int option, int regSan, int printMethod = 0)
        {
            var startTime = DateTime.Now;
            try
            {
                Console.WriteLine($"Iniciando generación de PDF para remisión P-{entryRequests.Id}");
                Console.WriteLine($"Parámetros: Lot={lot}, Price={price}, Code={code}, DueDate={duedate}, Option={option}, RegSan={regSan}, PrintMethod={printMethod}");
                
                // Generar el HTML de la remisión
                string htmlContent = await GenerateRemisionHtmlAsync(entryRequests, companyCode, lot, price, code, duedate, option, regSan);
                
                Console.WriteLine($"HTML generado exitosamente. Longitud: {htmlContent?.Length ?? 0} caracteres");
                
                // Determinar el método de generación basado en el parámetro printMethod
                switch (printMethod)
                {
                    case 1: // HTML - Forzar HTML
                        Console.WriteLine("MÉTODO 1: Forzando generación de HTML para impresión desde navegador");
                        throw new PdfGenerationException($"Método HTML seleccionado. Se puede usar la versión HTML para imprimir desde el navegador.");
                        
                    case 2: // iTextSharp - Forzar iTextSharp
                        Console.WriteLine("MÉTODO 2: Forzando generación con iTextSharp");
                        try
                        {
                            var iTextPdfBytes = await GeneratePdfWithITextSharpAsync(htmlContent, entryRequests.Id);
                            var endTime = DateTime.Now;
                            var duration = endTime - startTime;
                            Console.WriteLine($"✓ PDF generado exitosamente con iTextSharp (método forzado) en {duration.TotalMilliseconds:F2}ms. Tamaño final: {iTextPdfBytes.Length} bytes");
                            return iTextPdfBytes;
                        }
                        catch (Exception iTextEx)
                        {
                            Console.WriteLine($"✗ iTextSharp falló: {iTextEx.Message}");
                            throw new PdfGenerationException($"No se pudo generar el PDF con iTextSharp. Se puede usar la versión HTML para imprimir desde el navegador.", iTextEx);
                        }
                        
                    case 0: // Flujo automático (default)
                    default:
                        Console.WriteLine("MÉTODO 0: Usando flujo automático (DinkToPdf → HTML → iTextSharp)");
                        
                        // PASO 1: Intentar con DinkToPdf (método preferido)
                        if (_converter != null && IsDinkToPdfNativeAvailable())
                        {
                            try
                            {
                                Console.WriteLine("PASO 1: Intentando generar PDF con DinkToPdf...");
                                var pdfBytes = await GeneratePdfWithDinkToPdfAsync(htmlContent, entryRequests.Id);
                                
                                var endTime = DateTime.Now;
                                var duration = endTime - startTime;
                                Console.WriteLine($"✓ PDF generado exitosamente con DinkToPdf en {duration.TotalMilliseconds:F2}ms. Tamaño final: {pdfBytes.Length} bytes");
                                
                                return pdfBytes;
                            }
                            catch (Exception dinkEx)
                            {
                                Console.WriteLine($"✗ DinkToPdf falló: {dinkEx.Message}");
                                Console.WriteLine("PASO 2: Procediendo con fallback a HTML para impresión desde navegador...");
                                
                                // PASO 2: Si DinkToPdf falla, lanzar excepción para que el controlador proporcione enlace HTML
                                throw new PdfGenerationException($"No se pudo generar el PDF automáticamente. Se puede usar la versión HTML para imprimir desde el navegador.", dinkEx);
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠️ DinkToPdf no está disponible");
                            Console.WriteLine("PASO 2: Procediendo con fallback a HTML para impresión desde navegador...");
                            
                            // Si DinkToPdf no está disponible, ir directamente al HTML
                            throw new PdfGenerationException($"DinkToPdf no está disponible. Se puede usar la versión HTML para imprimir desde el navegador.");
                        }
                }
            }
            catch (PdfGenerationException)
            {
                // Re-lanzar la excepción para que el controlador la maneje
                throw;
            }
            catch (Exception ex)
            {
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                Console.WriteLine($"✗ Error crítico en GenerateRemisionPdfAsync después de {duration.TotalMilliseconds:F2}ms: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // PASO 3: Si todo falla y estamos en flujo automático, intentar con iTextSharp como último recurso
                if (printMethod == 0)
                {
                    Console.WriteLine("PASO 3: Último recurso - Intentando con iTextSharp...");
                    try
                    {
                        var htmlContent = await GenerateRemisionHtmlAsync(entryRequests, companyCode, lot, price, code, duedate, option, regSan);
                        var fallbackPdfBytes = await GeneratePdfWithITextSharpAsync(htmlContent, entryRequests.Id);
                        
                        var fallbackEndTime = DateTime.Now;
                        var fallbackDuration = fallbackEndTime - startTime;
                        Console.WriteLine($"✓ PDF generado exitosamente con iTextSharp (último recurso) en {fallbackDuration.TotalMilliseconds:F2}ms. Tamaño final: {fallbackPdfBytes.Length} bytes");
                        
                        return fallbackPdfBytes;
                    }
                    catch (Exception iTextEx)
                    {
                        Console.WriteLine($"✗ iTextSharp también falló: {iTextEx.Message}");
                        throw new PdfGenerationException($"No se pudo generar el PDF con ningún método disponible. Se puede usar la versión HTML para imprimir desde el navegador.", ex);
                    }
                }
                else
                {
                    // Si se especificó un método específico y falló, lanzar la excepción original
                    throw new PdfGenerationException($"Error generando PDF con el método especificado ({printMethod}): {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Genera PDF usando DinkToPdf para preservar el diseño visual del HTML
        /// </summary>
        private async Task<byte[]> GeneratePdfWithDinkToPdfAsync(string htmlContent, int entryRequestId)
        {
            try
            {
                Console.WriteLine("Generando PDF con DinkToPdf para preservar diseño visual...");
                
                // Mejorar la estructura del footer para centrado
                var improvedHtmlContent = ImproveFooterHtml(htmlContent);
                
                // Agregar CSS específico para saltos de página en DinkToPdf
                var enhancedHtmlContent = AddPageBreakCssForDinkToPdf(improvedHtmlContent);
                
                // Configurar las opciones de conversión
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.Letter,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                    // NO especificar Out para que retorne bytes en lugar de guardar en archivo
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = enhancedHtmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 9, Right = "Página [page] de [topage]", Line = true },
                    UseLocalLinks = true,
                    LoadSettings = new LoadSettings
                    {
                        ZoomFactor = 1.0
                    }
                };

                var document = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                // Convertir HTML a PDF - esto retornará bytes directamente
                var pdfBytes = _converter.Convert(document);

                Console.WriteLine($"PDF generado exitosamente con DinkToPdf. Tamaño: {pdfBytes.Length} bytes");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando PDF con DinkToPdf: {ex.Message}");
                throw new Exception($"Error generando PDF con DinkToPdf: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera el HTML de la remisión
        /// </summary>
        private async Task<string> GenerateRemisionHtmlAsync(EntryRequests entryRequests, string companyCode, int lot, int price, int code, int duedate, int option, int regSan)
        {
            try
            {
                Console.WriteLine("Iniciando generación de HTML...");
                var fullPath = Path.Combine(_env.WebRootPath, "Format", "htmlremision.html");
                string templateHtml = "";

                Console.WriteLine($"Buscando plantilla HTML en: {fullPath}");
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"No se encontró la plantilla HTML en: {fullPath}");
                }
                
                Console.WriteLine("Plantilla HTML encontrada, leyendo contenido...");

                templateHtml = await File.ReadAllTextAsync(fullPath);
                string detailsHtml = string.Empty;
                string detailsHtmlZero = string.Empty;
                string detailsEqpHtml = string.Empty;
                string atcName = "";

                List<string> detailsComp = new List<string>();
                List<string> detailsZero = new List<string>();

                // Obtener información del ATC
                if (entryRequests.IdAtcNavigation != null)
                    atcName = entryRequests.IdAtcNavigation.Name;

                // Obtener información del cliente usando el contexto dinámico de la compañía
                Customer customer = null;
                try
                {
                    var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                    customer = await companyContext.Customer
                        .FirstOrDefaultAsync(c => c.Id == entryRequests.IdCustomer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error obteniendo cliente con contexto dinámico: {ex.Message}");
                    // Fallback al método tradicional si falla el contexto dinámico
                    customer = await _customerBO.GetByIdAsync(entryRequests.IdCustomer);
                }

                // Configurar logo - usar el logo de la carpeta uploads/logos
                var logoPath = Path.Combine(_env.WebRootPath, "uploads", "logos", "suplemedicos.png");
                Console.WriteLine($"Buscando logo en: {logoPath}");
                
                if (!File.Exists(logoPath))
                {
                    Console.WriteLine("Logo especificado no encontrado, usando fallback...");
                    // Fallback al logo original si no existe el especificado
                    logoPath = Path.Combine(_env.WebRootPath, "template", "img", "suplemedicos.png");
                    Console.WriteLine($"Fallback logo path: {logoPath}");
                }
                else
                {
                    Console.WriteLine("Logo especificado encontrado");
                }
                
                // Convertir la ruta del logo a una URL relativa para el HTML
                var logoUrl = logoPath.Replace(_env.WebRootPath, "").Replace("\\", "/");
                if (!logoUrl.StartsWith("/"))
                    logoUrl = "/" + logoUrl;
                
                Console.WriteLine($"Logo URL final: {logoUrl}");
                templateHtml = templateHtml.Replace("#COMPANY_LOGO#", logoUrl);
                templateHtml = templateHtml.Replace("#NO#", "P-" + entryRequests.Id);
                templateHtml = templateHtml.Replace("#CUSTOMER_NAME#", entryRequests.IdCustomerNavigation?.Name ?? "");
                templateHtml = templateHtml.Replace("#NOMBRE_PACIENTE#", 
                    $"{entryRequests.IdPatientNavigation?.Name ?? ""} {entryRequests.IdPatientNavigation?.LastName ?? ""}");
                templateHtml = templateHtml.Replace("#CUSTOMER_NIT#", entryRequests.IdCustomerNavigation?.Identification ?? "");
                templateHtml = templateHtml.Replace("#PACIENTE_ID#", entryRequests.IdPatientNavigation?.Identification ?? "");
                templateHtml = templateHtml.Replace("#CUSTOMER_ADDRESS#", entryRequests.IdCustomerNavigation?.Address ?? "");
                templateHtml = templateHtml.Replace("#CUSTOMER_CITY#", entryRequests.IdCustomerNavigation?.City ?? "");
                templateHtml = templateHtml.Replace("#OBS#", entryRequests.Observations ?? "");

                string deliveryAddress = string.Empty;
                string applicant = string.Empty;

                if (!string.IsNullOrEmpty(entryRequests.Applicant) && customer != null)
                {
                    // Obtener el contexto de la compañía usando el companyCode proporcionado
                    var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                    
                    // Obtener las direcciones del cliente usando el contexto dinámico
                    try
                    {
                        var customerAddresses = await companyContext.CustomerAddress
                            .Where(ca => ca.CustomerId == customer.Id)
                            .ToListAsync();
                        
                        if (customerAddresses != null && customerAddresses.Count > 0)
                        {
                            var DA = customerAddresses.FirstOrDefault(x => x.Code == entryRequests.DeliveryAddress);
                            if (DA != null)
                                deliveryAddress = DA.Name;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error obteniendo direcciones del cliente: {ex.Message}");
                        // Continuar sin las direcciones si hay error
                    }

                    // Obtener los contactos del cliente usando el contexto dinámico
                    try
                    {
                        var customerContacts = await companyContext.CustomerContact
                            .Where(cc => cc.CustomerName == customer.Name)
                            .ToListAsync();
                        
                        if (customerContacts != null && customerContacts.Count > 0)
                        {
                            var CC = customerContacts.FirstOrDefault(x => x.Code == entryRequests.Applicant);
                            if (CC != null)
                                applicant = CC.Name;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error obteniendo contactos del cliente: {ex.Message}");
                        // Continuar sin los contactos si hay error
                    }
                }

                templateHtml = templateHtml.Replace("#DELIVERY_ADDRESS#", deliveryAddress);
                templateHtml = templateHtml.Replace("#ORDER_NO#", entryRequests.PurchaseOrder ?? "");
                templateHtml = templateHtml.Replace("#CUSTOMER_PHONE#", entryRequests.IdCustomerNavigation?.Phone ?? "");
                templateHtml = templateHtml.Replace("#APPLICANT#", applicant);
                templateHtml = templateHtml.Replace("#MECIC_NAME#", 
                    $"{entryRequests.IdMedicNavigation?.Name ?? ""} {entryRequests.IdMedicNavigation?.LastName ?? ""}");
                templateHtml = templateHtml.Replace("#HCPACIENTE#", entryRequests.IdPatientNavigation?.MedicalRecord ?? "");
                templateHtml = templateHtml.Replace("#ATC#", atcName);
                templateHtml = templateHtml.Replace("#DELIVERY_DATE#", entryRequests.DeliveryDate.ToString("dd/MM/yyyy"));
                templateHtml = templateHtml.Replace("#SURGERY_DATE#", 
                    entryRequests.SurgeryInit?.ToString("dd/MM/yyyy") ?? "");
                templateHtml = templateHtml.Replace("#SURGERY_END_DATE#", 
                    entryRequests.SurgeryEnd?.ToString("dd/MM/yyyy") ?? "");
                templateHtml = templateHtml.Replace("#SURGERY_TIME#", 
                    entryRequests.SurgeryInit?.ToString("HH:mm") ?? "");
                templateHtml = templateHtml.Replace("#FECHA_IMPRESION#", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                int colspan = 9;
                if (lot == 0) colspan--;
                if (price == 0) colspan -= 2;
                if (duedate == 0) colspan -= 1;
                if (regSan == 0) colspan -= 2;

                DateTime expDate = DateTime.Now;

                // Generar detalles de componentes
                if (entryRequests.EntryRequestComponents != null && entryRequests.EntryRequestComponents.Any())
                {
                    if (entryRequests.EntryRequestDetails != null && entryRequests.EntryRequestDetails.Count > 0)
                    {
                        var compAdData = entryRequests.EntryRequestDetails.FirstOrDefault(x => x.IsComponent == true);
                        if (compAdData != null)
                        {
                            detailsHtml += "<tr HEIGHT=\"15\">";
                            detailsHtml += "<td class=\"table_info\"><strong> " + compAdData.IdEquipmentNavigation?.Code + "</strong></td>";
                            detailsHtml += "<td colspan='" + colspan + "' class=\"table_info\"><strong> " + compAdData.IdEquipmentNavigation?.Name + " </strong></td>";
                            detailsHtml += "</tr>";
                        }
                    }

                    detailsHtml += "<tr HEIGHT=\"15\">";
                    detailsHtml += "<th class=\"negrita table_info\">REF</th>";
                    detailsHtml += "<th class=\"negrita table_info\" colspan='2'>Descripción</th>";
                    detailsHtml += "<th style=\" font-size:8px;\"class=\"negrita table_info\">INVIMA</th>";
                    if (lot == 1) detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">LOTE</th>";
                    if (duedate == 1) detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">F.Vencimiento</th>";
                    detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Cant. LT</th>";
                    detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Cant. Total</th>";

                    if (regSan == 1)
                    {
                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">RS Fecha V.</th>";
                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">RS Clas. Riesgo</th>";
                    }

                    if (price == 1)
                    {
                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Precio U.</th>";
                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">IVA</th>";
                    }
                    detailsHtml += "<th style=\" font-size:8px;border-right:1px solid #000000;\" class=\"negrita table_info\">Gasto</th>";
                    detailsHtml += "</tr>";

                    foreach (var CompoD in entryRequests.EntryRequestComponents)
                    {
                        if (!detailsComp.Contains(CompoD.ItemNo))
                        {
                            detailsComp.Add(CompoD.ItemNo);
                            decimal totalQuantityAd = entryRequests.EntryRequestComponents
                                .Where(x => x.ItemNo == CompoD.ItemNo)
                                .Sum(x => (decimal)x.Quantity);

                            detailsHtml += "<tr HEIGHT=\"15\">";
                            if (code == 3)
                                detailsHtml += "<td class=\"table_info\">" + CompoD.shortDesc + "</br></td>";
                            else
                                detailsHtml += "<td class=\"table_info\">" + CompoD.ItemNo + "</br></td>";

                            detailsHtml += "<td class=\"table_info\" style='text-align: left;' colspan='2'><strong>" + CompoD.ItemName + "</strong></td>";
                            detailsHtml += "<td class=\"table_info\">" + CompoD.Invima + "</td>";
                            if (lot == 1) detailsHtml += "<td class=\"table_info\"> </td>";
                            if (duedate == 1) detailsHtml += "<td class=\"table_info\"> </td>";
                            detailsHtml += "<td class=\"table_info\"> </td>";

                            if (regSan == 1)
                            {
                                detailsHtml += "<td class=\"table_info\"> </td>";
                                detailsHtml += "<td class=\"table_info\"> </td>";
                            }

                            detailsHtml += "<td class=\"table_info\"> <strong>" + Convert.ToInt32(totalQuantityAd) + "</strong></td>";
                            if (price == 1)
                            {
                                detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">";
                                if (CompoD.UnitPrice != null)
                                {
                                    decimal UP = (decimal)CompoD.UnitPrice;
                                    detailsHtml += string.Format("{0:n0}", UP);
                                }
                                else
                                    detailsHtml += "0";
                                detailsHtml += "</td>";

                                if (CompoD.UnitPrice != null && CompoD.UnitPrice != 0 && CompoD.TaxCode != "V_ARTVENTAEXC" && (entryRequests.IdCustomerNavigation?.ExIva ?? false))
                                {
                                    decimal IVA = ((decimal)CompoD.UnitPrice) * (decimal)0.19;
                                    detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + string.Format("{0:n0}", IVA) + "</td>";
                                }
                                else
                                {
                                    detailsHtml += "<td class=\"table_info\">0</td>";
                                }
                            }
                            detailsHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'></td>";
                            detailsHtml += "</tr>";

                            foreach (var CompoDdub in entryRequests.EntryRequestComponents)
                            {
                                if (CompoDdub.ItemNo == CompoD.ItemNo && !string.IsNullOrEmpty(CompoDdub.Lot))
                                {
                                    detailsHtml += "<tr HEIGHT=\"15\">";
                                    detailsHtml += "<td class=\"table_info\"> </td>";
                                    detailsHtml += "<td class=\"table_info\" colspan='2'> </td>";
                                    detailsHtml += "<td class=\"table_info\"> </td>";
                                    if (lot == 1) detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + CompoDdub.Lot + "</td>";
                                    if (duedate == 1)
                                    {
                                        if (CompoDdub.ExpirationDate != null)
                                        {
                                            expDate = (DateTime)CompoDdub.ExpirationDate;
                                            detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + expDate.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                                        }
                                        else
                                            detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\"></td>";
                                    }
                                    detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + Convert.ToInt32(CompoDdub.Quantity) + "</td>";
                                    detailsHtml += "<td class=\"table_info\"> </td>";
                                    if (regSan == 1)
                                    {
                                        detailsHtml += "<td class=\"table_info\"> </td>";
                                        detailsHtml += "<td class=\"table_info\"> </td>";
                                    }
                                    if (price == 1)
                                    {
                                        detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\"></td>";
                                        detailsHtml += "<td class=\"table_info\"></td>";
                                    }
                                    detailsHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'></td>";
                                    detailsHtml += "</tr>";
                                }
                            }
                        }
                    }
                }

                // Generar detalles de equipos
                if (entryRequests.EntryRequestDetails != null && entryRequests.EntryRequestDetails.Count() > 0)
                {
                    // Cargar los assemblies para cada detalle si no están cargados
                    try
                    {
                        var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                        var dataAssembly = await companyContext.EntryRequestAssembly
                            .Where(x => x.EntryRequestId == entryRequests.Id)
                            .OrderByDescending(x => x.QuantityConsumed)
                            .ToListAsync();

                        Console.WriteLine($"DEBUG: Assemblies encontrados para EntryRequest {entryRequests.Id}: {dataAssembly?.Count ?? 0}");

                        if (dataAssembly != null && dataAssembly.Any())
                        {
                            // Asignar los assemblies a cada detalle correspondiente
                            foreach (var detail in entryRequests.EntryRequestDetails)
                            {
                                detail.EntryRequestAssembly = dataAssembly
                                    .Where(y => y.EntryRequestDetailId == detail.Id)
                                    .ToList();
                                Console.WriteLine($"DEBUG: Detail ID {detail.Id} - Assemblies asignados: {detail.EntryRequestAssembly?.Count ?? 0}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: No se pudieron cargar los assemblies: {ex.Message}");
                    }

                    int totalBoxes = 0;
                    foreach (var detail in entryRequests.EntryRequestDetails)
                    {
                        if ((option == 1 && detail.DateLoadState >= DateTime.Now.AddMinutes(-10)) || (option == 0 && detail.status == "DESPACHADO"))
                        {
                            if (detail.IsComponent == null || detail.IsComponent == false)
                            {
                                List<string> details = new List<string>();
                                detailsHtml += "<tr HEIGHT=\"15\">";
                                detailsHtml += "<td class=  \"table_info\"><strong>" + detail.IdEquipmentNavigation?.Code + "</strong></td>";
                                detailsHtml += "<td class=  \"table_info\"><strong>" + detail.status + "</strong></td>";
                                detailsHtml += "<td colspan='" + colspan + "' class=\"table_info\"><strong>" + detail.IdEquipmentNavigation?.Name + "</strong></td>";
                                detailsHtml += "<td class=\"table_info\"># Cajas</td>";
                                detailsHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'>" + detail.IdEquipmentNavigation?.NoBoxes + "</td>";
                                detailsHtml += "</tr>";

                                // Debug: Verificar si EntryRequestAssembly está cargado
                                Console.WriteLine($"DEBUG: Detail ID {detail.Id} - EntryRequestAssembly is null: {detail.EntryRequestAssembly == null}");
                                if (detail.EntryRequestAssembly != null)
                                {
                                    Console.WriteLine($"DEBUG: Detail ID {detail.Id} - EntryRequestAssembly count: {detail.EntryRequestAssembly.Count}");
                                }
                                
                                if (detail.EntryRequestAssembly != null && detail.EntryRequestAssembly.Any())
                                {
                                    detailsHtml += "<tr HEIGHT=\"15\">";
                                    detailsHtml += "<th class=\"negrita table_info\">REF</th>";
                                    detailsHtml += "<th class=\"negrita table_info\" colspan='2'>Descripción</th>";
                                    detailsHtml += "<th style=\" font-size:8px;\"class=\"negrita table_info\">INVIMA</th>";
                                    if (lot == 1) detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">LOTE</th>";
                                    if (duedate == 1) detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">F.Vencimiento</th>";
                                    detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Cant. LT</th>";
                                    detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Cant. Total</th>";
                                    if (regSan == 1)
                                    {
                                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">RS Fecha V.</th>";
                                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">RS Clas. Riesgo</th>";
                                    }
                                    if (price == 1)
                                    {
                                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Precio U.</th>";
                                        detailsHtml += "<th style=\" font-size:8px;\" class=\"negrita table_info\">IVA</th>";
                                    }
                                    detailsHtml += "<th style=\"font-size:8px;border-right:1px solid #000000;\" class=\"negrita table_info\">Gasto</th>";
                                    detailsHtml += "</tr>";

                                    var validq = new List<EntryRequestAssembly>();
                                    if (entryRequests.Id > 9597)
                                        validq = detail.EntryRequestAssembly.OrderBy(x => x.Position).ToList();
                                    else
                                        validq = detail.EntryRequestAssembly.OrderBy(x => x.LineNo).ToList();

                                    foreach (var subDetail in validq)
                                    {
                                        decimal totalQuantity = validq.Where(x => x.Code == subDetail.Code && !string.IsNullOrEmpty(x.Lot)).Sum(x => (decimal)x.ReservedQuantity);
                                        if (!details.Contains(subDetail.Code))
                                        {
                                            string color = "none";
                                            details.Add(subDetail.Code);

                                            if (subDetail.LowTurnover ?? false)
                                            {
                                                color = "#CCCCCC";
                                                detailsHtml += "<tr HEIGHT=\"15\">";
                                                if (code == 3)
                                                    detailsHtml += "<td class='table_info bkColor' style='background-color:" + color + ";'>" + subDetail.ShortDesc + "</td>";
                                                else
                                                    detailsHtml += "<td class='table_info bkColor' style='background-color:" + color + ";'>" + subDetail.Code + "</td>";

                                                detailsHtml += "<td class='table_info bkColor' style='text-align: left;background-color:" + color + ";' colspan='2'><strong>" + subDetail.Description + "</strong></td>";
                                            }
                                            else
                                            {
                                                detailsHtml += "<tr HEIGHT=\"15\">";
                                                if (code == 3)
                                                    detailsHtml += "<td class='table_info'>" + subDetail.ShortDesc + "</td>";
                                                else
                                                    detailsHtml += "<td class='table_info'>" + subDetail.Code + "</td>";

                                                detailsHtml += "<td class='table_info' style='text-align: left;' colspan='2'><strong>" + subDetail.Description + "</strong></td>";
                                            }

                                            detailsHtml += "<td class=\"table_info\"></td>";
                                            if (lot == 1) detailsHtml += "<td class=\"table_info\"></td>";
                                            if (duedate == 1) detailsHtml += "<td class=\"table_info\"></td>";
                                            detailsHtml += "<td class=\"table_info\"></td>";
                                            detailsHtml += "<td style=\" font-size:12px;\" class=\"table_info\"><strong>" + Convert.ToInt32(totalQuantity) + "</strong></td>";

                                            if (regSan == 1)
                                            {
                                                if (subDetail.RSFechaVencimiento != null)
                                                {
                                                    expDate = (DateTime)subDetail.RSFechaVencimiento;
                                                    detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + expDate.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                                                }
                                                else
                                                    detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\"></td>";
                                                detailsHtml += "<td class=\"table_info\">" + subDetail.RSClasifRegistro + "</td>";
                                            }

                                            if (price == 1)
                                            {
                                                detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">";
                                                if (subDetail.UnitPrice != null)
                                                {
                                                    decimal UP = (decimal)subDetail.UnitPrice;
                                                    detailsHtml += string.Format("{0:n0}", UP);
                                                }
                                                else
                                                    detailsHtml += "0";
                                                detailsHtml += "</td>";

                                                if (subDetail.UnitPrice != null && subDetail.UnitPrice != 0 && subDetail.TaxCode != "V_ARTVENTAEXC" && (entryRequests.IdCustomerNavigation?.ExIva ?? false))
                                                {
                                                    decimal IVA = ((decimal)subDetail.UnitPrice) * (decimal)0.19;
                                                    detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + string.Format("{0:n0}", IVA) + "</td>";
                                                }
                                                else
                                                {
                                                    detailsHtml += "<td class=\"table_info\">0</td>";
                                                }
                                            }
                                            detailsHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'></td>";
                                            detailsHtml += "</tr>";

                                            foreach (var subSubDetail in validq)
                                            {
                                                if (subSubDetail.Code == subDetail.Code)
                                                {
                                                    if (!string.IsNullOrEmpty(subSubDetail.Lot))
                                                    {
                                                        detailsHtml += "<tr HEIGHT=\"15\">";
                                                        detailsHtml += "<td class=\"table_info\"></td>";
                                                        detailsHtml += "<td class=\"table_info\" colspan='2'></td>";
                                                        detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + subSubDetail.Invima + "</td>";
                                                        if (lot == 1) detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + subSubDetail.Lot + "</td>";
                                                        if (duedate == 1)
                                                        {
                                                            if (subSubDetail.ExpirationDate != null)
                                                            {
                                                                expDate = (DateTime)subSubDetail.ExpirationDate;
                                                                detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + expDate.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                                                            }
                                                            else
                                                                detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\"></td>";
                                                        }
                                                        detailsHtml += "<td style=\" font-size:8px;\" class=\"table_info\">" + Convert.ToInt32(subSubDetail.ReservedQuantity) + "</td>";
                                                        detailsHtml += "<td class=\"table_info\"></td>";
                                                        if (regSan == 1)
                                                        {
                                                            detailsHtml += "<td class=\"table_info\"></td>";
                                                            detailsHtml += "<td class=\"table_info\"></td>";
                                                        }
                                                        if (price == 1)
                                                        {
                                                            detailsHtml += "<td class=\"table_info\"></td>";
                                                            detailsHtml += "<td class=\"table_info\"></td>";
                                                        }
                                                        detailsHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'></td>";
                                                        detailsHtml += "</tr>";
                                                    }

                                                    if (totalQuantity <= 0)
                                                    {
                                                        if (!detailsZero.Contains(detail.IdEquipmentNavigation?.Code))
                                                        {
                                                            detailsZero.Add(detail.IdEquipmentNavigation?.Code ?? "");
                                                            detailsHtmlZero += "<tr HEIGHT=\"15\">";
                                                            detailsHtmlZero += "<td class=\"table_info\"><strong>" + detail.IdEquipmentNavigation?.Code + "</strong></td>";
                                                            detailsHtmlZero += "<td class=\"table_info\">" + detail.status + "</td>";
                                                            detailsHtmlZero += "<td colspan='5' class=\"table_info\" style='text-align: left;border-right:1px solid #000000;'><strong>" + detail.IdEquipmentNavigation?.Name + "</strong></td>";
                                                            detailsHtmlZero += "</tr>";

                                                            detailsHtmlZero += "<tr HEIGHT=\"15\">";
                                                            detailsHtmlZero += "<th class=\"negrita table_info\">REF</th>";
                                                            detailsHtmlZero += "<th class=\"negrita table_info\">Descripción</th>";
                                                            detailsHtmlZero += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Cant.</th>";
                                                            detailsHtmlZero += "<th style=\" font-size:8px;\" class=\"negrita table_info\">Precio U.</th>";
                                                            if (price == 1)
                                                            {
                                                                detailsHtmlZero += "<th style=\" font-size:8px;\" class=\"negrita table_info\">IVA</th>";
                                                                detailsHtmlZero += "<th  style=\" font-size:8px;border-right:1px solid #000000;\" class=\"negrita table_info\">Gasto</th>";
                                                            }
                                                            detailsHtmlZero += "</tr>";
                                                        }
                                                        detailsHtmlZero += "<tr HEIGHT=\"15\">";
                                                        if (code == 3)
                                                            detailsHtmlZero += "<td class=\"table_info\" colspan='2'>" + subDetail.ShortDesc + "</td>";
                                                        else
                                                            detailsHtmlZero += "<td class=\"table_info\">" + subDetail.Code + "</td>";

                                                        detailsHtmlZero += "<td class=\"table_info\" style='text-align: left;'><strong>" + subDetail.Description + "</strong></td>";
                                                        detailsHtmlZero += "<td style=\" font-size:8px;\" class=\"table_info\">0</td>";

                                                        if (price == 1)
                                                        {
                                                            detailsHtmlZero += "<td style=\" font-size:8px;\" class=\"table_info\">";
                                                            if (subDetail.UnitPrice != null)
                                                            {
                                                                decimal UP = (decimal)subDetail.UnitPrice;
                                                                detailsHtmlZero += string.Format("{0:n0}", UP);
                                                            }
                                                            else
                                                                detailsHtmlZero += "0";
                                                            detailsHtmlZero += "</td>";
                                                            if (subDetail.UnitPrice != null && subDetail.UnitPrice != 0 && subDetail.TaxCode != "V_ARTVENTAEXC" && (entryRequests.IdCustomerNavigation?.ExIva ?? false))
                                                            {
                                                                decimal IVA = ((decimal)subDetail.UnitPrice) * (decimal)0.19;
                                                                detailsHtmlZero += "<td style=\" font-size:8px;\" class=\"table_info\">" + string.Format("{0:n0}", IVA) + "</td>";
                                                            }
                                                            else
                                                            {
                                                                detailsHtmlZero += "<td class=\"table_info\">0</td>";
                                                            }
                                                        }

                                                        detailsHtmlZero += "<td class=\"table_info\" style='border-right:1px solid #000000;'></td>";
                                                        detailsHtmlZero += "</tr>";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Agregar los detalles del equipo
                                detailsEqpHtml += "<tr HEIGHT=\"15\">";
                                detailsEqpHtml += "<td class=\"table_info\">" + detail.IdEquipmentNavigation?.Code + "</td>";
                                detailsEqpHtml += "<td class=\"table_info\">" + detail.IdEquipmentNavigation?.Status + "</td>";
                                detailsEqpHtml += "<td class=\"table_info\"><strong>" + detail.IdEquipmentNavigation?.Name + "</strong></td>";
                                detailsEqpHtml += "<td class=\"table_info\">" + detail.IdEquipmentNavigation?.NoBoxes + "</td>";
                                detailsEqpHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'>&nbsp;&nbsp;&nbsp;</td>";
                                totalBoxes += detail.IdEquipmentNavigation?.NoBoxes ?? 0;
                                detailsEqpHtml += "</tr>";
                            }
                        }
                    }
                    detailsEqpHtml += "<tr HEIGHT=\"15\">";
                    detailsEqpHtml += "<td class=\"table_info\" colspan='3'><strong>Total Cajas</strong></td>";
                    detailsEqpHtml += "<td class=\"table_info\">" + totalBoxes.ToString() + "</td>";
                    detailsEqpHtml += "<td class=\"table_info\" style='border-right:1px solid #000000;'>&nbsp;&nbsp;&nbsp;</td>";
                    detailsEqpHtml += "</tr>";
                }

                templateHtml = templateHtml.Replace("#DETAILS#", detailsHtml);
                templateHtml = templateHtml.Replace("#DETAILS_ZERO#", detailsHtmlZero);
                templateHtml = templateHtml.Replace("#DETAILS_EQP#", detailsEqpHtml);
                templateHtml = templateHtml.Replace("#ESTERILIZACION#", "");

                // Mejorar la estructura del footer para centrado
                templateHtml = ImproveFooterHtml(templateHtml);

                Console.WriteLine($"HTML generado exitosamente. Detalles: {detailsHtml.Length} chars, Detalles Zero: {detailsHtmlZero.Length} chars, Detalles EQP: {detailsEqpHtml.Length} chars");
                Console.WriteLine($"HTML final generado con {templateHtml.Length} caracteres");
                
                return templateHtml;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el HTML de la remisión: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera PDF usando iTextSharp como método principal
        /// </summary>
        private async Task<byte[]> GeneratePdfWithITextSharpAsync(string htmlContent, int entryRequestId)
        {
            try
            {
                Console.WriteLine("Generando PDF con iTextSharp como método principal...");
                
                // Validar entrada
                if (string.IsNullOrEmpty(htmlContent))
                {
                    Console.WriteLine("Error: HTML content está vacío");
                    throw new ArgumentException("HTML content no puede estar vacío");
                }
                
                // Usar iTextSharp para generar un PDF real y completo
                using (var ms = new MemoryStream())
                {
                    using (var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30))
                    {
                        var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                        document.Open();
                        
                        // Crear encabezado personalizado con tabla para alineación
                        var headerTable = new iTextSharp.text.pdf.PdfPTable(2);
                        headerTable.WidthPercentage = 100;
                        headerTable.DefaultCell.BorderWidth = 0;
                        headerTable.DefaultCell.Padding = 5f;
                        
                        // Columna izquierda: Logo y nombre de empresa
                        var leftCell = new iTextSharp.text.pdf.PdfPCell();
                        leftCell.BorderWidth = 0;
                        leftCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                        leftCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP;
                        
                        // Agregar logo
                        try
                        {
                            var logoPath = Path.Combine(_env.WebRootPath, "uploads", "logos", "logo_1_20250715022611.png");
                            if (File.Exists(logoPath))
                            {
                                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                                logo.ScaleToFit(80, 60);
                                leftCell.AddElement(logo);
                            }
                            else
                            {
                                Console.WriteLine("Logo no encontrado, continuando sin logo");
                            }
                        }
                        catch (Exception logoEx)
                        {
                            Console.WriteLine($"No se pudo cargar el logo: {logoEx.Message}");
                        }
                        
                        // Agregar nombre de empresa
                        var companyFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD);
                        companyFont.SetColor(0, 51, 102); // Color azul oscuro
                        var companyName = new iTextSharp.text.Paragraph("Suplemédicos", companyFont);
                        companyName.SpacingAfter = 5f;
                        leftCell.AddElement(companyName);
                        
                        // Agregar NIT
                        var nitFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD);
                        var nitLabel = new iTextSharp.text.Paragraph("Nit: ", nitFont);
                        nitLabel.Add(new iTextSharp.text.Chunk("8110417843", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL)));
                        leftCell.AddElement(nitLabel);
                        
                        // Columna derecha: Tipo de documento y número
                        var rightCell = new iTextSharp.text.pdf.PdfPCell();
                        rightCell.BorderWidth = 0;
                        rightCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        rightCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP;
                        
                        var documentFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD);
                        var documentInfo = new iTextSharp.text.Paragraph($"REMISIÓN P-{entryRequestId}", documentFont);
                        documentInfo.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        rightCell.AddElement(documentInfo);
                        
                        // Agregar celdas a la tabla
                        headerTable.AddCell(leftCell);
                        headerTable.AddCell(rightCell);
                        
                        // Agregar la tabla del encabezado
                        document.Add(headerTable);
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio después del encabezado
                        
                        // Agregar línea separadora
                        var line = new iTextSharp.text.pdf.draw.LineSeparator();
                        document.Add(line);
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio
                        
                        // Agregar línea separadora
                        line = new iTextSharp.text.pdf.draw.LineSeparator();
                        document.Add(line);
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio
                        
                        // Procesar el HTML de manera más inteligente para preservar tablas y estructura
                        await ProcessHtmlContentAsync(document, htmlContent);
                        
                        // Agregar pie de página
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio
                        var footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.ITALIC);
                        var footer = new iTextSharp.text.Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss} - Sistema DINMED", footerFont);
                        footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        footer.SpacingBefore = 20f;
                        document.Add(footer);
                        
                        document.Close();
                    }
                    
                    var pdfBytes = ms.ToArray();
                    
                    // Validar que el PDF se generó correctamente
                    if (pdfBytes.Length == 0)
                    {
                        throw new InvalidOperationException("El PDF generado está vacío");
                    }
                    
                    Console.WriteLine($"PDF generado exitosamente con iTextSharp. Tamaño: {pdfBytes.Length} bytes");
                    return pdfBytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GeneratePdfWithITextSharpAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Si falla, usar el método de fallback
                return await GenerateSimplePdfFallbackAsync(htmlContent, entryRequestId);
            }
        }

        /// <summary>
        /// Procesa el contenido HTML preservando la estructura de tablas y formato
        /// </summary>
        private async Task ProcessHtmlContentAsync(iTextSharp.text.Document document, string htmlContent)
        {
            try
            {
                if (string.IsNullOrEmpty(htmlContent))
                {
                    Console.WriteLine("Advertencia: HTML content está vacío, agregando mensaje de error");
                    var errorFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                    var errorPara = new iTextSharp.text.Paragraph("Error: No se pudo generar el contenido HTML", errorFont);
                    document.Add(errorPara);
                    return;
                }

                Console.WriteLine($"Procesando HTML con {htmlContent.Length} caracteres");
                
                var contentFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL);
                var boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD);
                var headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD);
                var smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL);
                
                // Dividir el HTML en secciones basadas en los pagebreak
                var sections = SplitHtmlByPageBreaks(htmlContent);
                Console.WriteLine($"HTML dividido en {sections.Count} secciones por pagebreak");
                
                for (int i = 0; i < sections.Count; i++)
                {
                    var section = sections[i];
                    
                    // Si no es la primera sección, agregar salto de página
                    if (i > 0)
                    {
                        Console.WriteLine($"Agregando salto de página antes de la sección {i + 1}");
                        document.NewPage();
                    }
                    
                    // Procesar cada sección
                    await ProcessHtmlSectionAsync(document, section, contentFont, boldFont, headerFont, smallFont);
                }
                
                Console.WriteLine("HTML procesado exitosamente con el nuevo método");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando HTML: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                // Fallback: agregar contenido como texto plano
                var fallbackFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL);
                var plainText = Regex.Replace(htmlContent, "<[^>]*>", " ");
                var fallbackPara = new iTextSharp.text.Paragraph(plainText, fallbackFont);
                document.Add(fallbackPara);
            }
        }

        /// <summary>
        /// Extrae una sección del HTML entre dos marcadores
        /// </summary>
        private string ExtractSection(string htmlContent, string startMarker, string endMarker)
        {
            try
            {
                var startIndex = htmlContent.IndexOf(startMarker);
                if (startIndex == -1) return string.Empty;
                
                var endIndex = htmlContent.IndexOf(endMarker, startIndex + startMarker.Length);
                if (endIndex == -1) endIndex = htmlContent.Length;
                
                var section = htmlContent.Substring(startIndex, endIndex - startIndex);
                return section;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo sección {startMarker}: {ex.Message}");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Extrae información del cliente de una sección HTML
        /// </summary>
        private Dictionary<string, string> ExtractCustomerInfo(string customerSection)
        {
            var customerInfo = new Dictionary<string, string>();
            try
            {
                // Extraer información usando regex para encontrar los pares label-value
                var pattern = @"<span class=""info-label"">([^<]+)</span>\s*<span class=""info-value"">([^<]*)</span>";
                var matches = Regex.Matches(customerSection, pattern);
                
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 3)
                    {
                        var label = match.Groups[1].Value.Trim();
                        var value = match.Groups[2].Value.Trim();
                        if (!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(value))
                        {
                            customerInfo[label] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo información del cliente: {ex.Message}");
            }
            return customerInfo;
        }
        
        /// <summary>
        /// Extrae información del paciente de una sección HTML
        /// </summary>
        private Dictionary<string, string> ExtractPatientInfo(string patientSection)
        {
            var patientInfo = new Dictionary<string, string>();
            try
            {
                // Extraer información usando regex para encontrar los pares label-value
                var pattern = @"<span class=""info-label"">([^<]+)</span>\s*<span class=""info-value"">([^<]*)</span>";
                var matches = Regex.Matches(patientSection, pattern);
                
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 3)
                    {
                        var label = match.Groups[1].Value.Trim();
                        var value = match.Groups[2].Value.Trim();
                        if (!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(value))
                        {
                            patientInfo[label] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo información del paciente: {ex.Message}");
            }
            return patientInfo;
        }
        
        /// <summary>
        /// Extrae el texto de observaciones de una sección HTML
        /// </summary>
        private string ExtractObservations(string observationsSection)
        {
            try
            {
                // Buscar el contenido dentro de la etiqueta p
                var pattern = @"<p>([^<]*)</p>";
                var match = Regex.Match(observationsSection, pattern);
                if (match.Success && match.Groups.Count >= 2)
                {
                    return match.Groups[1].Value.Trim();
                }
                
                // Si no hay etiqueta p, buscar texto después de OBSERVACIONES
                var textAfterHeader = observationsSection.Substring("OBSERVACIONES".Length);
                var cleanText = Regex.Replace(textAfterHeader, "<[^>]*>", " ").Trim();
                return cleanText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo observaciones: {ex.Message}");
                return "Sin observaciones";
            }
        }
        
        /// <summary>
        /// Procesa una tabla HTML y la convierte a tabla PDF
        /// </summary>
        private async Task ProcessHtmlTableAsync(iTextSharp.text.Document document, string tableHtml, 
            iTextSharp.text.Font contentFont, iTextSharp.text.Font boldFont, iTextSharp.text.Font smallFont)
        {
            try
            {
                Console.WriteLine($"Procesando tabla HTML con {tableHtml.Length} caracteres");
                
                // Extraer filas de la tabla
                var rows = tableHtml.Split(new[] { "<tr", "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"Se encontraron {rows.Length} filas en la tabla");
                
                if (rows.Length == 0)
                {
                    Console.WriteLine("Advertencia: No se encontraron filas en la tabla");
                    return;
                }
                
                // Crear tabla PDF con una columna para mejor presentación
                var pdfTable = new iTextSharp.text.pdf.PdfPTable(1);
                pdfTable.WidthPercentage = 100;
                pdfTable.SpacingBefore = 5f;
                pdfTable.SpacingAfter = 10f;
                pdfTable.DefaultCell.BorderWidth = 0.5f;
                pdfTable.DefaultCell.Padding = 4f;
                
                var processedRows = 0;
                var maxRowsPerPage = 25; // Máximo número de filas por página
                var currentRowCount = 0;
                
                foreach (var row in rows)
                {
                    if (row.Contains("<th") || row.Contains("<td"))
                    {
                        try
                        {
                            // Verificar si necesitamos un salto de página
                            if (currentRowCount > 0 && currentRowCount % maxRowsPerPage == 0)
                            {
                                Console.WriteLine($"Agregando salto de página después de {currentRowCount} filas");
                                document.NewPage();
                                
                                // Agregar encabezado de tabla en la nueva página si es necesario
                                if (processedRows > 0)
                                {
                                    await AddTableHeaderOnNewPage(document, contentFont, boldFont, smallFont);
                                }
                            }
                            
                            // Extraer contenido de las celdas
                            var cellContent = ExtractCellContent(row);
                            if (!string.IsNullOrEmpty(cellContent))
                            {
                                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(cellContent, contentFont));
                                cell.BorderWidth = 0.5f;
                                cell.Padding = 4f;
                                cell.MinimumHeight = 20f;
                                
                                // Detectar si es un encabezado
                                if (row.Contains("<th") || cellContent.Contains("REF") || cellContent.Contains("Descripción") || 
                                    cellContent.Contains("Cant.") || cellContent.Contains("Precio") || cellContent.Contains("Estado") ||
                                    cellContent.Contains("INVIMA") || cellContent.Contains("LOTE") || cellContent.Contains("F.Vencimiento"))
                                {
                                    cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                                    cell.Phrase.Font = boldFont;
                                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                                }
                                else
                                {
                                    // Alinear texto a la izquierda para contenido normal
                                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                                }
                                
                                pdfTable.AddCell(cell);
                                processedRows++;
                                currentRowCount++;
                            }
                        }
                        catch (Exception rowEx)
                        {
                            Console.WriteLine($"Error procesando fila: {rowEx.Message}");
                            // Agregar celda de error
                            var errorCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Error en fila", contentFont));
                            errorCell.BackgroundColor = iTextSharp.text.BaseColor.RED;
                            pdfTable.AddCell(errorCell);
                            currentRowCount++;
                        }
                    }
                }
                
                if (pdfTable.Rows.Count > 0)
                {
                    document.Add(pdfTable);
                    Console.WriteLine($"Tabla PDF agregada con {processedRows} filas procesadas");
                }
                else
                {
                    Console.WriteLine("Advertencia: No se pudo procesar ninguna fila de la tabla");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando tabla: {ex.Message}");
                // Agregar mensaje de error al documento
                var errorFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD);
                var errorPara = new iTextSharp.text.Paragraph("Error procesando tabla", errorFont);
                errorPara.SpacingAfter = 10f;
                document.Add(errorPara);
            }
        }

        /// <summary>
        /// Agrega encabezado de tabla en una nueva página
        /// </summary>
        private async Task AddTableHeaderOnNewPage(iTextSharp.text.Document document, 
            iTextSharp.text.Font contentFont, iTextSharp.text.Font boldFont, iTextSharp.text.Font smallFont)
        {
            try
            {
                // Crear encabezado de tabla para la nueva página
                var headerTable = new iTextSharp.text.pdf.PdfPTable(1);
                headerTable.WidthPercentage = 100;
                headerTable.SpacingBefore = 5f;
                headerTable.SpacingAfter = 5f;
                headerTable.DefaultCell.BorderWidth = 0.5f;
                headerTable.DefaultCell.Padding = 4f;
                
                // Agregar encabezados típicos
                var headers = new[] { "REF", "Descripción", "Cant.", "Precio U.", "Estado" };
                foreach (var header in headers)
                {
                    var headerCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, boldFont));
                    headerCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    headerCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    headerCell.BorderWidth = 0.5f;
                    headerCell.Padding = 4f;
                    headerTable.AddCell(headerCell);
                }
                
                document.Add(headerTable);
                Console.WriteLine("Encabezado de tabla agregado en nueva página");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error agregando encabezado de tabla: {ex.Message}");
            }
        }

        /// <summary>
        /// Divide el HTML en secciones basadas en los elementos pagebreak
        /// </summary>
        private List<string> SplitHtmlByPageBreaks(string htmlContent)
        {
            var sections = new List<string>();
            try
            {
                // Buscar todos los elementos pagebreak
                var pagebreakPattern = @"<div\s+class=""pagebreak"">\s*</div>";
                var matches = Regex.Matches(htmlContent, pagebreakPattern, RegexOptions.IgnoreCase);
                
                Console.WriteLine($"Encontrados {matches.Count} elementos pagebreak en el HTML");
                
                if (matches.Count == 0)
                {
                    // Si no hay pagebreaks, devolver todo el contenido como una sección
                    sections.Add(htmlContent);
                    return sections;
                }
                
                int lastIndex = 0;
                foreach (Match match in matches)
                {
                    // Agregar el contenido antes del pagebreak
                    var section = htmlContent.Substring(lastIndex, match.Index - lastIndex);
                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        sections.Add(section);
                    }
                    lastIndex = match.Index + match.Length;
                }
                
                // Agregar el contenido después del último pagebreak
                if (lastIndex < htmlContent.Length)
                {
                    var lastSection = htmlContent.Substring(lastIndex);
                    if (!string.IsNullOrWhiteSpace(lastSection))
                    {
                        sections.Add(lastSection);
                    }
                }
                
                Console.WriteLine($"HTML dividido en {sections.Count} secciones");
                return sections;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dividiendo HTML por pagebreaks: {ex.Message}");
                // Fallback: devolver todo el contenido como una sección
                sections.Add(htmlContent);
                return sections;
            }
        }

        /// <summary>
        /// Procesa una sección HTML individual
        /// </summary>
        private async Task ProcessHtmlSectionAsync(iTextSharp.text.Document document, string sectionHtml, 
            iTextSharp.text.Font contentFont, iTextSharp.text.Font boldFont, 
            iTextSharp.text.Font headerFont, iTextSharp.text.Font smallFont)
        {
            try
            {
                Console.WriteLine($"Procesando sección HTML con {sectionHtml.Length} caracteres");
                
                // Detectar el tipo de contenido de la sección
                if (sectionHtml.Contains("<table"))
                {
                    // Es una tabla, procesarla directamente
                    await ProcessHtmlTableAsync(document, sectionHtml, contentFont, boldFont, smallFont);
                }
                else if (sectionHtml.Contains("REMISIÓN") || sectionHtml.Contains("#NO#"))
                {
                    // Es un encabezado de remisión, procesar como información general
                    await ProcessRemisionHeaderAsync(document, sectionHtml, contentFont, boldFont, headerFont);
                }
                else
                {
                    // Es contenido de texto general
                    await ProcessHtmlTextAsync(document, sectionHtml, contentFont, boldFont);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando sección HTML: {ex.Message}");
                // Fallback: procesar como texto plano
                await ProcessHtmlTextAsync(document, sectionHtml, contentFont, boldFont);
            }
        }

        /// <summary>
        /// Procesa el encabezado de remisión
        /// </summary>
        private async Task ProcessRemisionHeaderAsync(iTextSharp.text.Document document, string headerHtml, 
            iTextSharp.text.Font contentFont, iTextSharp.text.Font boldFont, iTextSharp.text.Font headerFont)
        {
            try
            {
                Console.WriteLine("Procesando encabezado de remisión");
                
                // Extraer información del cliente y paciente
                var customerInfo = ExtractCustomerInfoFromHeader(headerHtml);
                var patientInfo = ExtractPatientInfoFromHeader(headerHtml);
                
                // Crear tabla de información
                var infoTable = new iTextSharp.text.pdf.PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SpacingBefore = 10f;
                infoTable.SpacingAfter = 10f;
                infoTable.DefaultCell.BorderWidth = 0.5f;
                infoTable.DefaultCell.Padding = 5f;
                
                // Agregar información del cliente
                foreach (var info in customerInfo)
                {
                    var labelCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(info.Key, boldFont));
                    labelCell.BorderWidth = 0.5f;
                    labelCell.Padding = 5f;
                    
                    var valueCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(info.Value, contentFont));
                    valueCell.BorderWidth = 0.5f;
                    valueCell.Padding = 5f;
                    
                    infoTable.AddCell(labelCell);
                    infoTable.AddCell(valueCell);
                }
                
                // Agregar información del paciente
                foreach (var info in patientInfo)
                {
                    var labelCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(info.Key, boldFont));
                    labelCell.BorderWidth = 0.5f;
                    labelCell.Padding = 5f;
                    
                    var valueCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(info.Value, contentFont));
                    valueCell.BorderWidth = 0.5f;
                    valueCell.Padding = 5f;
                    
                    infoTable.AddCell(labelCell);
                    infoTable.AddCell(valueCell);
                }
                
                document.Add(infoTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando encabezado de remisión: {ex.Message}");
                // Fallback: procesar como texto plano
                await ProcessHtmlTextAsync(document, headerHtml, contentFont, boldFont);
            }
        }

        /// <summary>
        /// Extrae información del cliente del encabezado HTML
        /// </summary>
        private Dictionary<string, string> ExtractCustomerInfoFromHeader(string headerHtml)
        {
            var customerInfo = new Dictionary<string, string>();
            try
            {
                // Buscar información del cliente usando patrones específicos
                var patterns = new Dictionary<string, string>
                {
                    { "Cliente Pagador", @"Cliente Pagador[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Nit", @"Nit:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Dirección", @"Dirección:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Teléfono", @"Teléfono:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Ciudad", @"Ciudad:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" }
                };
                
                foreach (var pattern in patterns)
                {
                    var match = Regex.Match(headerHtml, pattern.Value, RegexOptions.IgnoreCase);
                    if (match.Success && match.Groups.Count >= 2)
                    {
                        var value = match.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            customerInfo[pattern.Key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo información del cliente: {ex.Message}");
            }
            return customerInfo;
        }

        /// <summary>
        /// Extrae información del paciente del encabezado HTML
        /// </summary>
        private Dictionary<string, string> ExtractPatientInfoFromHeader(string headerHtml)
        {
            var patientInfo = new Dictionary<string, string>();
            try
            {
                // Buscar información del paciente usando patrones específicos
                var patterns = new Dictionary<string, string>
                {
                    { "Paciente", @"Paciente:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Cédula Paciente", @"Cédula Paciente:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Dir. Entrega", @"Dir\. Entrega:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Nombre solicitante", @"Nombre solicitante:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Doctor", @"Doctor:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Orden de Compra", @"Orden de Compra:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" },
                    { "Historia Clínica", @"Historia Clinica:[^<]*</td>[^<]*<td[^>]*>([^<]*)</td>" }
                };
                
                foreach (var pattern in patterns)
                {
                    var match = Regex.Match(headerHtml, pattern.Value, RegexOptions.IgnoreCase);
                    if (match.Success && match.Groups.Count >= 2)
                    {
                        var value = match.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(value))
                        {
                            patientInfo[pattern.Key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo información del paciente: {ex.Message}");
            }
            return patientInfo;
        }

        /// <summary>
        /// Extrae el contenido de texto de una celda HTML
        /// </summary>
        private string ExtractCellContent(string cellHtml)
        {
            try
            {
                if (string.IsNullOrEmpty(cellHtml))
                    return string.Empty;
                
                // Remover tags HTML y entidades
                var content = Regex.Replace(cellHtml, "<[^>]*>", " ");
                
                // Reemplazar entidades HTML comunes
                content = content.Replace("&nbsp;", " ");
                content = content.Replace("&amp;", "&");
                content = content.Replace("&lt;", "<");
                content = content.Replace("&gt;", ">");
                content = content.Replace("&quot;", "\"");
                content = content.Replace("&#39;", "'");
                
                // Remover atributos HTML comunes que pueden quedar
                content = content.Replace("HEIGHT=\"15\"", "");
                content = content.Replace("class=\"table_info\"", "");
                content = content.Replace("class=\"negrita table_info\"", "");
                content = content.Replace("class=\"table_info bkColor\"", "");
                content = content.Replace("colspan='", "");
                content = content.Replace("colspan=\"", "");
                content = content.Replace("'", "");
                content = content.Replace("\"", "");
                content = content.Replace("style=", "");
                content = content.Replace("background-color:", "");
                content = content.Replace("text-align:", "");
                content = content.Replace("left", "");
                content = content.Replace("center", "");
                content = content.Replace("right", "");
                content = content.Replace(";", "");
                content = content.Replace(":", "");
                
                // Limpiar espacios extra y caracteres especiales
                content = Regex.Replace(content, "\\s+", " ");
                content = content.Replace("  ", " ");
                content = content.Replace(" ,", ",");
                content = content.Replace(", ", ",");
                content = content.Replace(" .", ".");
                content = content.Replace(". ", ".");
                
                var result = content.Trim();
                
                // Log para debugging
                if (result.Length > 0 && result.Length < 100)
                {
                    Console.WriteLine($"Contenido extraído: '{result}'");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extrayendo contenido de celda: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Procesa texto HTML normal
        /// </summary>
        private async Task ProcessHtmlTextAsync(iTextSharp.text.Document document, string textHtml, 
            iTextSharp.text.Font contentFont, iTextSharp.text.Font boldFont)
        {
            try
            {
                // Limpiar HTML y convertir a texto
                var cleanText = Regex.Replace(textHtml, "<[^>]*>", " ");
                cleanText = cleanText.Replace("&nbsp;", " ");
                cleanText = cleanText.Replace("&amp;", "&");
                cleanText = cleanText.Replace("&lt;", "<");
                cleanText = cleanText.Replace("&gt;", ">");
                
                // Dividir en líneas
                var lines = cleanText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.Length > 2)
                    {
                        // Detectar si es información importante
                        if (trimmedLine.Contains("Cliente:") || trimmedLine.Contains("Dirección:") || 
                            trimmedLine.Contains("Paciente:") || trimmedLine.Contains("Médico:"))
                        {
                            var boldPara = new iTextSharp.text.Paragraph(trimmedLine, boldFont);
                            boldPara.SpacingAfter = 2f;
                            document.Add(boldPara);
                        }
                        else
                        {
                            var para = new iTextSharp.text.Paragraph(trimmedLine, contentFont);
                            para.SpacingAfter = 2f;
                            document.Add(para);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando texto: {ex.Message}");
            }
        }

        /// <summary>
        /// Método de fallback para generar PDF cuando DinkToPdf falla
        /// </summary>
        private async Task<byte[]> GenerateSimplePdfFallbackAsync(string htmlContent, int entryRequestId)
        {
            try
            {
                Console.WriteLine("Generando PDF con método de fallback usando iTextSharp...");
                
                // Usar iTextSharp para generar un PDF real
                using (var ms = new MemoryStream())
                {
                    using (var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30))
                    {
                        var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                        document.Open();
                        
                        // Crear encabezado personalizado con tabla para alineación
                        var headerTable = new iTextSharp.text.pdf.PdfPTable(2);
                        headerTable.WidthPercentage = 100;
                        headerTable.DefaultCell.BorderWidth = 0;
                        headerTable.DefaultCell.Padding = 5f;
                        
                        // Columna izquierda: Logo y nombre de empresa
                        var leftCell = new iTextSharp.text.pdf.PdfPCell();
                        leftCell.BorderWidth = 0;
                        leftCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                        leftCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP;
                        
                        // Agregar logo
                        try
                        {
                            var logoPath = Path.Combine(_env.WebRootPath, "uploads", "logos", "logo_1_20250715022611.png");
                            if (File.Exists(logoPath))
                            {
                                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                                logo.ScaleToFit(80, 60);
                                leftCell.AddElement(logo);
                            }
                        }
                        catch (Exception logoEx)
                        {
                            Console.WriteLine($"No se pudo cargar el logo: {logoEx.Message}");
                        }
                        
                        // Agregar nombre de empresa
                        var companyFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD);
                        companyFont.SetColor(0, 51, 102); // Color azul oscuro
                        var companyName = new iTextSharp.text.Paragraph("Suplemédicos", companyFont);
                        companyName.SpacingAfter = 5f;
                        leftCell.AddElement(companyName);
                        
                        // Agregar NIT
                        var nitFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD);
                        var nitLabel = new iTextSharp.text.Paragraph("Nit: ", nitFont);
                        nitLabel.Add(new iTextSharp.text.Chunk("8110417843", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL)));
                        leftCell.AddElement(nitLabel);
                        
                        // Columna derecha: Tipo de documento y número
                        var rightCell = new iTextSharp.text.pdf.PdfPCell();
                        rightCell.BorderWidth = 0;
                        rightCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        rightCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP;
                        
                        var documentFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD);
                        var documentInfo = new iTextSharp.text.Paragraph($"REMISIÓN P-{entryRequestId}", documentFont);
                        documentInfo.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                        rightCell.AddElement(documentInfo);
                        
                        // Agregar celdas a la tabla
                        headerTable.AddCell(leftCell);
                        headerTable.AddCell(rightCell);
                        
                        // Agregar la tabla del encabezado
                        document.Add(headerTable);
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio después del encabezado
                        
                        // Agregar línea separadora
                        var line = new iTextSharp.text.pdf.draw.LineSeparator();
                        document.Add(line);
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio
                        
                        // Agregar contenido HTML convertido a texto plano
                        var contentFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL);
                        
                        // Convertir HTML a texto plano (remover tags HTML)
                        var plainText = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<[^>]*>", " ");
                        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, "&nbsp;", " ");
                        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, "&amp;", "&");
                        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, "&lt;", "<");
                        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, "&gt;", ">");
                        
                        // Dividir en párrafos para mejor legibilidad
                        var paragraphs = plainText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var paragraph in paragraphs.Take(20)) // Limitar a 20 párrafos para evitar PDFs muy largos
                        {
                            var trimmedParagraph = paragraph.Trim();
                            if (!string.IsNullOrEmpty(trimmedParagraph) && trimmedParagraph.Length > 3)
                            {
                                var para = new iTextSharp.text.Paragraph(trimmedParagraph, contentFont);
                                document.Add(para);
                            }
                        }
                        
                        // Agregar pie de página
                        document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio
                        var footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.ITALIC);
                        var footer = new iTextSharp.text.Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss} - Método alternativo", footerFont);
                        footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        document.Add(footer);
                        
                        document.Close();
                    }
                    
                    var pdfBytes = ms.ToArray();
                    Console.WriteLine($"PDF de fallback generado exitosamente con iTextSharp. Tamaño: {pdfBytes.Length} bytes");
                    return pdfBytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en método de fallback con iTextSharp: {ex.Message}");
                // Si todo falla, retornar un PDF de error básico
                return GenerateErrorPdfAsync(entryRequestId);
            }
        }

        /// <summary>
        /// Agrega CSS específico para saltos de página en DinkToPdf
        /// </summary>
        private string AddPageBreakCssForDinkToPdf(string htmlContent)
        {
            try
            {
                // CSS simplificado para DinkToPdf que evita saltos inesperados
                var pageBreakCss = @"
                    <style>
                        /* Estilos simplificados para el footer */
                        .page-footer {
                            width: 100% !important;
                            position: relative !important;
                            margin-top: 20px !important;
                            margin-bottom: 10px !important;
                            padding: 10px 0 !important;
                            text-align: center !important;
                            font-size: 9px !important;
                            line-height: 1.2 !important;
                            page-break-inside: avoid !important;
                            break-inside: avoid !important;
                            background: white !important;
                        }

                        .page-footer-content {
                            margin: 0 !important;
                            padding: 0 !important;
                            text-align: center !important;
                            font-size: 8pt !important;
                            line-height: 1.0 !important;
                            color: #333 !important;
                        }

                        /* Estilos simplificados para el pagebreak */
                        .pagebreak {
                            page-break-before: always !important;
                            break-before: page !important;
                            margin: 0 !important;
                            padding: 0 !important;
                            height: 0 !important;
                            display: block !important;
                        }
                        
                        /* Tablas */
                        table {
                            page-break-inside: avoid !important;
                            break-inside: avoid !important;
                            width: 100% !important;
                            margin: 0 !important;
                            padding: 0 !important;
                        }
                        
                        tr {
                            page-break-inside: avoid !important;
                            break-inside: avoid !important;
                        }
                        
                        td, th {
                            page-break-inside: avoid !important;
                            break-inside: avoid !important;
                            padding: 2px 4px !important;
                            margin: 0 !important;
                        }

                        /* Contenedor principal */
                        .tabla_principal {
                            margin-bottom: 20px !important;
                        }

                        /* Estilos específicos para impresión */
                        @media print {
                            body {
                                margin: 0 !important;
                                padding: 0 !important;
                            }
                            
                            .page-footer {
                                position: relative !important;
                                margin-top: 20px !important;
                                margin-bottom: 10px !important;
                            }
                        }
                    </style>
                ";
                
                // Insertar el CSS en el head del HTML
                if (htmlContent.Contains("<head>"))
                {
                    htmlContent = htmlContent.Replace("<head>", "<head>" + pageBreakCss);
                }
                else if (htmlContent.Contains("<html>"))
                {
                    htmlContent = htmlContent.Replace("<html>", "<html><head>" + pageBreakCss + "</head>");
                }
                else
                {
                    // Si no hay estructura HTML, envolver en HTML básico
                    htmlContent = "<html><head>" + pageBreakCss + "</head><body>" + htmlContent + "</body></html>";
                }
                
                Console.WriteLine("CSS de saltos de página y footer centrado agregado para DinkToPdf");
                return htmlContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error agregando CSS de saltos de página: {ex.Message}");
                return htmlContent; // Retornar HTML original si falla
            }
        }

        /// <summary>
        /// Verifica si las librerías nativas de DinkToPdf están disponibles
        /// </summary>
        private bool IsDinkToPdfNativeAvailable()
        {
            try
            {
                // Verificar si DinkToPdf está disponible
                if (_converter == null)
                {
                    Console.WriteLine("DinkToPdf converter no está disponible");
                    return false;
                }

                // Verificar si la DLL nativa está disponible
                var dllPaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libwkhtmltox.dll"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wkhtmltox.dll"),
                    Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"),
                    Path.Combine(Directory.GetCurrentDirectory(), "wkhtmltox.dll"),
                    Path.Combine(_env.ContentRootPath, "libwkhtmltox.dll"),
                    Path.Combine(_env.ContentRootPath, "wkhtmltox.dll")
                };

                Console.WriteLine("Buscando librería nativa de DinkToPdf...");
                bool dllFound = false;
                foreach (var dllPath in dllPaths)
                {
                    if (File.Exists(dllPath))
                    {
                        Console.WriteLine($"✓ Librería nativa encontrada en: {dllPath}");
                        dllFound = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"✗ No encontrada en: {dllPath}");
                    }
                }

                if (!dllFound)
                {
                    Console.WriteLine("⚠️ ADVERTENCIA: No se encontró la librería nativa libwkhtmltox.dll");
                    Console.WriteLine("   Coloca el archivo en uno de estos directorios:");
                    foreach (var dllPath in dllPaths)
                    {
                        Console.WriteLine($"   - {Path.GetDirectoryName(dllPath)}");
                    }
                }

                // Intentar una conversión simple para verificar que funciona
                Console.WriteLine("Probando conversión de prueba con DinkToPdf...");
                var testSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                };

                var testObject = new ObjectSettings
                {
                    HtmlContent = "<html><body><h1>Test</h1></body></html>"
                };

                var testPdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = testSettings,
                    Objects = { testObject }
                };

                // Si no hay excepción, las librerías están disponibles
                var result = _converter.Convert(testPdf);
                
                if (result != null && result.Length > 0)
                {
                    Console.WriteLine($"✓ DinkToPdf funcionando correctamente. PDF de prueba generado: {result.Length} bytes");
                    return true;
                }
                else
                {
                    Console.WriteLine("✗ DinkToPdf no pudo generar PDF de prueba");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verificación de DinkToPdf falló: {ex.Message}");
                Console.WriteLine($"Tipo de error: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// Genera un PDF de error cuando todos los métodos fallan
        /// </summary>
        private byte[] GenerateErrorPdfAsync(int entryRequestId)
        {
            try
            {
                // Crear un PDF simple de error
                var document = new System.IO.MemoryStream();
                var writer = new StreamWriter(document);
                
                // Escribir contenido básico del PDF de error
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("1 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Type /Catalog");
                writer.WriteLine("/Pages 2 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
                
                writer.WriteLine("2 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Type /Pages");
                writer.WriteLine("/Kids [3 0 R]");
                writer.WriteLine("/Count 1");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
                
                writer.WriteLine("3 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Type /Page");
                writer.WriteLine("/Parent 2 0 R");
                writer.WriteLine("/MediaBox [0 0 612 792]");
                writer.WriteLine("/Contents 4 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
                
                writer.WriteLine("4 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Length 150");
                writer.WriteLine(">>");
                writer.WriteLine("stream");
                writer.WriteLine("BT");
                writer.WriteLine("/F1 12 Tf");
                writer.WriteLine("72 720 Td");
                writer.WriteLine("(Error al generar PDF) Tj");
                writer.WriteLine("72 700 Td");
                writer.WriteLine($"(Remisión P-{entryRequestId}) Tj");
                writer.WriteLine("72 680 Td");
                writer.WriteLine("(Contacte al administrador) Tj");
                writer.WriteLine("ET");
                writer.WriteLine("endstream");
                writer.WriteLine("endobj");
                
                writer.WriteLine("xref");
                writer.WriteLine("0 5");
                writer.WriteLine("0000000000 65535 f");
                writer.WriteLine("0000000009 00000 n");
                writer.WriteLine("0000000058 00000 n");
                writer.WriteLine("0000000115 00000 n");
                writer.WriteLine("0000000300 00000 n");
                writer.WriteLine("trailer");
                writer.WriteLine("<<");
                writer.WriteLine("/Size 5");
                writer.WriteLine("/Root 1 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("startxref");
                writer.WriteLine("400");
                writer.WriteLine("%%EOF");
                
                writer.Flush();
                document.Position = 0;
                
                return document.ToArray();
            }
            catch
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Mejora el HTML del footer para asegurar que se centre correctamente
        /// </summary>
        private string ImproveFooterHtml(string htmlContent)
        {
            try
            {
                Console.WriteLine("Mejorando estructura del footer para centrado...");
                
                // Patrón para encontrar el contenido del footer
                var footerPattern = @"<div class=""page-footer-content""><br /> <br /> <br /> <br />MEDELLÍN: Cll 66A N\. 43 - 02 Oficina 107 Centro Empresarial La Esmeralda\. Itagüí Antioquia PBX \(604\) 3730700 Servicio al cliente: Telefono de Servicio 24 Horas: \(604\) 6049931 BOGOTA: \(601\) Calle 63 # 74B -42 Local 11\. Telefono de Servicio 24 Horas: \(601\) 7440090<br /> CALI: AVENIDA ROOSEVELT # 25 - 32\.Telefono de Servicio 24 Horas: \(602\) 4850210 <br />BARRANQUILLA:  Telefono de Servicio 24 Horas: \(602\)4850210 EMAIL servicioalcliente@diverquin\.com\.co <br /> Somos una Empresa comprometida con el cuidado del medio Ambiente\.</div>";
                
                // HTML mejorado del footer
                var improvedFooter = @"<div class=""page-footer-content"">
                <div style=""text-align: center; width: 100%; margin: 0; padding: 0;"">
                    <br /><br /><br /><br />
                    <div style=""text-align: center; margin: 2px 0;"">MEDELLÍN: Cll 66A N. 43 - 02 Oficina 107 Centro Empresarial La Esmeralda. Itagüí Antioquia PBX (604) 3730700 Servicio al cliente: Telefono de Servicio 24 Horas: (604) 6049931</div>
                    <div style=""text-align: center; margin: 2px 0;"">BOGOTA: (601) Calle 63 # 74B -42 Local 11. Telefono de Servicio 24 Horas: (601) 7440090</div>
                    <div style=""text-align: center; margin: 2px 0;"">CALI: AVENIDA ROOSEVELT # 25 - 32. Telefono de Servicio 24 Horas: (602) 4850210</div>
                    <div style=""text-align: center; margin: 2px 0;"">BARRANQUILLA: Telefono de Servicio 24 Horas: (602)4850210 EMAIL servicioalcliente@diverquin.com.co</div>
                    <div style=""text-align: center; margin: 2px 0;"">Somos una Empresa comprometida con el cuidado del medio Ambiente.</div>
                </div>
            </div>";
                
                // Reemplazar todas las instancias del footer
                var improvedHtml = Regex.Replace(htmlContent, footerPattern, improvedFooter, RegexOptions.IgnoreCase);
                
                Console.WriteLine("Estructura del footer mejorada para centrado");
                return improvedHtml;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mejorando el footer: {ex.Message}");
                return htmlContent; // Retornar HTML original si falla
            }
        }

        /// <summary>
        /// Genera el HTML de remisión optimizado para impresión en navegador
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
        public async Task<string> GenerateRemisionHtmlForPrintAsync(EntryRequests entryRequests, string companyCode, int lot, int price, int code, int duedate, int option, int regSan)
        {
            try
            {
                Console.WriteLine($"Generando HTML optimizado para impresión de remisión P-{entryRequests.Id}");
                
                // Generar el HTML base
                string htmlContent = await GenerateRemisionHtmlAsync(entryRequests, companyCode, lot, price, code, duedate, option, regSan);
                
                // Optimizar el HTML para impresión en navegador
                string optimizedHtml = OptimizeHtmlForPrint(htmlContent);
                
                Console.WriteLine($"HTML optimizado para impresión generado exitosamente. Longitud: {optimizedHtml.Length} caracteres");
                
                return optimizedHtml;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando HTML para impresión: {ex.Message}");
                throw new Exception($"Error al generar el HTML de remisión para impresión: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Optimiza el HTML para impresión en navegador
        /// </summary>
        private string OptimizeHtmlForPrint(string htmlContent)
        {
            try
            {
                Console.WriteLine("Optimizando HTML para impresión en navegador...");
                
                // CSS específico para impresión en navegador
                var printCss = @"
                    <style>
                        /* Estilos generales para impresión */
                        @media print {
                            body {
                                margin: 0;
                                padding: 20px;
                                font-family: Arial, sans-serif;
                                font-size: 12px;
                                line-height: 1.4;
                                color: #000;
                                background: white;
                            }
                            
                            /* Ocultar elementos no necesarios para impresión */
                            .no-print {
                                display: none !important;
                            }
                            
                            /* Asegurar que las tablas se impriman correctamente */
                            table {
                                width: 100%;
                                border-collapse: collapse;
                                page-break-inside: avoid;
                                margin-bottom: 10px;
                            }
                            
                            th, td {
                                border: 1px solid #000;
                                padding: 4px 6px;
                                text-align: left;
                                vertical-align: top;
                                font-size: 10px;
                            }
                            
                            th {
                                background-color: #f0f0f0 !important;
                                font-weight: bold;
                                text-align: center;
                            }
                            
                            /* Asegurar que las imágenes se impriman */
                            img {
                                max-width: 100%;
                                height: auto;
                            }
                            
                            /* Estilos para el encabezado */
                            .header {
                                text-align: center;
                                margin-bottom: 20px;
                                page-break-after: avoid;
                            }
                            
                            .header img {
                                max-height: 60px;
                                margin-bottom: 10px;
                            }
                            
                            /* Estilos para el footer */
                            .page-footer {
                                position: relative;
                                margin-top: 15px;
                                margin-bottom: 5px;
                                text-align: center;
                                font-size: 8px;
                                line-height: 1.1;
                                page-break-inside: avoid;
                            }
                            
                            /* Saltos de página simplificados */
                            .pagebreak {
                                page-break-before: always;
                                margin: 0;
                                padding: 0;
                                height: 0;
                            }
                            
                            /* Evitar saltos de página en elementos importantes */
                            .no-break {
                                page-break-inside: avoid;
                            }
                            
                            /* Estilos para información del cliente y paciente */
                            .info-section {
                                margin-bottom: 15px;
                                page-break-inside: avoid;
                            }
                            
                            .info-table {
                                width: 100%;
                                border: none;
                            }
                            
                            .info-table td {
                                border: none;
                                padding: 2px 4px;
                                vertical-align: top;
                            }
                            
                            .info-label {
                                font-weight: bold;
                                width: 30%;
                            }
                            
                            .info-value {
                                width: 70%;
                            }
                            
                            /* Estilos para observaciones */
                            .observations {
                                margin: 10px 0;
                                padding: 10px;
                                border: 1px solid #ccc;
                                background-color: #f9f9f9;
                                page-break-inside: avoid;
                            }
                            
                            /* Botón de impresión (solo visible en pantalla) */
                            .print-button {
                                position: fixed;
                                top: 20px;
                                right: 20px;
                                background: #007bff;
                                color: white;
                                border: none;
                                padding: 10px 20px;
                                border-radius: 5px;
                                cursor: pointer;
                                font-size: 14px;
                                z-index: 1000;
                            }
                            
                            .print-button:hover {
                                background: #0056b3;
                            }
                            
                            @media print {
                                .print-button {
                                    display: none;
                                }
                            }
                        }
                        
                        /* Estilos para pantalla */
                        @media screen {
                            body {
                                font-family: Arial, sans-serif;
                                font-size: 14px;
                                line-height: 1.6;
                                margin: 20px;
                                background-color: #f5f5f5;
                            }
                            
                            .print-container {
                                background: white;
                                padding: 30px;
                                margin: 0 auto;
                                max-width: 800px;
                                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                                border-radius: 5px;
                            }
                            
                            .print-button {
                                position: fixed;
                                top: 20px;
                                right: 20px;
                                background: #007bff;
                                color: white;
                                border: none;
                                padding: 10px 20px;
                                border-radius: 5px;
                                cursor: pointer;
                                font-size: 14px;
                                z-index: 1000;
                            }
                            
                            .print-button:hover {
                                background: #0056b3;
                            }
                        }
                    </style>
                ";
                
                // JavaScript para funcionalidad de impresión
                var printScript = @"
                    <script>
                        function printDocument() {
                            window.print();
                        }
                    </script>
                ";
                
                // HTML de instrucciones (removido para impresión limpia)
                var instructionsHtml = "";
                
                // Botón de impresión
                var printButtonHtml = @"
                    <button class='print-button' onclick='printDocument()'>
                        🖨️ Imprimir / Guardar PDF
                    </button>
                ";
                
                // Insertar CSS en el head
                if (htmlContent.Contains("<head>"))
                {
                    htmlContent = htmlContent.Replace("<head>", "<head>" + printCss);
                }
                else if (htmlContent.Contains("<html>"))
                {
                    htmlContent = htmlContent.Replace("<html>", "<html><head>" + printCss + "</head>");
                }
                else
                {
                    htmlContent = "<html><head>" + printCss + "</head><body>" + htmlContent + "</body></html>";
                }
                
                // Insertar JavaScript antes del cierre del body
                if (htmlContent.Contains("</body>"))
                {
                    htmlContent = htmlContent.Replace("</body>", printScript + "</body>");
                }
                else
                {
                    htmlContent += printScript;
                }
                
                // Envolver el contenido en un contenedor para pantalla
                var bodyContent = htmlContent;
                if (htmlContent.Contains("<body>"))
                {
                    var bodyStart = htmlContent.IndexOf("<body>") + 6;
                    var bodyEnd = htmlContent.IndexOf("</body>");
                    if (bodyEnd > bodyStart)
                    {
                        var bodyInner = htmlContent.Substring(bodyStart, bodyEnd - bodyStart);
                        bodyContent = htmlContent.Substring(0, bodyStart) + 
                                    "<div class='print-container'>" +
                                    instructionsHtml +
                                    printButtonHtml +
                                    bodyInner +
                                    "</div>" +
                                    htmlContent.Substring(bodyEnd);
                    }
                }
                else
                {
                    bodyContent = "<div class='print-container'>" +
                                instructionsHtml +
                                printButtonHtml +
                                htmlContent +
                                "</div>";
                }
                
                Console.WriteLine("HTML optimizado para impresión en navegador");
                return bodyContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error optimizando HTML para impresión: {ex.Message}");
                return htmlContent; // Retornar HTML original si falla
            }
        }
    }

    /// <summary>
    /// Excepción personalizada para errores de generación de PDF
    /// </summary>
    public class PdfGenerationException : Exception
    {
        public PdfGenerationException(string message) : base(message) { }
        public PdfGenerationException(string message, Exception innerException) : base(message, innerException) { }
    }
} 