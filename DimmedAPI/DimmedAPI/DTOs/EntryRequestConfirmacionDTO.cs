namespace DimmedAPI.DTOs
{
    public class EntryRequestConfirmacionDTO
    {
        public int? IdPedido { get; set; }
        public string? Saludo { get; set; }
        public string? HeaderCorreo { get; set; }
        public string? ListaEquipos { get; set; }
        public string? FooterCorreo { get; set; }
        public string? Destinatario { get; set; }
    }
} 