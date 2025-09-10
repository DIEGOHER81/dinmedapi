using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para la solicitud de validación de agendamiento de equipos
    /// </summary>
    public class EquipmentSchedulingValidationRequestDTO
    {
        /// <summary>
        /// ID del equipo a validar
        /// </summary>
        [Required(ErrorMessage = "El ID del equipo es requerido")]
        public int IdEquipment { get; set; }

        /// <summary>
        /// Fecha inicial del rango a validar
        /// </summary>
        [Required(ErrorMessage = "La fecha inicial es requerida")]
        public DateTime DateIni { get; set; }

        /// <summary>
        /// Fecha final del rango a validar
        /// </summary>
        [Required(ErrorMessage = "La fecha final es requerida")]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// ID del pedido que se está validando (opcional, para excluirlo de los pedidos relacionados)
        /// </summary>
        public int? IdEntryReq { get; set; }
    }
}
