namespace CineCordobaWebApi.Models
{
    public class AsistenciaIngresosClienteDTO
    {
        public string GrupoEdad { get; set; }
        public string NombreCompleto { get; set; }
        public int Edad { get; set; }
        public int TotalAsistencias { get; set; }
        public string GeneroMasVisto { get; set; }
        public decimal? IngresosTotales { get; set; }
        public string ClasificacionIngresos { get; set; }
        public string Frecuencia { get; set; }

        public AsistenciaIngresosClienteDTO(string grupoEdad, string nombreCompleto, int edad, int totalAsistencias,
            string generoMasVisto, decimal? ingresosTotales, string clasificacionIngresos, string frecuencia)
        {
            GrupoEdad = grupoEdad;
            NombreCompleto = nombreCompleto;
            Edad = edad;
            TotalAsistencias = totalAsistencias;
            GeneroMasVisto = generoMasVisto;
            IngresosTotales = ingresosTotales;
            ClasificacionIngresos = clasificacionIngresos;
            Frecuencia = frecuencia;
        }
    }
}
