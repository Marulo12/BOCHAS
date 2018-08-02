using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BOCHAS.Models
{
    public partial class BOCHASContext : DbContext
    {
        public virtual DbSet<Agenda> Agenda { get; set; }
        public virtual DbSet<AlquilerCancha> AlquilerCancha { get; set; }
        public virtual DbSet<Barrio> Barrio { get; set; }
        public virtual DbSet<Cancha> Cancha { get; set; }
        public virtual DbSet<Cargo> Cargo { get; set; }
        public virtual DbSet<DetalleAlquilerCancha> DetalleAlquilerCancha { get; set; }
        public virtual DbSet<Domicilio> Domicilio { get; set; }
        public virtual DbSet<Empleado> Empleado { get; set; }
        public virtual DbSet<EstadoAlquiler> EstadoAlquiler { get; set; }
        public virtual DbSet<EstadoCancha> EstadoCancha { get; set; }
        public virtual DbSet<Jugador> Jugador { get; set; }
        public virtual DbSet<Localidad> Localidad { get; set; }
        public virtual DbSet<Noticias> Noticias { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Tarjeta> Tarjeta { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoJugador> TipoJugador { get; set; }
        public virtual DbSet<TipoMaterial> TipoMaterial { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                //  optionsBuilder.UseSqlServer(@"Data Source=sistemas04;Initial Catalog=BOCHAS;User ID=bsp;Password=bochas");
                optionsBuilder.UseSqlServer(@"Data Source=186.124.221.26,1433;Initial Catalog=BOCHAS;User ID=bsp;Password=bochas");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agenda>(entity =>
            {
                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.IdAlquilerCancha).HasColumnName("IdAlquiler_Cancha");

                entity.HasOne(d => d.IdAlquilerCanchaNavigation)
                    .WithMany(p => p.Agenda)
                    .HasForeignKey(d => d.IdAlquilerCancha)
                    .HasConstraintName("FK_Agenda_Alquiler_Cancha");

                entity.HasOne(d => d.IdCanchaNavigation)
                    .WithMany(p => p.Agenda)
                    .HasForeignKey(d => d.IdCancha)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Agenda_Cancha");
            });

            modelBuilder.Entity<AlquilerCancha>(entity =>
            {
                entity.HasKey(e => e.Numero);

                entity.ToTable("Alquiler_Cancha");

                entity.Property(e => e.FechaCancelacion)
                    .HasColumnName("Fecha_Cancelacion")
                    .HasColumnType("date");

                entity.Property(e => e.FechaPedido)
                    .HasColumnName("Fecha_Pedido")
                    .HasColumnType("date");

                entity.Property(e => e.FechaReserva)
                    .HasColumnName("Fecha_Reserva")
                    .HasColumnType("date");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.AlquilerCanchaIdClienteNavigation)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Alquiler_Cancha_Usuario1");

                entity.HasOne(d => d.IdEmpleadoNavigation)
                    .WithMany(p => p.AlquilerCanchaIdEmpleadoNavigation)
                    .HasForeignKey(d => d.IdEmpleado)
                    .HasConstraintName("FK_Alquiler_Cancha_Usuario");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.AlquilerCancha)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Alquiler_Cancha_EstadoAlquiler");
            });

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

            modelBuilder.Entity<Cancha>(entity =>
            {
                entity.Property(e => e.Descripcion).HasColumnType("nchar(50)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEstadoCanchaNavigation)
                    .WithMany(p => p.Cancha)
                    .HasForeignKey(d => d.IdEstadoCancha)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cancha_EstadoCancha");

                entity.HasOne(d => d.IdTipoMaterialNavigation)
                    .WithMany(p => p.Cancha)
                    .HasForeignKey(d => d.IdTipoMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cancha_TipoMaterial");
            });

            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.Property(e => e.Descripcion).HasMaxLength(50);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DetalleAlquilerCancha>(entity =>
            {
                entity.ToTable("Detalle_AlquilerCancha");

                entity.Property(e => e.IdAlquilerCancha).HasColumnName("IdAlquiler_Cancha");

                entity.HasOne(d => d.IdAlquilerCanchaNavigation)
                    .WithMany(p => p.DetalleAlquilerCancha)
                    .HasForeignKey(d => d.IdAlquilerCancha)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Detalle_AlquilerCancha_Alquiler_Cancha");

                entity.HasOne(d => d.IdCanchaNavigation)
                    .WithMany(p => p.DetalleAlquilerCancha)
                    .HasForeignKey(d => d.IdCancha)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Detalle_AlquilerCancha_Cancha");
            });

            modelBuilder.Entity<Domicilio>(entity =>
            {
                entity.Property(e => e.Calle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Departamento)
                    .HasColumnName("departamento")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Piso).HasColumnName("piso");

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

                entity.Property(e => e.MotivoBaja)
                    .HasMaxLength(100)
                    .IsUnicode(false);

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

            modelBuilder.Entity<EstadoAlquiler>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoCancha>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Jugador>(entity =>
            {
                entity.HasKey(e => new { e.IdPersona, e.IdTipoJugador });

                entity.HasOne(d => d.IdPersonaNavigation)
                    .WithMany(p => p.Jugador)
                    .HasForeignKey(d => d.IdPersona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jugador_Persona");

                entity.HasOne(d => d.IdTipoJugadorNavigation)
                    .WithMany(p => p.Jugador)
                    .HasForeignKey(d => d.IdTipoJugador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jugador_TipoJugador");
            });

            modelBuilder.Entity<Localidad>(entity =>
            {
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Noticias>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Activo).HasColumnName("activo");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .IsUnicode(false);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasColumnName("titulo")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(250)
                    .IsUnicode(false);
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

                entity.Property(e => e.Imagen).HasColumnType("text");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NroDocumento)
                    .IsRequired()
                    .HasMaxLength(20)
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

                entity.Property(e => e.Origen).HasColumnName("origen");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Session_Usuario");
            });

            modelBuilder.Entity<Tarjeta>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.TipoTarjeta)
                    .IsRequired()
                    .HasMaxLength(70)
                    .IsUnicode(false);
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

            modelBuilder.Entity<TipoJugador>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoMaterial>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.Contraseña)
                    .IsRequired()
                    .HasColumnType("nchar(100)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
