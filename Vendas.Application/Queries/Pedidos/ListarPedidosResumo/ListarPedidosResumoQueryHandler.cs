using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Queries.Pedidos.DTOs;

namespace Vendas.Application.Queries.Pedidos.ListarPedidosResumo
{
    public sealed class ListarPedidosResumoQueryHandler
    {
        private readonly IPedidoQueryRepository _queryRepo;

        public ListarPedidosResumoQueryHandler(IPedidoQueryRepository queryRepo)
        {
            _queryRepo = queryRepo;
        }

        public async Task<IReadOnlyList<PedidoResumoDto>> HandleAsync(ListarPedidosResumoQuery query, CancellationToken ct = default)
        {
            return await _queryRepo.ListarResumoAsync(ct);
        }
    }
}
