﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CineCordobaWebApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public int? IdCliente { get; set; }

    public int? IdRol { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; }

    public virtual Role IdRolNavigation { get; set; }
}