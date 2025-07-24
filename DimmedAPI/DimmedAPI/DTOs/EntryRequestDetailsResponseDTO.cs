using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestDetailsResponseDTO
    {
        public int Id { get; set; }
        public int IdEntryReq { get; set; }
        public int IdEquipment { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DateIni { get; set; }
        public DateTime DateEnd { get; set; }
        public string status { get; set; }
        public DateTime? DateLoadState { get; set; }
        public string TraceState { get; set; }
        public bool? IsComponent { get; set; }
        public int? UserIdTraceState { get; set; }
        public string sInformation { get; set; }
        public string Name { get; set; }
        public string? EquipmentName { get; set; }
    }
} 