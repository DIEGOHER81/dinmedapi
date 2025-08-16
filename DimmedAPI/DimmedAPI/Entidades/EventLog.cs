using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    /// <summary>
    /// Entidad para el registro de eventos del sistema
    /// </summary>
    public class EventLog
    {
        /// <summary>
        /// ID único del evento
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descripción del evento
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Información adicional del evento
        /// </summary>
        public string Information { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora del evento
        /// </summary>
        public DateTime DateLoad { get; set; }

        /// <summary>
        /// ID del usuario que generó el evento
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ID del módulo donde ocurrió el evento
        /// </summary>
        public int IdModule { get; set; }

        /// <summary>
        /// Navegación al usuario
        /// </summary>
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}

