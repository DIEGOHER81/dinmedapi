using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("Equipment")]
    public class Equipment
    {
        public Equipment()
        {
            DbMaintenance = new HashSet<DbMaintenance>();
            EntryRequestDetails = new HashSet<EntryRequestDetails>();
            TraceabilityEquipment = new List<TraceabilityEquipment>();
        }

        public int Id { get; set; }
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
        public string SystemIdBC { get; set; }
        public int NoBoxes { get; set; }
        public DateTime LastPreventiveMaintenance { get; set; }
        public DateTime LastMaintenance { get; set; }
        public virtual ICollection<EntryRequestDetails> EntryRequestDetails { get; set; }
        public virtual ICollection<DbMaintenance> DbMaintenance { get; set; }
        public string Alert { get; set; }
        public string LocationCode { get; set; }
        public string TransferStatus { get; set; }
        public List<TraceabilityEquipment> TraceabilityEquipment { get; set; }

        [NotMapped]
        public List<EntryRequestAssembly> entryRequestAssemblies { get; set; }

        [NotMapped]
        public string IDOrder { get; set; }
    }

    public class TraceabilityEquipment
    {
        public int Id { get; set; }
        public string EquipmentCode { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public string TraceState { get; set; }
        public string TraceabilityDate { get; set; }
        public string IdOrder { get; set; }
        public string OrderDate { get; set; }
        public string StatusOrder { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerName { get; set; }
    }
} 