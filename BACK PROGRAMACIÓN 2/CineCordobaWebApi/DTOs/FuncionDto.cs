namespace CineCordobaWebApi.DTOs
{
    public class FuncionDto
    {
        public int IdFuncion { get; set; }
        public string TituloPelicula { get; set; }
        public int? SalaId { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? Hora { get; set; }
        public bool? SubtituloId { get; set; }
        public int? LenguajeId { get; set; }

    }
}
