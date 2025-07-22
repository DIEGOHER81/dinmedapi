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
    }
} 