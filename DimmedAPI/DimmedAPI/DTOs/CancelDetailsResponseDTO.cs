namespace DimmedAPI.DTOs
{
    public class CancelDetailsResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? IdRelation { get; set; }
        public string? CancellationReasonDescription { get; set; }
    }
} 