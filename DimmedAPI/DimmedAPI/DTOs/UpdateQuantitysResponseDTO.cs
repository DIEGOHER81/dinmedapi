namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para la respuesta de actualización de cantidades de un pedido
    /// </summary>
    public class UpdateQuantitysResponseDTO
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Datos adicionales del resultado
        /// </summary>
        public object? Result { get; set; }
    }
}

