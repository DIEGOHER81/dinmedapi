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
    }
}
