using Microsoft.EntityFrameworkCore;
using Gs.Models;

namespace Gs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a tabela 'tb_cliente_gs' para a entidade Cliente
            modelBuilder.Entity<Cliente>().ToTable("tb_cliente_gs");

            // Exemplo: índice único para Email
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Configura a tabela 'tb_job' para a entidade Job
            modelBuilder.Entity<Job>().ToTable("tb_job");
        }
    }
}
