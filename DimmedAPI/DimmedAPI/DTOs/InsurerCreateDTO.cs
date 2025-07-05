namespace DimmedAPI.DTOs
{
    public class InsurerCreateDTO
    {
        public string Nit { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int? InsurerTypeId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
