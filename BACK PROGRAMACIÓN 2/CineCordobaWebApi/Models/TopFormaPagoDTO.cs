namespace CineCordobaWebApi.Models
{
    public class TopFormaPagoDTO
    {
        public string NombreFormaPago { get; set; }
        public int CantidadClientes { get; set; }
        public decimal? Total { get; set; }
        public int CantidadTransacciones { get; set; }
        public int CantidadConPromocion { get; set; }

        public TopFormaPagoDTO(string nombre, int cantidadCliente, decimal? total, int cantidadT,int cantidadConProm)
        {
            NombreFormaPago = nombre;
            CantidadClientes = cantidadCliente;
            Total = total;
            CantidadTransacciones = cantidadT;
            CantidadConPromocion = cantidadConProm;
        }

    }
}
