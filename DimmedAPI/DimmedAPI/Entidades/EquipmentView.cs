using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [NotMapped]
    public class EquipmentView
    {
        [Key]
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Sede { get; set; } = string.Empty;
        public string UltimoPedido { get; set; } = string.Empty;
        public DateTime? FechaUltimaCirugia { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public string EstadoTrazabilidad { get; set; } = string.Empty;
        public DateTime? FechaTrazabilidad { get; set; }
        public string Institucion { get; set; } = string.Empty;
        public string DireccionEntrega { get; set; } = string.Empty;
        public string TipoEquipo { get; set; } = string.Empty;
        public string EquipoPrincipal { get; set; } = string.Empty;
        public int? NroCajas { get; set; }
        public string LineaProducto { get; set; } = string.Empty;
        public string Ciclocirugia { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Clasificacion { get; set; } = string.Empty;
        public string Alerta { get; set; } = string.Empty;
    }
} 