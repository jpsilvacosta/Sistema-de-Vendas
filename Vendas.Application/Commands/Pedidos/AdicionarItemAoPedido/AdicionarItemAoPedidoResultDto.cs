using System.Text;
using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoResultDto
    {
        public Guid PedidoId { get; }
        public decimal ValorTotal { get; }
        public string StatusPedido { get; }

        public AdicionarItemAoPedidoResultDto(Guid pedidoId, decimal valorTotal, string status)
        {
            PedidoId = pedidoId;
            ValorTotal = valorTotal;
            StatusPedido = status;
        }
    }
}
