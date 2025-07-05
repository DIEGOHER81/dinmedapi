namespace DimmedAPI.Entidades
{
    public class Companies
    {
        public int Id { get; set; }

        public int IdentificationTypeId { get; set; }
        public required string IdentificationNumber { get; set; }
        public required string BusinessName { get; set; }
        public string? TradeName { get; set; }
        public string? MainAddress { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; }
        public string? LegalRepresentative { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public required string SqlConnectionString { get; set; }

        public required string BCURLWebService { get; set; }
        public required string BCURL { get; set; }
        public required string BCCodigoEmpresa { get; set; }


        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Relaciones de navegación
        public IdentificationTypes? IdentificationType { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public AppUser? ModifiedByUser { get; set; }
    }
}
