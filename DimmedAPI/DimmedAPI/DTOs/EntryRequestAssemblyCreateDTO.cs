using System;
using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class EntryRequestAssemblyCreateDTO
    {
        [Required(ErrorMessage = "El código es requerido")]
        [MaxLength(100, ErrorMessage = "El código no puede exceder 100 caracteres")]
        public string Code { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; }

        [MaxLength(200, ErrorMessage = "La descripción corta no puede exceder 200 caracteres")]
        public string? ShortDesc { get; set; }

        [MaxLength(100, ErrorMessage = "El INVIMA no puede exceder 100 caracteres")]
        public string? Invima { get; set; }

        [MaxLength(100, ErrorMessage = "El lote no puede exceder 100 caracteres")]
        public string? Lot { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a 0")]
        public decimal? UnitPrice { get; set; }

        [Required(ErrorMessage = "El número de ensamble es requerido")]
        [MaxLength(100, ErrorMessage = "El número de ensamble no puede exceder 100 caracteres")]
        public string AssemblyNo { get; set; }

        [Required(ErrorMessage = "El ID de la solicitud de entrada es requerido")]
        public int EntryRequestId { get; set; }

        public int? EntryRequestDetailId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La cantidad consumida debe ser mayor o igual a 0")]
        public decimal? QuantityConsumed { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La cantidad reservada debe ser mayor o igual a 0")]
        public decimal? ReservedQuantity { get; set; }

        [MaxLength(100, ErrorMessage = "El código de ubicación no puede exceder 100 caracteres")]
        public string? Location_Code_ile { get; set; }

        [MaxLength(100, ErrorMessage = "La clasificación no puede exceder 100 caracteres")]
        public string? Classification { get; set; }

        [MaxLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres")]
        public string? Status { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El número de línea debe ser mayor o igual a 0")]
        public int? LineNo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La posición debe ser mayor o igual a 0")]
        public int? Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La cantidad ile debe ser mayor o igual a 0")]
        public decimal? Quantity_ile { get; set; }

        [MaxLength(50, ErrorMessage = "El código de impuesto no puede exceder 50 caracteres")]
        public string? TaxCode { get; set; }

        public bool? LowTurnover { get; set; }

        public bool? IsComponent { get; set; }

        public DateTime? RSFechaVencimiento { get; set; }

        [MaxLength(100, ErrorMessage = "La clasificación de registro no puede exceder 100 caracteres")]
        public string? RSClasifRegistro { get; set; }
    }
}
