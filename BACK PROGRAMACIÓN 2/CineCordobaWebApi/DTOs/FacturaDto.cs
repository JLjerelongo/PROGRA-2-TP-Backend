using CineCordobaWebApi.Models;

namespace CineCordobaWebApi.DTOs
{
    public class FacturaDto
    {
        public int NroFactura { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public List<DetalleFacturaDto> Detalles { get; set; }
    }

}
