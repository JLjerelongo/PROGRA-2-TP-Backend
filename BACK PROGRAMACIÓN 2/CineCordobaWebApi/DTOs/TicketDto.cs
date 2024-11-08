namespace CineCordobaWebApi.DTOs
{
    public class TicketDto
    {
        public int NroTicket { get; set; }
        public int? IdButacaSala { get; set; }
        public int? IdPelicula { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? Hora { get; set; }
        public decimal? Precio { get; set; }
        public int? NroFactura { get; set; }
        public int? IdFuncion { get; set; }
    }
}
