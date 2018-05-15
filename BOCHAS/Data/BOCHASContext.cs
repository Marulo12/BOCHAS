using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BOCHAS.Models
{
    public class BOCHASContext : DbContext
    {
        public BOCHASContext (DbContextOptions<BOCHASContext> options)
            : base(options)
        {
        }

        public DbSet<BOCHAS.Models.Persona> Persona { get; set; }
        public DbSet<BOCHAS.Models.TipoDocumento>TipoDocumento { get; set; }
        public DbSet<BOCHAS.Models.Localidad> Localidad { get; set; }
        public DbSet<BOCHAS.Models.Barrio> Barrio { get; set; }
        public DbSet<BOCHAS.Models.Domicilio> Domicilio { get; set; }
        public DbSet<BOCHAS.Models.Usuario> Usuario { get; set; }
        public DbSet<BOCHAS.Models.Empleado> Empleado { get; set; }
        public DbSet<BOCHAS.Models.Cargo> Cargo { get; set; }
    }
}
