namespace CineCordobaWebApi.DTOs
{
    public class DetalleFacturaDto
    {
        public int? IdPelicula { get; set; } 
        public string TituloPelicula { get; set; }
        public decimal? PrecioUnitario { get; set; } 
        public int? CantidadEntradas { get; set; }
    }

}
