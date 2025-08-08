namespace DimmedAPI.DTOs
{
    public class ProductoSincronizadoDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? Status { get; set; }
        public string? ProductLine { get; set; }
        public string? Branch { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Type { get; set; }
        public string? SystemIdBC { get; set; }
    }

    public class SincronizarProductosCRMResponseDTO
    {
        public bool IsSuccess { get; set; }
        public object? Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public DateTime SynchronizedAt { get; set; } = DateTime.UtcNow;
        public int EntryRequestId { get; set; }
        public int? TotalProductsSynchronized { get; set; }
        public List<ProductoSincronizadoDTO>? ProductosSincronizados { get; set; }
        public string? EntryRequestInfo { get; set; }
    }
}

