﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CineCordobaWebApi.Models;

public partial class DetallesFactura
{
    public int IdDetFact { get; set; }

    public int? NroFactura { get; set; }

    public int? IdPelicula { get; set; }

    public int? IdSala { get; set; }

    public int? IdFuncion { get; set; }

    public int? CantEntrada { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public virtual Funcione IdFuncionNavigation { get; set; }

    public virtual Pelicula IdPeliculaNavigation { get; set; }

    public virtual Sala IdSalaNavigation { get; set; }

    public virtual Factura NroFacturaNavigation { get; set; }
}