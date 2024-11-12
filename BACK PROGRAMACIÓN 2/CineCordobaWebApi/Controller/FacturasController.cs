using CineCordobaWebApi.Models;
using CineCordobaWebApi.DTOs;
using CineCordobaWebApi.Repositories;
using CineCordobaWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineCordobaWebApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly ICineService _service;
        public FacturasController(ICineService cineService)
        {
            _service = cineService;
        }

        [HttpPost("comprar")]
        public async Task<IActionResult> ComprarEntrada([FromBody] CompraEntradaDto compraEntradaDto)
        {
            if (compraEntradaDto == null)
            {
                return BadRequest("Datos de compra inválidos.");
            }

            try
            {
                var cliente = await _service.ObtenerClientePorIdAsync(compraEntradaDto.IdCliente);

                if (cliente == null)
                {
                    return NotFound("Cliente no encontrado.");
                }

                var facturaDto = await _service.CrearFacturaAsync(cliente, compraEntradaDto);

                return Ok(facturaDto); // Devuelve el DTO de la factura creada
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la compra.");
            }
        }

        [HttpGet("facturas")]
        public async Task<IActionResult> GetAllFacturas()
        {
            try
            {
                var facturas = await _service.GetAllFacturasAsync();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las facturas.");
            }
        }

        [HttpGet("facturas/{id}")]
        public async Task<IActionResult> GetFacturaById(int id)
        {
            try
            {
                var factura = await _service.GetFacturaByIdAsync(id);
                if (factura == null) return NotFound();
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener la factura.");
            }
        }

        [HttpGet("detalles-factura/{nroFactura}")]
        public async Task<IActionResult> GetDetallesFacturaById(int nroFactura)
        {
            try
            {
                var detalles = await _service.GetDetallesFacturaByIdAsync(nroFactura);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener los detalles de la factura.");
            }
        }
        [HttpGet("tickets")]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var tickets = await _service.GetAllTicketsAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener los tickets.");
            }
        }

        [HttpGet("butacas-disponibles/{idFuncion}")]
        public async Task<IActionResult> GetButacasDisponibles(int idFuncion)
        {
            try
            {
                var butacas = await _service.GetButacasDisponiblesAsync(idFuncion);
                return Ok(butacas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las butacas disponibles.");
            }
        }

        [HttpGet("formas-pago")]
        public async Task<IActionResult> GetAllFormasPago()
        {
            try
            {
                var formasPago = await _service.GetAllFormasPagoAsync();
                return Ok(formasPago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las formas de pago.");
            }
        }

        [HttpGet("funciones/{idPelicula}")]
        public async Task<IActionResult> GetFuncionesByPeliculaId(int idPelicula)
        {
            try
            {
                var funciones = await _service.GetFuncionesByPeliculaIdAsync(idPelicula);
                return Ok(funciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las funciones de la película.");
            }
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> ObtenerClientePorIdAsync(int idCliente)
        {
            try
            {
                var cliente = await _service.ObtenerClientePorIdAsync(idCliente);
                return Ok(cliente);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al obtener el cliente.");
            }
        }


        ////Nueva parte
        //[HttpGet("clientes")]
        //public async Task<IActionResult> GetClientePorId()
        //{
        //    try
        //    {
        //        var cliente = await _service.GetClientePorId();
        //        return Ok(cliente);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "Error al obtener los clientes.");
        //    }

        //}

    }
}
