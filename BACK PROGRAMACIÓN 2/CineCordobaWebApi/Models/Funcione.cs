﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CineCordobaWebApi.Models;

public partial class Funcione
{
    public int IdFuncion { get; set; }

    public int? IdPelicula { get; set; }

    public int? IdSala { get; set; }

    public DateTime? Fecha { get; set; }

    public TimeSpan? Hora { get; set; }

    public bool? Subtitulos { get; set; }

    public int? IdLenguaje { get; set; }

    public virtual ICollection<DetallesFactura> DetallesFacturas { get; set; } = new List<DetallesFactura>();

    public virtual Lenguaje IdLenguajeNavigation { get; set; }

    public virtual Pelicula IdPeliculaNavigation { get; set; }

    public virtual Sala IdSalaNavigation { get; set; }

    public virtual ICollection<Promocione> Promociones { get; set; } = new List<Promocione>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}