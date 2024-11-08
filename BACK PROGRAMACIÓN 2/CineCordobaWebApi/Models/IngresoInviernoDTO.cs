using System.Globalization;

namespace CineCordobaWebApi.Models
{
    public class IngresoInviernoDTO
    {
        public string TipoPublico { get; set; }
        public decimal? Total { get; set; }
        
        public IngresoInviernoDTO(string tipoPublico, decimal? total)
        {
            TipoPublico = tipoPublico;
            Total = total;
        }
    }
}
