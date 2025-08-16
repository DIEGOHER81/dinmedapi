using System.Collections.Generic;

namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para el header de la solicitud a Business Central
    /// </summary>
    public class EntryRequestApiBC_Header
    {
        /// <summary>
        /// Número del documento
        /// </summary>
        public string documentNo { get; set; } = string.Empty;

        /// <summary>
        /// Número del cliente
        /// </summary>
        public string customerNo { get; set; } = string.Empty;

        /// <summary>
        /// Sucursal
        /// </summary>
        public string branch { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de entrega
        /// </summary>
        public string address { get; set; } = string.Empty;

        /// <summary>
        /// Fecha inicial
        /// </summary>
        public string dateIni { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de cirugía
        /// </summary>
        public string dateSrg { get; set; } = string.Empty;

        /// <summary>
        /// Fecha final
        /// </summary>
        public string dateEnd { get; set; } = string.Empty;

        /// <summary>
        /// Identificación del médico
        /// </summary>
        public string idMedic { get; set; } = string.Empty;

        /// <summary>
        /// Identificación del paciente
        /// </summary>
        public string idPatient { get; set; } = string.Empty;

        /// <summary>
        /// Solicitante
        /// </summary>
        public string solicitante { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del ATC
        /// </summary>
        public string atcName { get; set; } = string.Empty;

        /// <summary>
        /// ID del ATC
        /// </summary>
        public string atcId { get; set; } = string.Empty;

        /// <summary>
        /// Número de orden
        /// </summary>
        public string orderNo { get; set; } = string.Empty;

        /// <summary>
        /// Aseguradora
        /// </summary>
        public string insurer { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de aseguradora
        /// </summary>
        public string insurerType { get; set; } = string.Empty;

        /// <summary>
        /// Lista de precios
        /// </summary>
        public string listPrice { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del paciente
        /// </summary>
        public string patientName { get; set; } = string.Empty;

        /// <summary>
        /// Expediente médico del paciente
        /// </summary>
        public string medicalrecordPatient { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del médico
        /// </summary>
        public string medicName { get; set; } = string.Empty;

        /// <summary>
        /// Líneas de venta
        /// </summary>
        public List<EntryRequestApiBC_Line> salesline { get; set; } = new List<EntryRequestApiBC_Line>();

        /// <summary>
        /// Ensambles de venta
        /// </summary>
        public List<EntryRequestApiBC_Assembly> salesassembly { get; set; } = new List<EntryRequestApiBC_Assembly>();
    }

    /// <summary>
    /// DTO para las líneas de venta a Business Central
    /// </summary>
    public class EntryRequestApiBC_Line
    {
        /// <summary>
        /// Número del documento
        /// </summary>
        public string documentNo { get; set; } = string.Empty;

        /// <summary>
        /// Número del artículo
        /// </summary>
        public string itemNo { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Precio
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Número de ensamble
        /// </summary>
        public string assemblyNo { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad consumida
        /// </summary>
        public decimal quantityConsumed { get; set; }

        /// <summary>
        /// Código de ubicación
        /// </summary>
        public string locationCode { get; set; } = string.Empty;

        /// <summary>
        /// Número de línea
        /// </summary>
        public int lineNo { get; set; }

        /// <summary>
        /// ID web
        /// </summary>
        public int idWeb { get; set; }

        /// <summary>
        /// Tipo de fila
        /// </summary>
        public int rowType { get; set; }
    }

    /// <summary>
    /// DTO para los ensambles de venta a Business Central
    /// </summary>
    public class EntryRequestApiBC_Assembly
    {
        /// <summary>
        /// Número del documento
        /// </summary>
        public string documentNo { get; set; } = string.Empty;

        /// <summary>
        /// Número del artículo
        /// </summary>
        public string itemNo { get; set; } = string.Empty;

        /// <summary>
        /// Lote
        /// </summary>
        public string lot { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad consumida
        /// </summary>
        public decimal quantityConsumed { get; set; }

        /// <summary>
        /// Número de línea
        /// </summary>
        public int lineNo { get; set; }

        /// <summary>
        /// Número de ensamble
        /// </summary>
        public string assemblyNo { get; set; } = string.Empty;

        /// <summary>
        /// ID web
        /// </summary>
        public int idWeb { get; set; }
    }
}

