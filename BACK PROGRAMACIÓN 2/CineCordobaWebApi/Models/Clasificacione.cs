﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CineCordobaWebApi.Models;

public partial class Clasificacione
{
    public int IdClasificacion { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}