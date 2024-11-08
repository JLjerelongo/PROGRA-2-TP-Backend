namespace CineCordobaWebApi.Models
{
    public class PeliculaConFuncionesDTO
    {
        public PeliculaTicketDTO Pelicula { get; set; }
        public List<FuncionDTO> Funciones { get; set; }
    }
}
