using CineCordobaWebApi.Models;
using CineCordobaWebApi.Repositories;
using CineCordobaWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CineCordobaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CineController : ControllerBase
    {
        private readonly ICineService _service;
        public CineController(ICineService cineService)
        {
            _service = cineService;
        }

        //CRUD
        //GET PELICULAS

        [HttpGet("peliculas")]
        public ActionResult<List<PeliculaDTO>> GetAllPeliculas()
        {
            var peliculas = _service.GetAllPeliculas();
            if (peliculas == null || !peliculas.Any())
            {
                return NotFound("No se encontraron películas.");
            }
            return Ok(peliculas);
        }

        // Método GET para obtener una película por ID
        [HttpGet("peliculas/{id}")]
        public async Task<ActionResult<Pelicula>> GetPeliculaById(int id)
        {
            var pelicula = await _service.ObtenerPeliculaPorIdAsync(id);

            if (pelicula == null)
            {
                return NotFound(); // Si no se encuentra la película, devuelve 404
            }

            return Ok(pelicula); // Devuelve la película con los detalles
        }



        // Endpoint para obtener todos los géneros
        [HttpGet("generos")]
        public ActionResult<List<Genero>> GetAllGeneros()
        {
            var generos = _service.GetAllGeneross();
            if (generos == null || !generos.Any())
            {
                return NotFound("No se encontraron géneros.");
            }
            return Ok(generos);
        }


        // Endpoint para obtener todas las clasificaciones
        [HttpGet("clasificaciones")]
        public ActionResult<List<Clasificacione>> GetAllClasificaciones()
        {
            var clasificaciones = _service.GetAllClasificaciones(); // Suponiendo que ya tienes este método en el servicio
            if (clasificaciones == null || !clasificaciones.Any())
            {
                return NotFound("No se encontraron clasificaciones.");
            }
            return Ok(clasificaciones);
        }

        // Endpoint para obtener todos los países
        [HttpGet("paises")]
        public ActionResult<List<Paise>> GetAllPaises()
        {
            var paises = _service.GetAllPaises(); // Suponiendo que ya tienes este método en el servicio
            if (paises == null || !paises.Any())
            {
                return NotFound("No se encontraron países.");
            }
            return Ok(paises);
        }

        // Endpoint para obtener todos los directores
        [HttpGet("directores")]
        public ActionResult<List<Directore>> GetAllDirectores()
        {
            var directores = _service.GetAllDirectores(); // Suponiendo que ya tienes este método en el servicio
            if (directores == null || !directores.Any())
            {
                return NotFound("No se encontraron directores.");
            }
            return Ok(directores);
        }

        // Endpoint para obtener todos los estados
        [HttpGet("estados")]
        public ActionResult<List<Directore>> GetAllEstados()
        {
            var estados = _service.GetAllEstados(); // Suponiendo que ya tienes este método en el servicio
            if (estados == null || !estados.Any())
            {
                return NotFound("No se encontraron estados.");
            }
            return Ok(estados);
        }




        // POST de Películas
        [HttpPost]
        public IActionResult CreatePelicula([FromBody] PeliculasGenerosDTO peliculasDTO)
        {
            var validationResult = ValidarPelicula(peliculasDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = validationResult.ErrorMessage });  // Responde con un objeto JSON
            }

            try
            {
                bool resultado = _service.CreatePelicula(peliculasDTO);  // Intenta crear la película
                if (resultado)
                {
                    return Ok(new { message = "Película creada con éxito." });  // Responde con un objeto JSON
                }
                else
                {
                    return BadRequest(new { message = "No se pudo crear la película." });  // Responde con un objeto JSON
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });  // Captura cualquier error en el servidor y lo muestra
            }
        }



        //DELETE ("BAJA LÓGICA PELICULA")
        [HttpDelete("RETIRAR PELICULA/{idPelicula}")]
        public IActionResult DeleteOrden(int idPelicula)
        {
            try
            {
                if (_service.UpdateEstadoPelicula(idPelicula))
                {
                    return Ok($"Pelicula con id: {idPelicula}, retirada!!");
                }
                else
                {
                    return NotFound("Pelicula no encontrada o en un estado incorrecto!!");
                }
            }
            catch (Exception)
            {

                return StatusCode(500, "Error Interno.");
            }
        }

        //VALIDACION DE POST
        private (bool IsValid, string ErrorMessage) ValidarPelicula(PeliculasGenerosDTO peliculasDTO)
        {
            if (string.IsNullOrWhiteSpace(peliculasDTO.TituloPelicula))
            {
                return (false, "El título de la película es obligatorio.");
            }

            if (peliculasDTO.DuracionMin == null || peliculasDTO.DuracionMin <= 0)
            {
                return (false, "La duración de la película debe ser mayor a 0.");
            }

            if (peliculasDTO.IdClasificacion == null || peliculasDTO.IdClasificacion <= 0)
            {
                return (false, "Debe seleccionar una clasificación válida.");
            }

            if (peliculasDTO.IdPais == null || peliculasDTO.IdPais <= 0)
            {
                return (false, "Debe seleccionar un país válido.");
            }


            if (peliculasDTO.IdEstado == null || peliculasDTO.IdEstado <= 0)
            {
                return (false, "Debe seleccionar un estado válido.");
            }


            if (string.IsNullOrWhiteSpace(peliculasDTO.NombreDirector))
            {
                return (false, "El nombre del director es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(peliculasDTO.ApellidoDirector))
            {
                return (false, "El apellido del director es obligatorio.");
            }

            if (peliculasDTO.GenerosSeleccionados == null || !peliculasDTO.GenerosSeleccionados.Any())
            {
                return (false, "Debe seleccionar al menos un género.");
            }

            return (true, string.Empty);
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = _service.Login(loginRequest.Username, loginRequest.Password);
            if (usuario == null)
            {
                return Unauthorized(new { success = false, message = "Usuario o contraseña incorrectos" });
            }

            return Ok(new
            {
                success = true,
                username = usuario.Username,
                role = usuario.IdRolNavigation.RolNombre // Aquí accedemos al nombre del rol
            });
        }


        //METODO PARA EDITAR UNA PELICULA

        [HttpPut("peliculas/{id}")]
        public async Task<IActionResult> UpdatePelicula(int id, [FromBody] PeliculaTicketDTO peliculaActualizada)
        {
            if (id != peliculaActualizada.IdPelicula)
            {
                return BadRequest("El ID de la película no coincide.");
            }

            var resultado = await _service.UpdatePelicula(peliculaActualizada);

            if (resultado)
            {
                return Ok(new { message = "Película actualizada exitosamente." });
            }
            else
            {
                return NotFound("No se pudo encontrar la película para actualizar.");
            }
        }





    }
}
