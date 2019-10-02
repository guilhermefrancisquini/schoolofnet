using System;
using System.Collections.Generic;
using System.Text;
using apirest.Models;
using Microsoft.EntityFrameworkCore;

namespace apirest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}