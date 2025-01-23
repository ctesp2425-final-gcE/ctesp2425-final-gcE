using Microsoft.EntityFrameworkCore;
using restaurante_C_api.Models;

namespace restaurante_C_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet para a tabela Reservations
        public DbSet<ReservaModel> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração adicional para a tabela Reservations
            modelBuilder.Entity<ReservaModel>(entity =>
            {
                entity.ToTable("Reservations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.NomeCliente)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.DataReserva)
                      .IsRequired();

                entity.Property(e => e.HorarioReserva)
                      .IsRequired();

                entity.Property(e => e.NumeroMesa)
                      .IsRequired();

                entity.Property(e => e.NumeroPessoas)
                      .IsRequired();

                entity.Property(e => e.DataCriacao)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
