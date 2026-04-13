using Microsoft.EntityFrameworkCore;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos;
using Vendas.Infra.Persistence.Context;

namespace Vendas.Infra.Repositories
{
    public sealed class PedidoRepository : IPedidoRepository
    {
        private readonly VendasDbContext _context;
        public PedidoRepository(VendasDbContext context)
        {
            _context = context;
        }
        public async Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IReadOnlyList<Pedido>> ListarTodosAsync(CancellationToken ct = default)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task AdicionarAsync(Pedido pedido, CancellationToken ct = default)
        {
            await _context.Pedidos.AddAsync(pedido, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AtualizarAsync(Pedido pedido, CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
