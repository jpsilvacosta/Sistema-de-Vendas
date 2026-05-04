using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Application.Queries.Pedidos.ListarPedidosPagamentosPorStatus
{
    public sealed class ListarPedidosPagamentosPorStatusQuery
    {
        public StatusPagamento Status { get; }
        public ListarPedidosPagamentosPorStatusQuery(StatusPagamento status)
        {
            Status = status;
        }
    }
}
