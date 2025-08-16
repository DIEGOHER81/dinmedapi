using System.Collections.Generic;

namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para la solicitud de actualización de cantidades de un pedido
    /// </summary>
    public class UpdateQuantitysRequestDTO
    {
        /// <summary>
        /// ID del pedido a actualizar
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lista de ensambles con cantidades actualizadas
        /// </summary>
        public List<UpdateQuantitysAssemblyDTO>? EntryRequestAssembly { get; set; }

        /// <summary>
        /// Lista de componentes con cantidades actualizadas
        /// </summary>
        public List<UpdateQuantitysComponentDTO>? EntryRequestComponents { get; set; }
    }

    /// <summary>
    /// DTO para actualizar cantidades de ensambles
    /// </summary>
    public class UpdateQuantitysAssemblyDTO
    {
        /// <summary>
        /// ID del ensamble
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nueva cantidad consumida
        /// </summary>
        public decimal QuantityConsumed { get; set; }
    }

    /// <summary>
    /// DTO para actualizar cantidades de componentes
    /// </summary>
    public class UpdateQuantitysComponentDTO
    {
        /// <summary>
        /// ID del componente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nueva cantidad consumida
        /// </summary>
        public decimal QuantityConsumed { get; set; }
    }
}

