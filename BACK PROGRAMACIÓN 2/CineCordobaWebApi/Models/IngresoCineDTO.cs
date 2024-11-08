namespace CineCordobaWebApi.Models
{
    public class IngresoCineDTO
    {
        public decimal? Total { get; set; }
        public string Cuando { get; set; }
        public string ClasificacionIngresos { get; set; }

        public IngresoCineDTO(decimal? total, string cuando, string clasificacionIngresos)
        {
            Total = total;
            Cuando = cuando;
            ClasificacionIngresos = clasificacionIngresos;
        }
    }
}
