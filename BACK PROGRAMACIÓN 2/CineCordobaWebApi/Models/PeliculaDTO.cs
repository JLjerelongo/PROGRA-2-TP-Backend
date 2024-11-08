namespace CineCordobaWebApi.Models
{
    public class PeliculaDTO
    {
        public int IdPelicula { get; set; }
        public string TituloPelicula { get; set; }
        public List<string> Generos { get; set; } = new List<string>();
    }
}
