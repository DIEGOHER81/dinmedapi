using System;

namespace DimmedAPI.DTOs
{
    public class CalendarDispatchResponseDTO
    {
        public int ResourceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateIni { get; set; }
        public DateTime DateEnd { get; set; }
        public string Client { get; set; }
        public string Status { get; set; }
        public string EquipmentStatus { get; set; }
    }
}
