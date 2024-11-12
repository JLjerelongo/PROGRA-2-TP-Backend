using CineCordobaWebApi.DTOs;
using CineCordobaWebApi.Models;
using System.IO;

namespace CineCordobaWebApi.Services
{
    public interface ICineService
    {

        //METODO PARA OBTENER TODAS LAS PELICULAS Y SUS GENEROS
        public List<PeliculaTicketDTO> GetAllPeliculas();


        public bool UpdateEstadoPelicula(int idPelicula);

        // Nuevo método para obtener todos los géneros
        public List<Genero> GetAllGeneross();

        bool CreatePelicula(PeliculasGenerosDTO peliculasDTO);


        List<Clasificacione> GetAllClasificaciones();
        List<Paise> GetAllPaises();
        List<Directore> GetAllDirectores();
        List<TiposEstado> GetAllEstados();

        //metodo para traer todas las peliculas con sus generos

        //traer pelicula por id
        public Task<PeliculaTicketDTO> ObtenerPeliculaPorIdAsync(int id);

        // Método de autenticación
        Usuario Login(string username, string password);

        //METODO PARA EDITAR PELICULA
        Task<bool> UpdatePelicula(PeliculaTicketDTO peliculaActualizada);

        //TRANSACCION
        Task<CompraEntradaDto> ComprarEntrada(CompraEntradaDto compraEntradaDto);
        
        Task<FacturaDto> CrearFacturaAsync(ClienteFacDTO cliente, CompraEntradaDto compraEntradaDto);
        Task<List<TicketDto>> GetAllTicketsAsync();
        Task<List<FacturaDto>> GetAllFacturasAsync();
        Task<FacturaDto> GetFacturaByIdAsync(int id);
        Task<List<DetalleFacturaDto>> GetDetallesFacturaByIdAsync(int nroFactura);
        Task<List<ButacaDto>> GetButacasDisponiblesAsync(int idFuncion);
        Task<string> GetButacasDescripcionById(int idButaca);
        Task<List<FormaPagoDto>> GetAllFormasPagoAsync();
        Task<List<FuncionDto>> GetFuncionesByPeliculaIdAsync(int idPelicula);
        Task<FuncionDto> GetFuncionByIdFuncion(int idFuncion);
        Task<ClienteFacDTO> ObtenerClientePorIdAsync(int idCliente);

        Task<ClienteUsuarioDto> GetClienteByUsernameAsync(string username);

        //Task<List<Cliente>> GetClientePorId(); 
    }
}
