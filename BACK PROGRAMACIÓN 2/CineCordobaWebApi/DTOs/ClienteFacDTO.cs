namespace CineCordobaWebApi.DTOs
{
    public class ClienteFacDTO
    {
        public int IdCliente { get; set; } // Añade esta propiedad para que coincida con el modelo
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaNac { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}
