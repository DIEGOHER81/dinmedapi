namespace DimmedAPI.Entidades
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsProcessResponsable { get; set; }
        public string? Guid { get; set; }

        // Relación inversa (opcional)
        public ICollection<IdentificationTypes>? CreatedIdentificationTypes { get; set; }
        public ICollection<IdentificationTypes>? ModifiedIdentificationTypes { get; set; }
    }
}
