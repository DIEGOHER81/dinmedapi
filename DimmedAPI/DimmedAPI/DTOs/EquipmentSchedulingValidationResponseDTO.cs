namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para la respuesta de validación de agendamiento de equipos
    /// </summary>
    public class EquipmentSchedulingValidationResponseDTO
    {
        /// <summary>
        /// Indica si el agendamiento está permitido
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// ID del equipo validado
        /// </summary>
        public int IdEquipment { get; set; }

        /// <summary>
        /// Fecha inicial del rango validado
        /// </summary>
        public DateTime DateIni { get; set; }

        /// <summary>
        /// Fecha final del rango validado
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Fecha y hora de la validación
        /// </summary>
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Lista de pedidos relacionados con el equipo en el rango de fechas
        /// </summary>
        public List<EquipmentSchedulingRelatedOrderDTO> RelatedOrders { get; set; } = new List<EquipmentSchedulingRelatedOrderDTO>();
    }

    /// <summary>
    /// DTO para representar un pedido relacionado con el agendamiento de equipos
    /// </summary>
    public class EquipmentSchedulingRelatedOrderDTO
    {
        /// <summary>
        /// ID del detalle del pedido
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del pedido principal
        /// </summary>
        public int IdEntryReq { get; set; }

        /// <summary>
        /// ID del equipo
        /// </summary>
        public int IdEquipment { get; set; }

        /// <summary>
        /// Fecha de creación del agendamiento
        /// </summary>
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// Fecha inicial del agendamiento
        /// </summary>
        public DateTime DateIni { get; set; }

        /// <summary>
        /// Fecha final del agendamiento
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Estado del agendamiento
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de carga del estado
        /// </summary>
        public DateTime? DateLoadState { get; set; }

        /// <summary>
        /// Estado de trazabilidad
        /// </summary>
        public string TraceState { get; set; } = string.Empty;

        /// <summary>
        /// Indica si es un componente
        /// </summary>
        public bool? IsComponent { get; set; }

        /// <summary>
        /// Información adicional del agendamiento
        /// </summary>
        public string sInformation { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del agendamiento
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del equipo
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// Código del equipo
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// Información del pedido principal
        /// </summary>
        public EquipmentSchedulingOrderInfoDTO? OrderInfo { get; set; }
    }

    /// <summary>
    /// DTO para información básica del pedido principal
    /// </summary>
    public class EquipmentSchedulingOrderInfoDTO
    {
        /// <summary>
        /// ID del pedido
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha del pedido
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Servicio
        /// </summary>
        public string? Service { get; set; }

        /// <summary>
        /// Tipo de pedido
        /// </summary>
        public int IdOrderType { get; set; }

        /// <summary>
        /// Prioridad de entrega
        /// </summary>
        public string? DeliveryPriority { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public int IdCustomer { get; set; }

        /// <summary>
        /// Solicitante
        /// </summary>
        public string? Applicant { get; set; }

        /// <summary>
        /// Fecha de entrega
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Observaciones del pedido
        /// </summary>
        public string? OrderObs { get; set; }

        /// <summary>
        /// Estado del pedido
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Dirección de entrega
        /// </summary>
        public string? DeliveryAddress { get; set; }

        /// <summary>
        /// Orden de compra
        /// </summary>
        public string? PurchaseOrder { get; set; }
    }
}
