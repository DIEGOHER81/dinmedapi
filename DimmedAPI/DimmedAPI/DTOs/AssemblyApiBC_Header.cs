using System.Collections.Generic;

namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para el header de la solicitud de ensamble a Business Central
    /// </summary>
    public class AssemblyApiBC_Header
    {
        /// <summary>
        /// Número del documento
        /// </summary>
        public string documentNo { get; set; } = string.Empty;

        /// <summary>
        /// Sucursal
        /// </summary>
        public string branch { get; set; } = string.Empty;

        /// <summary>
        /// Número del artículo
        /// </summary>
        public string itemNo { get; set; } = string.Empty;

        /// <summary>
        /// Líneas del ensamble
        /// </summary>
        public List<AssemblyApiBC_Line> lines { get; set; } = new List<AssemblyApiBC_Line>();
    }

    /// <summary>
    /// DTO para las líneas del ensamble a Business Central
    /// </summary>
    public class AssemblyApiBC_Line
    {
        /// <summary>
        /// Número del artículo
        /// </summary>
        public string itemNo { get; set; } = string.Empty;

        /// <summary>
        /// Número del documento
        /// </summary>
        public string documentNo { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Precio
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Código de ubicación
        /// </summary>
        public string locationCode { get; set; } = string.Empty;

        /// <summary>
        /// Número de lote
        /// </summary>
        public string lotNo { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad de línea
        /// </summary>
        public int quantityLine { get; set; }

        /// <summary>
        /// ID web
        /// </summary>
        public int idWeb { get; set; }
    }
}


