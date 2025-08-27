using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestComponentsUpdateDTO
    {
        /// <summary>
        /// Código del almacén
        /// </summary>
        public string? Warehouse { get; set; }

        /// <summary>
        /// Fecha de expiración del componente
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Número de lote del componente
        /// </summary>
        public string? Lot { get; set; }
    }
}
