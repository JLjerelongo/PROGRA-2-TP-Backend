﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CineCordobaWebApi.Models;

public partial class ButacasSala
{
    public int IdButacaSala { get; set; }

    public int? NroButaca { get; set; }

    public string Fila { get; set; }

    public int? IdSala { get; set; }

    public virtual Sala IdSalaNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}