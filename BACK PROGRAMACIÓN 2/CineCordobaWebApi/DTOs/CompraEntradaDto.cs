namespace CineCordobaWebApi.DTOs
{
    public class CompraEntradaDto
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string DocumentoCliente { get; set; }
        public DateTime FechaNacimientoCliente { get; set; }
        public string EmailCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string TituloPelicula { get; set; } // Título de la película
        public DateTime FechaFuncion { get; set; } // Fecha de la función
        public TimeSpan HoraFuncion { get; set; } // Hora de la función
        public decimal PrecioTotal { get; set; } // Precio total de la compra
        public List<int> IdButacasSala { get; set; } // ID de la butaca
        public int IdFormaPago { get; set; } // ID de la forma de pago
        public int IdBoleteria { get; set; } // ID de la boletería
        public int IdFuncion { get; set; } // ID de la función
        public int CantidadEntradas { get; set; }
    }
}


