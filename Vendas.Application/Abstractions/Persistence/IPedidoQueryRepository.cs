using Vendas.Application.Queries.Pedidos.DTOs;
using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Application.Abstractions.Persistence
{
    public interface IPedidoQueryRepository
    {
        Task<IReadOnlyList<PedidoResumoDto>> ListarResumoAsync(CancellationToken ct = default);
        Task<IReadOnlyList<PedidoResumoDto>> ListarResumoPorClienteAsync(Guid clienteId, CancellationToken ct = default);
        Task<IReadOnlyList<PagamentoPorStatusDto>> ListarPagamentosPorStatusAsync(StatusPagamento status, CancellationToken ct = default);
        Task<PedidoCompletoDto?> ObterPedidoCompletoPorIdAsync(Guid pedidoId, CancellationToken ct = default);
    }
}
