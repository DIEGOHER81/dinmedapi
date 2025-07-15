namespace DimmedAPI.DTOs
{
    public class CompanyUpdateDTO
    {
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
        public string? logoCompany { get; set; }
        public string? instancia { get; set; }
        public string? dominio { get; set; }
        public string? clienteid { get; set; }
        public string? tenantid { get; set; }
        public string? clientsecret { get; set; }
        public string? callbackpath { get; set; }
        public string? correonotificacion { get; set; }
        public string? nombrenotificacion { get; set; }
        public string? pwdnotificacion { get; set; }
        public string? smtpserver { get; set; }
        public string? puertosmtp { get; set; }
        public int? ModifiedBy { get; set; }
    }
} 