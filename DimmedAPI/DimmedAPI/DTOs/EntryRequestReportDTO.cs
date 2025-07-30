using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestReportDTO
    {
        public int? Pedido { get; set; }
        public int? Consumo { get; set; }
        public DateTime? FechaCirugia { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string? Estado { get; set; }
        public string? EstadoTrazabilidad { get; set; }
        public string? Cliente { get; set; }
        public string? Equipos { get; set; }
        public string? DireccionEntrega { get; set; }
        public string? PrioridadEntrega { get; set; }
        public string? Observaciones { get; set; }
        public string? ObservacionesComerciales { get; set; }
        public string? NombrePaciente { get; set; }
        public string? NombreMedico { get; set; }
        public string? NombreAtc { get; set; }
        public string? Sede { get; set; }
        public string? Servicio { get; set; }
        public string? TipodePedido { get; set; }
        public string? Causalesdenocirugia { get; set; }
        public string? Detallescausalesdenocirugia { get; set; }
        public string? Aseguradora { get; set; }
        public string? Tipodeaseguradora { get; set; }
        public string? Solicitante { get; set; }
        public string? LadoExtremidad { get; set; }
        public DateTime? FechaTerminacion { get; set; }
        public bool? EsReposicion { get; set; }
        public bool? Imprimir { get; set; }
        public string? ReporteTrazabilidad { get; set; }
    }
} 