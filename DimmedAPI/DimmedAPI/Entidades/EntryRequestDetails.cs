using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestDetails
    {
        public int Id { get; set; }
        public int IdEntryReq { get; set; }
        public int IdEquipment { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DateIni { get; set; }
        public DateTime DateEnd { get; set; }
        public string status { get; set; }
        [ForeignKey("IdEntryReq")]
        public virtual EntryRequests IdEntryReqNavigation { get; set; }
        [ForeignKey("IdEquipment")]
        public virtual Equipment IdEquipmentNavigation { get; set; }

        [NotMapped]
        public string sInformation { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public virtual ICollection<EntryRequestAssembly> EntryRequestAssembly { get; set; }

        public DateTime? DateLoadState { get; set; }
        public string TraceState { get; set; }
        public bool? IsComponent { get; set; }

        [NotMapped]
        public int? UserIdTraceState { get; set; }
    }


    public class DbMaintenance
    {
        public int Id { get; set; }
        public int IdEquipment { get; set; }
        public virtual Equipment IdEquipmentNavigation { get; set; }
    }
} 