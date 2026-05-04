using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Pedidos;

namespace Vendas.Infra.Persistence.Context
{
    public sealed class VendasDbContext : DbContext
    {
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public VendasDbContext(DbContextOptions<VendasDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendasDbContext).Assembly);
            modelBuilder.Entity<ItemPedido>(item =>
            {
                item.ToTable("ItensPedido");
                item.HasKey(i => i.Id);
                item.Property(i => i.Id).ValueGeneratedNever();
                item.Property<Guid>("PedidoId").IsRequired();
                item.Property(i => i.DataCriacao).IsRequired();
                item.Property(i => i.DataAtualizacao).IsRequired(false);
                item.Ignore(i => i.DomainEvents);
                item.Property(i => i.NomeProduto).IsRequired().HasMaxLength(200);
                item.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
                item.Property(i => i.ValorTotal).HasPrecision(18, 2);
                item.Property(i => i.DescontoAplicado).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Pagamento>(pag =>
            {
                pag.ToTable("Pagamentos");
                pag.HasKey(i => i.Id);
                pag.Property(p => p.Id).ValueGeneratedNever();
                pag.Property(p => p.DataCriacao).IsRequired();
                pag.Property(p => p.DataAtualizacao).IsRequired(false);
                pag.Ignore(p => p.DomainEvents);
                pag.Property(p => p.Valor).HasPrecision(18, 2);
                pag.Property(p => p.MetodoPagamento).HasConversion<string>().HasMaxLength(50);
                pag.Property(p => p.StatusPagamento).HasConversion<string>().HasMaxLength(50);
                pag.Property(p => p.CodigoTransacao).HasMaxLength(100);
            });
        }
    }
}
