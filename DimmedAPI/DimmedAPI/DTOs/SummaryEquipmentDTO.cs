using System;

namespace DimmedAPI.DTOs
{
    public class SummaryEquipmentDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string? Status { get; set; } // Nuevo campo para el estado
        public string? Alert { get; set; } // Campo para alertas (nullable)
    }
} 