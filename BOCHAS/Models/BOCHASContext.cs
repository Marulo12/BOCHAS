using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BOCHAS.Models
{
    public partial class BOCHASContext : DbContext
    {
        public virtual DbSet<Barrio> Barrio { get; set; }
        public virtual DbSet<Cargo> Cargo { get; set; }
        public virtual DbSet<Domicilio> Domicilio { get; set; }
        public virtual DbSet<Empleado> Empleado { get; set; }
        public virtual DbSet<Localidad> Localidad { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

               //    optionsBuilder.UseSqlServer(@"Data Source=BOCHASBD.mssql.somee.com;Initial Catalog=BOCHASBD;persist security info=False;user id=fgonzalez_SQLLogin_2;pwd=7jlascoceb");
                optionsBuilder.UseSqlServer(@"Data Source=SISTEMAS04;Initial Catalog=BOCHAS;Persist Security Info=True;User ID=BSP;Password=bochas");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barrio>(entity =>
            {
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdLocalidadNavigation)
                    .WithMany(p => p.Barrio)
                    .HasForeignKey(d => d.IdLocalidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Barrio_Localidad");
            });

            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.Property(e => e.Descripcion).HasMaxLength(50);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Domicilio>(entity =>
            {
                entity.Property(e => e.Calle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdBarrioNavigation)
                    .WithMany(p => p.Domicilio)
                    .HasForeignKey(d => d.IdBarrio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Domicilio_Barrio");

                entity.HasOne(d => d.IdLocalidadNavigation)
                    .WithMany(p => p.Domicilio)
                    .HasForeignKey(d => d.IdLocalidad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Domicilio_Localidad");
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasKey(e => e.IdPersona);

                entity.Property(e => e.IdPersona).ValueGeneratedNever();

                entity.Property(e => e.FechaComienzo).HasColumnType("date");

                entity.HasOne(d => d.IdCargoNavigation)
                    .WithMany(p => p.Empleado)
                    .HasForeignKey(d => d.IdCargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Empleado_Cargo");

                entity.HasOne(d => d.IdPersonaNavigation)
                    .WithOne(p => p.Empleado)
                    .HasForeignKey<Empleado>(d => d.IdPersona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Empleado_Persona");
            });

            modelBuilder.Entity<Localidad>(entity =>
            {
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaBaja)
                    .HasColumnName("Fecha_Baja")
                    .HasColumnType("date");

                entity.Property(e => e.IdDomicilio).HasColumnName("Id_Domicilio");

                entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdDomicilioNavigation)
                    .WithMany(p => p.Persona)
                    .HasForeignKey(d => d.IdDomicilio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Persona_Domicilio");

                entity.HasOne(d => d.IdTipoDocumentoNavigation)
                    .WithMany(p => p.Persona)
                    .HasForeignKey(d => d.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Persona_TipoDocumento");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Persona)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("FK_Persona_Usuario");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IdUsuario });

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FechaFin).HasColumnType("date");

                entity.Property(e => e.FechaInicio).HasColumnType("date");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_Usuario");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("nchar(30)");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Contraseña)
                    .IsRequired()
                    .HasColumnType("nchar(10)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
