using CineCordobaWebApi.Models;
using CineCordobaWebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Data;
using CineCordobaWebApi.DTOs;

namespace CineCordobaWebApi.Services
{
    public class CineService : ICineService
    {
        private readonly ICineRepository _cineRepository;

        public CineService(ICineRepository cineRepository)
        {
            _cineRepository = cineRepository;
        }

        //METODO PARA OBTENER TODAS LAS PELICULAS Y SUS GENEROS
        public List<PeliculaTicketDTO> GetAllPeliculas()
        {
            // Llama al repositorio para obtener todas las películas con sus géneros y otros atributos
            return _cineRepository.GetAllPeliculas();
        }


        // Implementación del método para obtener una película por ID
        public async Task<PeliculaTicketDTO> ObtenerPeliculaPorIdAsync(int id)
        {
            return await _cineRepository.GetPeliculaByIdAsync(id);
        }


        public bool UpdateEstadoPelicula(int idPelicula)
        {
            return _cineRepository.Equals(idPelicula);
        }

        //jere
        public List<Genero> GetAllGeneross()
        {
            return _cineRepository.GetAllGeneross();
        }

        public bool CreatePelicula(PeliculasGenerosDTO peliculasDTO)
        {
            return _cineRepository.CreatePelicula(peliculasDTO);
        }


        public List<Clasificacione> GetAllClasificaciones()
        {
            return _cineRepository.GetAllClasificaciones(); // Llamar al repositorio para obtener las clasificaciones
        }

        public List<Paise> GetAllPaises()
        {
            return _cineRepository.GetAllPaises(); // Llamar al repositorio para obtener los países
        }

        public List<Directore> GetAllDirectores()
        {
            return _cineRepository.GetAllDirectores(); // Llamar al repositorio para obtener los directores
        }

        public List<TiposEstado> GetAllEstados()
        {
            return _cineRepository.GetAllEstados(); // Llamar al repositorio para obtener los directores
        }

        // Implementación del método Login
        public Usuario Login(string username, string password)
        {
            return _cineRepository.AutenticarUsuario(username, password);
        }

        //METODO PARA EDITAR PELICULA
        public async Task<bool> UpdatePelicula(PeliculaTicketDTO peliculaActualizada)
        {
            try
            {
                // Llama al repositorio para actualizar la película
                return await _cineRepository.UpdatePelicula(peliculaActualizada);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false; // Manejo de excepciones
            }
        }

        public async Task<CompraEntradaDto> ComprarEntrada(CompraEntradaDto compraEntradaDto)
        {
            // Validación de datos
            if (string.IsNullOrWhiteSpace(compraEntradaDto.NombreCliente) ||
                string.IsNullOrWhiteSpace(compraEntradaDto.ApellidoCliente) ||
                string.IsNullOrWhiteSpace(compraEntradaDto.DocumentoCliente) ||
                compraEntradaDto.FechaNacimientoCliente == default ||
                string.IsNullOrWhiteSpace(compraEntradaDto.EmailCliente) ||
                string.IsNullOrWhiteSpace(compraEntradaDto.TelefonoCliente) ||
                compraEntradaDto.IdFuncion <= 0 ||
                (compraEntradaDto.IdButacasSala == null || !compraEntradaDto.IdButacasSala.Any()) ||
                compraEntradaDto.IdFormaPago <= 0 ||
                compraEntradaDto.IdBoleteria <= 0)
            {
                throw new ArgumentException("Todos los campos son obligatorios y deben ser válidos.");
            }

            await _cineRepository.BeginTransactionAsync(IsolationLevel.ReadCommitted); // Iniciar la transacción

            try
            {
                // Llama al método del repositorio para realizar la compra
                var resultado = await _cineRepository.RealizarCompra(compraEntradaDto);

                await _cineRepository.CommitAsync(); // Confirmar la transacción

                return resultado; // Devuelve el resultado
            }
            catch (Exception)
            {
                await _cineRepository.RollbackAsync(); // Deshacer la transacción en caso de error
                throw; // Lanzar la excepción para que el controlador la maneje
            }

        }
        public async Task<List<TicketDto>> GetAllTicketsAsync()
{
    return await _cineRepository.GetAllTicketsAsync();
}
        public async Task<FacturaDto> CrearFacturaAsync(ClienteFacDTO cliente, CompraEntradaDto compraEntradaDto)
        {
            return await _cineRepository.CrearFacturaAsync(cliente, compraEntradaDto);
        }

        public async Task<List<FacturaDto>> GetAllFacturasAsync()
        {
            return await _cineRepository.GetAllFacturasAsync();
        }

        public async Task<FacturaDto> GetFacturaByIdAsync(int id)
        {
            return await _cineRepository.GetFacturaByIdAsync(id);
        }

        public async Task<List<DetalleFacturaDto>> GetDetallesFacturaByIdAsync(int nroFactura)
        {
            return await _cineRepository.GetDetallesFacturaByIdAsync(nroFactura);
        }

        public async Task<List<ButacaDto>> GetButacasDisponiblesAsync(int idFuncion)
        {
            return await _cineRepository.GetButacasDisponiblesAsync(idFuncion);
        }
        public async Task<string> GetButacasDescripcionById(int idButaca)
        {
            return await _cineRepository.GetButacasDescripcionById(idButaca);
        }
        public async Task<List<FormaPagoDto>> GetAllFormasPagoAsync()
        {
            return await _cineRepository.GetAllFormasPagoAsync();
        }

        public async Task<List<FuncionDto>> GetFuncionesByPeliculaIdAsync(int idPelicula)
        {
            return await _cineRepository.GetFuncionesByPeliculaIdAsync(idPelicula);
        }
        public async Task<FuncionDto> GetFuncionByIdFuncion(int idFuncion)
        {
            return await _cineRepository.GetFuncionByIdFuncion(idFuncion);
        }

        public async Task<ClienteFacDTO> ObtenerClientePorIdAsync(int idCliente)
        {
            return await _cineRepository.ObtenerClientePorIdAsync(idCliente); // Asegúrate de que el repositorio tenga este método
        }

        public async Task<ClienteUsuarioDto> GetClienteByUsernameAsync(string username)
        {
            return await _cineRepository.GetClienteByUsernameAsync(username);
        }


        //public async Task<List<Cliente>> GetClientePorId()
        //{
        //    return await _cineRepository.GetClientePorId();
        //}
    }
}
