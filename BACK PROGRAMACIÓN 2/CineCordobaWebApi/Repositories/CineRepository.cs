using CineCordobaWebApi.DTOs;
using CineCordobaWebApi.Models;
using CineCordobaWebApi.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineCordobaWebApi.Repositories
{
    public class CineRepository : ICineRepository
    {
        private readonly CineCordobaDBContext _context;
        private IDbContextTransaction _transaction; // Para manejar la transacción

        public CineRepository(CineCordobaDBContext context)
        {
            _context = context;
        }

        //METODO PARA OBTENER TODAS LAS PELICULAS Y SUS GENEROS
        public List<PeliculaTicketDTO> GetAllPeliculas()
        {
            // Primero obtén las películas sin aplicar ConvertirDuracion
            var peliculas = _context.Peliculas
                .Include(p => p.IdClasificacionNavigation)
                .Include(p => p.IdDirectorNavigation)
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.IdPaisNavigation)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.IdGeneroNavigation)
                .Select(p => new
                {
                    p.IdPelicula,
                    p.TituloPelicula,
                    p.DuracionMin,
                    Clasificacion = p.IdClasificacionNavigation.Descripcion,
                    Director = p.IdDirectorNavigation.Nombre + " " + p.IdDirectorNavigation.Apellido,
                    Estado = p.IdEstadoNavigation.Estado,
                    Pais = p.IdPaisNavigation.Pais,
                    TicketCount = p.Tickets.Count(),
                    PrimerTicketFecha = p.Tickets.OrderBy(t => t.Fecha).Select(t => t.Fecha).FirstOrDefault(),
                    UltimaHoraTicket = p.Tickets.OrderByDescending(t => t.Hora).Select(t => t.Hora).FirstOrDefault(),
                    Generos = p.GenerosPeliculas.Select(gp => gp.IdGeneroNavigation.Descripcion).ToList()
                })
                .ToList();

            // Luego convierte DuracionMin a un formato legible en memoria
            var resultado = peliculas.Select(p => new PeliculaTicketDTO
            {
                IdPelicula = p.IdPelicula,
                TituloPelicula = p.TituloPelicula,
                DuracionMin = p.DuracionMin ?? 0,
                DuracionFormato = p.DuracionMin != null ? ConvertirDuracion(p.DuracionMin.Value) : "No disponible",
                Clasificacion = p.Clasificacion,
                Director = p.Director,
                Estado = p.Estado,
                Pais = p.Pais,
                TicketCount = p.TicketCount,
                PrimerTicketFecha = p.PrimerTicketFecha,
                UltimaHoraTicket = p.UltimaHoraTicket,
                Generos = p.Generos
            }).ToList();

            return resultado;
        }

        public List<Genero> GetAllGeneross()
        {
            return _context.Generos.ToList();
        }

        public List<ClienteDTO> GetClientesQueJamasVieronPeliculaPorGenero(string genero)
        {
            var clientes = _context.Clientes
                .Where(c => !_context.Reservas
                    .Join(_context.Funciones, r => r.IdFuncion, f => f.IdFuncion, (r, f) => new { r, f })
                    .Join(_context.GenerosPeliculas, rf => rf.f.IdPelicula, gp => gp.IdPelicula, (rf, gp) => new { rf.r, rf.f, gp })
                    .Join(_context.Generos, rfgp => rfgp.gp.IdGenero, g => g.IdGenero, (rfgp, g) => new { rfgp.r, rfgp.f, rfgp.gp, g })
                    .Any(joined => joined.r.IdCliente == c.IdCliente && joined.g.Descripcion == genero)
                )
                .Select(c => new ClienteDTO
                {
                    NombreCompleto = c.Nombre + " " + c.Apellido,
                    Edad = c.FechaNac.HasValue
                        ? (DateTime.Now.Year - c.FechaNac.Value.Year - ((DateTime.Now.Month < c.FechaNac.Value.Month || (DateTime.Now.Month == c.FechaNac.Value.Month && DateTime.Now.Day < c.FechaNac.Value.Day)) ? 1 : 0))
                        : 0,
                    PuedeVer18 = (c.FechaNac.HasValue && (DateTime.Now.Year - c.FechaNac.Value.Year - ((DateTime.Now.Month < c.FechaNac.Value.Month || (DateTime.Now.Month == c.FechaNac.Value.Month && DateTime.Now.Day < c.FechaNac.Value.Day)) ? 1 : 0)) >= 18) ? "Sí" : "No"
                })
                .ToList();

            return clientes;
        }

        public List<AsistenciaIngresosClienteDTO> GetAsistenciaIngresosPorCliente()
        {
            DataTable tabla = DBHelper.GetInstancia().Consultar("SP_ASISTENCIA_INGRESOS_POR_CLIENTE");
            List<AsistenciaIngresosClienteDTO> lista = new List<AsistenciaIngresosClienteDTO>();

            foreach (DataRow row in tabla.Rows)
            {
                string grupo = row["grupos edad"].ToString();
                string nombre = row["nombre completo"].ToString();
                int edad = int.Parse(row["edad"].ToString());
                int totalAsistencia = int.Parse(row["total asistencias"].ToString());
                string genero = row["genero mas visto"].ToString();
                decimal? total = Convert.ToDecimal(row["ingresos totales"].ToString());
                string clasificacion = row["clasificacion ingresos"].ToString();
                string frecuencia = row["frecuencia"].ToString();


                AsistenciaIngresosClienteDTO a = new AsistenciaIngresosClienteDTO(grupo, nombre, edad, totalAsistencia, genero,
                    total, clasificacion, frecuencia);
                lista.Add(a);
            }

            return lista;
        }

        public List<IngresoInviernoDTO> GetIngresosInvierno()
        {
            string query = @"
            SELECT ipp.[Tipo Publico], SUM(ipp.[Ingresos Totales]) 'INGRESOS TOTALES INVIERNO'
            FROM Ingresos_Por_Publico ipp
            WHERE MONTH(ipp.Fecha_Facturizacion) BETWEEN 6 AND 9
            GROUP BY [Tipo Publico]";


            DataTable tabla = DBHelper.GetInstancia().ConsultarVista(query);
            List<IngresoInviernoDTO> lst = new List<IngresoInviernoDTO>();
            foreach (DataRow row in tabla.Rows)
            {
                string tipoPublico = row[0].ToString();
                decimal? total = Convert.ToDecimal(row[1].ToString());
                IngresoInviernoDTO i = new IngresoInviernoDTO(tipoPublico, total);
                lst.Add(i);
            }
            return lst;
        }

        public List<TopFormaPagoDTO> GetTopFormasPago()
        {
            DataTable tabla = DBHelper.GetInstancia().Consultar("SP_TOP_FORMAS_PAGO");
            List<TopFormaPagoDTO> lista = new List<TopFormaPagoDTO>();

            foreach (DataRow row in tabla.Rows)
            {
                string nombreFP = row["forma pago"].ToString();
                int cantidadCliente = int.Parse(row["clientes unicos"].ToString());
                decimal? total = Convert.ToDecimal(row["total ingresos"].ToString());
                int cantidadTransaccion = int.Parse(row["cantidad transacciones"].ToString());
                int cantidadPromocion = int.Parse(row["transacciones con promocion"].ToString());

                TopFormaPagoDTO tfp = new TopFormaPagoDTO(nombreFP, cantidadCliente, total, cantidadTransaccion, cantidadPromocion);
                lista.Add(tfp);
            }

            return lista;
        }

        public List<IngresoCineDTO> GetTotalesPeriodos()
        {
            string query = @"
            SELECT TOTAL, CUANDO,
            CASE 
                WHEN TOTAL < 10000 THEN 'Bajo'
                WHEN TOTAL BETWEEN 10000 AND 50000 THEN 'Moderado'
                WHEN TOTAL < 50000 THEN 'Alto'
                ELSE 'Sin Ingresos'
            END ClasificacionIngresos
            FROM VISTA_TOTAL_DIA_MES_SEMESTRE_AÑO";

            DataTable tabla = DBHelper.GetInstancia().ConsultarVista(query);
            List<IngresoCineDTO> lst = new List<IngresoCineDTO>();
            foreach (DataRow row in tabla.Rows)
            {

                decimal? total = row["total"] != DBNull.Value ? Convert.ToDecimal(row[0]) : (decimal?)null;
                string cuando = row["cuando"].ToString();
                string clasificacion = row["ClasificacionIngresos"].ToString();
                IngresoCineDTO i = new IngresoCineDTO(total, cuando, clasificacion);
                lst.Add(i);
            }
            return lst;
        }
        //CRUD PELICULAS
       
        private string ConvertirDuracion(int duracionMin)
        {
            int horas = duracionMin / 60;
            int minutos = duracionMin % 60;

            return $"{horas}h {minutos}m";
        }

        /*public bool UpdateEstadoPelicula(int idPelicula)
        {
            List<SqlParameter> listaParam = new List<SqlParameter>();
            listaParam.Add(new SqlParameter("@id_pelicula", idPelicula));
            return DBHelper.GetInstancia().EjecutarSQL("SP_RETIRAR_PELICULA", listaParam) == 1;
        }*/

        
        public bool CreatePelicula(PeliculasGenerosDTO peliculasDTO)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Buscar o crear el director
                var director = _context.Directores
                    .FirstOrDefault(d => d.Nombre == peliculasDTO.NombreDirector && d.Apellido == peliculasDTO.ApellidoDirector);

                if (director == null)
                {
                    director = new Directore
                    {
                        Nombre = peliculasDTO.NombreDirector,
                        Apellido = peliculasDTO.ApellidoDirector
                    };
                    _context.Directores.Add(director);
                    _context.SaveChanges();
                }

                var pelicula = new Pelicula
                {
                    TituloPelicula = peliculasDTO.TituloPelicula,
                    DuracionMin = peliculasDTO.DuracionMin,
                    IdClasificacion = peliculasDTO.IdClasificacion,
                    IdPais = peliculasDTO.IdPais,
                    IdDirector = director.IdDirector,
                    IdEstado = peliculasDTO.IdEstado
                };
                _context.Peliculas.Add(pelicula);
                _context.SaveChanges();

                foreach (var idGenero in peliculasDTO.GenerosSeleccionados)
                {
                    var generoPelicula = new GenerosPelicula
                    {
                        IdPelicula = pelicula.IdPelicula,
                        IdGenero = idGenero
                    };
                    _context.GenerosPeliculas.Add(generoPelicula);
                }
                _context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        // Nuevas implementaciones para obtener Clasificaciones, Paises y Directores
        public List<Clasificacione> GetAllClasificaciones()
        {
            return _context.Clasificaciones.ToList(); // Obtener todas las clasificaciones
        }

        public List<Paise> GetAllPaises()
        {
            return _context.Paises.ToList(); // Obtener todos los países
        }

        public List<Directore> GetAllDirectores()
        {
            return _context.Directores.ToList(); // Obtener todos los directores
        }

        public List<TiposEstado> GetAllEstados()
        {
            return _context.TiposEstados.ToList(); // Obtener todos los directores
        }


        // Método para obtener una película por ID
        public async Task<PeliculaTicketDTO> GetPeliculaByIdAsync(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.IdClasificacionNavigation)
                .Include(p => p.IdDirectorNavigation)
                .Include(p => p.IdEstadoNavigation)
                .Include(p => p.IdPaisNavigation)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.IdGeneroNavigation)
                .Where(p => p.IdPelicula == id)
                .Select(p => new
                {
                    p.IdPelicula,
                    p.TituloPelicula,
                    p.DuracionMin,
                    Clasificacion = p.IdClasificacionNavigation.Descripcion,
                    Director = p.IdDirectorNavigation.Nombre + " " + p.IdDirectorNavigation.Apellido,
                    Estado = p.IdEstadoNavigation.Estado,
                    Pais = p.IdPaisNavigation.Pais,
                    TicketCount = p.Tickets.Count(),
                    PrimerTicketFecha = p.Tickets.OrderBy(t => t.Fecha).Select(t => t.Fecha).FirstOrDefault(),
                    UltimaHoraTicket = p.Tickets.OrderByDescending(t => t.Hora).Select(t => t.Hora).FirstOrDefault(),
                    Generos = p.GenerosPeliculas.Select(gp => gp.IdGeneroNavigation.Descripcion).ToList()
                })
                .FirstOrDefaultAsync();

            if (pelicula == null)
            {
                return null; // O lanzar una excepción si prefieres
            }

            // Convertir la duración a un formato legible
            var peliculaDTO = new PeliculaTicketDTO
            {
                IdPelicula = pelicula.IdPelicula,
                TituloPelicula = pelicula.TituloPelicula,
                DuracionMin = pelicula.DuracionMin ?? 0,
                DuracionFormato = pelicula.DuracionMin != null ? ConvertirDuracion(pelicula.DuracionMin.Value) : "No disponible",
                Clasificacion = pelicula.Clasificacion,
                Director = pelicula.Director,
                Estado = pelicula.Estado,
                Pais = pelicula.Pais,
                TicketCount = pelicula.TicketCount,
                PrimerTicketFecha = pelicula.PrimerTicketFecha,
                UltimaHoraTicket = pelicula.UltimaHoraTicket,
                Generos = pelicula.Generos
            };
            return peliculaDTO;
        }

        public Usuario AutenticarUsuario(string username, string password)
        {
            // Busca al usuario por nombre de usuario y verifica la contraseña
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (usuario != null)
            {
                // Carga el rol del usuario
                _context.Entry(usuario).Reference(u => u.IdRolNavigation).Load();
            }

            return usuario;
        }

        public async Task<bool> UpdatePelicula(PeliculaTicketDTO peliculaActualizada)
        {
            // Encuentra la película existente en la base de datos
            var peliculaExistente = await _context.Peliculas
                .Include(p => p.GenerosPeliculas) // Incluye la relación de géneros
                .FirstOrDefaultAsync(p => p.IdPelicula == peliculaActualizada.IdPelicula);

            if (peliculaExistente == null)
            {
                return false; // Película no encontrada
            }

            // Actualiza los campos de la película
            peliculaExistente.TituloPelicula = peliculaActualizada.TituloPelicula;
            peliculaExistente.DuracionMin = peliculaActualizada.DuracionMin;
            peliculaExistente.IdClasificacion = peliculaActualizada.IdClasificacion;
            peliculaExistente.IdPais = peliculaActualizada.IdPais;
            peliculaExistente.IdDirector = peliculaActualizada.IdDirector;
            peliculaExistente.IdEstado = peliculaActualizada.IdEstado;

            // Limpiar géneros existentes
            peliculaExistente.GenerosPeliculas.Clear();

            // Añadir los nuevos géneros
            foreach (var generoDescripcion in peliculaActualizada.Generos)
            {
                var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Descripcion == generoDescripcion);
                if (genero != null)
                {
                    peliculaExistente.GenerosPeliculas.Add(new GenerosPelicula
                    {
                        IdPelicula = peliculaExistente.IdPelicula,
                        IdGenero = genero.IdGenero
                    });
                }
            }

            await _context.SaveChangesAsync(); // Guardar cambios en la base de datos
            return true;
        }

        public async Task<FacturaDto> CrearFacturaAsync(ClienteFacDTO clienteDto, CompraEntradaDto compraEntradaDto)
        {
            var cliente = new Cliente
            {
                IdCliente = clienteDto.IdCliente,
                Nombre = clienteDto.Nombre,
                Apellido = clienteDto.Apellido,
                Documento = clienteDto.Documento,
                FechaNac = clienteDto.FechaNac,
                Email = clienteDto.Email,
                Telefono = clienteDto.Telefono
            };

            var factura = new Factura
            {
                IdCliente = cliente.IdCliente,
                FechaCompra = DateTime.Now,
                IdFormaPago = compraEntradaDto.IdFormaPago,
                IdBoleteria = compraEntradaDto.IdBoleteria,
            };

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync(); // Guarda la factura primero para obtener el NroFactura

            // Crear el detalle de la factura
            var detalleFactura = new DetallesFactura
            {
                NroFactura = factura.NroFactura,
                IdFuncion = compraEntradaDto.IdFuncion,
                IdPelicula = (await _context.Funciones
                    .Include(f => f.IdPeliculaNavigation)
                    .FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion))?.IdPelicula,
                PrecioUnitario = 1500.00m, // Ajustar el precio según la lógica
                CantEntrada = compraEntradaDto.CantidadEntradas
            };

            _context.DetallesFacturas.Add(detalleFactura);
            await _context.SaveChangesAsync(); // Guarda el detalle de factura

            // Crear un ticket para cada butaca en IdButacasSala
            foreach (var idButaca in compraEntradaDto.IdButacasSala)
            {
                var ticket = new Ticket
                {
                    IdButacaSala = idButaca,
                    IdPelicula = detalleFactura.IdPelicula,
                    Fecha = DateTime.Now,
                    Hora = (await _context.Funciones.FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion)).Hora,
                    Precio = detalleFactura.PrecioUnitario, // Precio por entrada
                    NroFactura = factura.NroFactura,
                    IdFuncion = compraEntradaDto.IdFuncion
                };

                _context.Tickets.Add(ticket);
            }

            await _context.SaveChangesAsync(); // Guarda todos los tickets

            // Convertir a DTO para devolver
            return new FacturaDto
            {
                NroFactura = factura.NroFactura,
                FechaCompra = factura.FechaCompra.Value,
                NombreCliente = cliente.Nombre,
                ApellidoCliente = cliente.Apellido,
                Detalles = new List<DetalleFacturaDto>
        {
            new DetalleFacturaDto
            {
                IdPelicula = detalleFactura.IdPelicula,
                TituloPelicula = (await _context.Peliculas.FirstOrDefaultAsync(p => p.IdPelicula == detalleFactura.IdPelicula))?.TituloPelicula,
                PrecioUnitario = detalleFactura.PrecioUnitario,
                CantidadEntradas = detalleFactura.CantEntrada
            }
        }
            };
        }

        public async Task<List<TicketDto>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Select(t => new TicketDto
                {
                    NroTicket = t.NroTicket,
                    IdButacaSala = t.IdButacaSala,
                    IdPelicula = t.IdPelicula,
                    Fecha = t.Fecha,
                    Hora = t.Hora,
                    Precio = t.Precio,
                    NroFactura = t.NroFactura,
                    IdFuncion = t.IdFuncion
                })
                .ToListAsync();
        }



        public async Task<List<FacturaDto>> GetAllFacturasAsync()
        {
            return await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.DetallesFacturas)
                    .ThenInclude(df => df.IdPeliculaNavigation)
                .Select(f => new FacturaDto
                {
                    NroFactura = f.NroFactura,
                    FechaCompra = f.FechaCompra ?? DateTime.Now,
                    NombreCliente = f.IdClienteNavigation.Nombre,
                    ApellidoCliente = f.IdClienteNavigation.Apellido,
                    Detalles = f.DetallesFacturas.Select(df => new DetalleFacturaDto
                    {
                        IdPelicula = df.IdPelicula,
                        TituloPelicula = df.IdPeliculaNavigation.TituloPelicula,
                        PrecioUnitario = df.PrecioUnitario ?? 0m,
                        CantidadEntradas = df.CantEntrada 
                    }).ToList()
                })
                .ToListAsync();
        }




        public async Task<FacturaDto> GetFacturaByIdAsync(int id)
        {
            return await _context.Facturas
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.DetallesFacturas)
                    .ThenInclude(df => df.IdPeliculaNavigation)
                .Where(f => f.NroFactura == id)
                .Select(f => new FacturaDto
                {
                    NroFactura = f.NroFactura,
                    FechaCompra = f.FechaCompra ?? DateTime.Now, 
                    NombreCliente = f.IdClienteNavigation.Nombre,
                    ApellidoCliente = f.IdClienteNavigation.Apellido,
                    Detalles = f.DetallesFacturas.Select(df => new DetalleFacturaDto
                    {
                        IdPelicula = df.IdPelicula,
                        TituloPelicula = df.IdPeliculaNavigation.TituloPelicula,
                        PrecioUnitario = df.PrecioUnitario ?? 0m,
                        CantidadEntradas = df.CantEntrada
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<DetalleFacturaDto>> GetDetallesFacturaByIdAsync(int nroFactura)
        {
            return await _context.DetallesFacturas
                .Where(df => df.NroFactura == nroFactura)
                .Select(df => new DetalleFacturaDto
                {
                    IdPelicula = df.IdPelicula,
                    TituloPelicula = df.IdPeliculaNavigation.TituloPelicula,
                    PrecioUnitario = df.PrecioUnitario,
                    CantidadEntradas = df.CantEntrada
                })
                .ToListAsync();
        }

        public async Task<List<ButacaDto>> GetButacasDisponiblesAsync(int idFuncion)
        {
            // Obtenemos la función especificada
            var funcion = await _context.Funciones
                .Where(f => f.IdFuncion == idFuncion)
                .Select(f => new { f.IdSala, f.Fecha })
                .FirstOrDefaultAsync();

            // Verificamos que la función exista
            if (funcion == null)
                throw new Exception("Función no encontrada.");

            // Obtenemos todas las butacas de la sala específica de la función
            var butacas = await _context.ButacasSalas
                .Where(b => b.IdSala == funcion.IdSala) // Filtramos por sala
                .Select(b => new ButacaDto
                {
                    IdButaca = b.IdButacaSala,
                    Descripcion = $"Sala {b.IdSala} - Butaca {b.NroButaca}",
                    Disponible = !b.Reservas.Any(r => r.FechaReserva == funcion.Fecha) && !b.Tickets.Any(t => t.Fecha == funcion.Fecha) // Marcar como false si está ocupada
                })
                .ToListAsync();

            // Retornamos todas las butacas, con el campo Disponible en false para las ocupadas
            return butacas;
        }


        public async Task<List<FormaPagoDto>> GetAllFormasPagoAsync()
        {
            return await _context.FormasPagos
                .Select(fp => new FormaPagoDto
                {
                    IdFormaPago = fp.IdFormaPago,
                    Descripcion = fp.Descripcion
                })
                .ToListAsync();
        }

        public async Task<List<FuncionDto>> GetFuncionesByPeliculaIdAsync(int idPelicula)
        {
            return await _context.Funciones
                .Where(f => f.IdPelicula == idPelicula)
                .Select(f => new FuncionDto
                {
                    IdFuncion = f.IdFuncion,
                    TituloPelicula = f.IdPeliculaNavigation.TituloPelicula,
                    SalaId = f.IdSala,
                    Fecha = f.Fecha,
                    Hora = f.Hora,
                    SubtituloId = f.Subtitulos, // Campo que indica si la función tiene subtítulos
                    LenguajeId = f.IdLenguaje
                })
                .ToListAsync();
        }
        public async Task<CompraEntradaDto> RealizarCompra(CompraEntradaDto compraEntradaDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Crear un nuevo cliente
                    var cliente = new Cliente
                    {
                        Nombre = compraEntradaDto.NombreCliente,
                        Apellido = compraEntradaDto.ApellidoCliente,
                        Documento = compraEntradaDto.DocumentoCliente,
                        FechaNac = compraEntradaDto.FechaNacimientoCliente,
                        Email = compraEntradaDto.EmailCliente,
                        Telefono = compraEntradaDto.TelefonoCliente,
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync(); // Guardar cliente primero para obtener el Id

                    // Crear una nueva factura
                    var factura = new Factura
                    {
                        IdCliente = cliente.IdCliente,
                        FechaCompra = DateTime.Now,
                        IdFormaPago = compraEntradaDto.IdFormaPago,
                        IdBoleteria = compraEntradaDto.IdBoleteria,
                    };
                    _context.Facturas.Add(factura);
                    await _context.SaveChangesAsync(); // Guardar factura para obtener el NroFactura

                    // Crear detalles de la factura
                    var detalleFactura = new DetallesFactura
                    {
                        NroFactura = factura.NroFactura,
                        IdFuncion = compraEntradaDto.IdFuncion,
                        IdPelicula = (await _context.Funciones
                            .Include(f => f.IdPeliculaNavigation)
                            .FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion))?.IdPelicula,
                        PrecioUnitario = 1500.00m, // Ajustar el precio según la lógica
                        CantEntrada = compraEntradaDto.CantidadEntradas
                    };
                    _context.DetallesFacturas.Add(detalleFactura);
                    await _context.SaveChangesAsync(); // Guarda el detalle de factura

                    // Crear un ticket por cada butaca en la lista
                    foreach (var idButacaSala in compraEntradaDto.IdButacasSala)
                    {
                        var ticket = new Ticket
                        {
                            IdButacaSala = idButacaSala,
                            IdPelicula = detalleFactura.IdPelicula,
                            Fecha = DateTime.Now,
                            Hora = (await _context.Funciones.FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion)).Hora,
                            Precio = detalleFactura.PrecioUnitario.GetValueOrDefault(), // Precio individual para cada ticket
                            NroFactura = factura.NroFactura,
                            IdFuncion = compraEntradaDto.IdFuncion
                        };

                        _context.Tickets.Add(ticket);
                    }

                    // Guardar todos los tickets en la base de datos
                    await _context.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    // Retorna DTO de respuesta
                    return new CompraEntradaDto
                    {
                        NombreCliente = cliente.Nombre + " " + cliente.Apellido,
                        TituloPelicula = (await _context.Funciones
                            .Include(f => f.IdPeliculaNavigation)
                            .FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion))?.IdPeliculaNavigation?.TituloPelicula,
                        FechaFuncion = (await _context.Funciones
                            .FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion)).Fecha ?? DateTime.Now,
                        HoraFuncion = (await _context.Funciones
                            .FirstOrDefaultAsync(f => f.IdFuncion == compraEntradaDto.IdFuncion)).Hora ?? TimeSpan.Zero,
                        PrecioTotal = (detalleFactura.PrecioUnitario ?? 0) * compraEntradaDto.CantidadEntradas,
                        IdFormaPago = compraEntradaDto.IdFormaPago,
                        IdBoleteria = compraEntradaDto.IdBoleteria,
                        IdFuncion = compraEntradaDto.IdFuncion // Incluye el ID de la función
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw; // Lanzar la excepción para que el controlador la maneje
                }
            }
        }


        public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync(); // Confirmar la transacción
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync(); // Deshacer la transacción
        }
        public async Task<ClienteFacDTO> GetClientePorIdAsync(int idCliente)
        {
            return await _context.Clientes
                .Where(c => c.IdCliente == idCliente)
                .Select(c => new ClienteFacDTO
                {
                    IdCliente = c.IdCliente,  
                    Nombre = c.Nombre,    
                    Apellido = c.Apellido,  
                    Documento = c.Documento,  
                    FechaNac = c.FechaNac,        
                    Email = c.Email,               
                    Telefono = c.Telefono          
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Cliente>> GetAllClientesAsync()
        {
            
            return await _context.Clientes.ToListAsync();

        }
    }
}

