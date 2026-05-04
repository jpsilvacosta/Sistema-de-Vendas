using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Queries.Pedidos.DTOs;

namespace Vendas.Application.Queries.Pedidos.ListarPedidosPagamentosPorStatus
{
    public class ListarPedidosPagamentosPorStatusQueryHandler
    {
        private readonly IPedidoQueryRepository _queryRepo;

        public ListarPedidosPagamentosPorStatusQueryHandler(IPedidoQueryRepository queryRepo)
        {
            _queryRepo = queryRepo;
        }

        public async Task<IReadOnlyList<PagamentoPorStatusDto>> HandleAsync(ListarPedidosPagamentosPorStatusQuery query, CancellationToken ct = default)
        {
            return await _queryRepo.ListarPagamentosPorStatusAsync(query.Status, ct);
        }
    }
}
