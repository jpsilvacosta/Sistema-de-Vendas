using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Queries.Pedidos.DTOs;

namespace Vendas.Application.Queries.Pedidos.ListarPedidosResumoPorCliente
{
    public sealed class ListarPedidosResumoPorClienteQueryHandler
    {
        private readonly IPedidoQueryRepository _queryRepo;

        public ListarPedidosResumoPorClienteQueryHandler(IPedidoQueryRepository queryRepo)
        {
            _queryRepo = queryRepo;
        }

        public async Task<IReadOnlyList<PedidoResumoDto>> HandleAsync(ListarPedidosResumoPorClienteQuery query, CancellationToken ct = default)
        {
            return await _queryRepo.ListarResumoPorClienteAsync(query.ClienteId, ct);
        }
    }
}
