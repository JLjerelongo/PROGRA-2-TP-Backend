namespace CineCordobaWebApi.Models
{
    public class PeliculasGenerosDTO

    {
        public string TituloPelicula { get; set; }
        public int? DuracionMin { get; set; }
        public int? IdClasificacion { get; set; }
        public int? IdPais { get; set; }
        public int? IdEstado { get; set; }
        public string NombreDirector { get; set; }
        public string ApellidoDirector { get; set; }
        public List<int> GenerosSeleccionados { get; set; }
    }
}
