using CineCordobaWebApi.Models;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using CineCordobaWebApi.DTOs;

namespace CineCordobaWebApi.Repositories
{
    public interface ICineRepository
    {
        //METODO PARA OBTENER TODAS LAS PELICULAS Y SUS GENEROS
        public List<PeliculaTicketDTO> GetAllPeliculas();



        //public bool UpdateEstadoPelicula(int idPelicula);

        public List<Genero> GetAllGeneross();

        public bool CreatePelicula(PeliculasGenerosDTO peliculasDTO);

        // Nuevos métodos para obtener clasificaciones, países y directores
        public List<Clasificacione> GetAllClasificaciones();
        public List<Paise> GetAllPaises();
        public List<Directore> GetAllDirectores();
        public List<TiposEstado> GetAllEstados();
        public Task<PeliculaTicketDTO> GetPeliculaByIdAsync(int id);


        // Declaración del método AutenticarUsuario
        Usuario AutenticarUsuario(string username, string password);

        //actualizar peli
        Task<bool> UpdatePelicula(PeliculaTicketDTO peliculaActualizada);

        //transacciones
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitAsync(); // Método para confirmar transacciones
        Task RollbackAsync(); // Método para deshacer transacciones
        Task<FacturaDto> CrearFacturaAsync(ClienteFacDTO cliente, CompraEntradaDto compraEntradaDto);
        Task<List<FacturaDto>> GetAllFacturasAsync();
        Task<FacturaDto> GetFacturaByIdAsync(int id);
        Task<List<DetalleFacturaDto>> GetDetallesFacturaByIdAsync(int nroFactura);
        Task<List<ButacaDto>> GetButacasDisponiblesAsync(int idFuncion);
        Task<List<FormaPagoDto>> GetAllFormasPagoAsync();
        Task<List<FuncionDto>> GetFuncionesByPeliculaIdAsync(int idPelicula);
        Task<CompraEntradaDto> RealizarCompra(CompraEntradaDto compraEntradaDto);
        Task<ClienteFacDTO> GetClientePorIdAsync(int idCliente);
        Task<List<TicketDto>> GetAllTicketsAsync();

    }
}
