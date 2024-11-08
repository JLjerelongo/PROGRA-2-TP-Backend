using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineCordobaWebApi.Models
{
    public class PeliculaTicketDTO
    {
        public int IdPelicula { get; set; }
        public string TituloPelicula { get; set; }
        public int DuracionMin { get; set; }
        public string DuracionFormato { get; set; }
        public string Clasificacion { get; set; }
        public string Director { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public int TicketCount { get; set; }
        public DateTime? PrimerTicketFecha { get; set; } // Fecha del primer ticket
        public TimeSpan? UltimaHoraTicket { get; set; } // Cambia a TimeSpan para almacenar solo la hora
        public List<string> Generos { get; set; } = new List<string>(); // Nueva propiedad para géneros

        // Agregar las siguientes propiedades:
        public int IdClasificacion { get; set; } // ID de clasificación
        public int IdPais { get; set; } // ID de país
        public int IdDirector { get; set; } // ID de director
        public int IdEstado { get; set; } // ID de estado
    }
}

