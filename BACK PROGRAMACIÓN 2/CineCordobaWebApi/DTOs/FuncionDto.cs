namespace CineCordobaWebApi.DTOs
{
    public class FuncionDto
    {
        public int IdFuncion { get; set; }
        public string TituloPelicula { get; set; }
        public DateTime? Fecha { get; set; } // Cambiado a DateTime?
        public TimeSpan? Hora { get; set; } // También puede ser nullable si es necesario
    }
}
