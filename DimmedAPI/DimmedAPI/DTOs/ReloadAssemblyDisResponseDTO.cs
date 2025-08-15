namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para la respuesta del endpoint reloadAssemblyDis
    /// </summary>
    public class ReloadAssemblyDisResponseDTO
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ID del pedido procesado
        /// </summary>
        public int EntryRequestId { get; set; }

        /// <summary>
        /// Texto con las validaciones encontradas (errores o advertencias)
        /// </summary>
        public string ValidationMessages { get; set; }

        /// <summary>
        /// Fecha y hora de la operación
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}
