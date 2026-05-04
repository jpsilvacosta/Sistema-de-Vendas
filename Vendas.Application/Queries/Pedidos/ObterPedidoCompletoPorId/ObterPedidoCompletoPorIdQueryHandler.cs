using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Queries.Pedidos.DTOs;

namespace Vendas.Application.Queries.Pedidos.ObterPedidoCompletoPorId
{
    public sealed class ObterPedidoCompletoPorIdQueryHandler
    {
        private readonly IPedidoQueryRepository _queryRepo;

        public ObterPedidoCompletoPorIdQueryHandler(IPedidoQueryRepository queryRepo)
        {
            _queryRepo = queryRepo;
        }

        public async Task<PedidoCompletoDto?> HandleAsync(ObterPedidoCompletoPorIdQuery query, CancellationToken ct = default)
        {
            return await _queryRepo.ObterPedidoCompletoPorIdAsync(query.PedidoId, ct);
        }
    }
}
