namespace CineCordobaWebApi.Models
{
    public class FuncionDTO
    {
        public int Id { get; set; }
        public int IdPelicula { get; set; }  // Cambiar el nombre a IdPelicula
        public int SalaId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public int SubtituloId { get; set; }
        public int LenguajeId { get; set; }
    }


}
