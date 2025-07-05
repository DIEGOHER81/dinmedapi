using System;

namespace DimmedAPI.DTOs
{
    public class EquipmentBCDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Status { get; set; }
        public string ProductLine { get; set; }
        public string Branch { get; set; }
        public string EstimatedTime { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string TechSpec { get; set; }
        public string DestinationBranch { get; set; }
        public DateTime? LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? InitDate { get; set; }
        public string Vendor { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Abc { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; }
        public string SystemId { get; set; }
        public int? NoBoxes { get; set; }
        public DateTime? LastPreventiveMaintenance { get; set; }
        public DateTime? LastMaintenance { get; set; }
        public string Alert { get; set; }
        public string LocationCode { get; set; }
        public string TransferStatus { get; set; }
    }
} 