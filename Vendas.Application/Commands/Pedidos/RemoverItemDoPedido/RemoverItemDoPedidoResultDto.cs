namespace Vendas.Application.Commands.Pedidos.RemoverItemDoPedido
{
    public sealed class RemoverItemDoPedidoResultDto
    {
        public Guid PedidoId { get; }
        public decimal ValorTotal { get; }
        public string Status { get; }

        public RemoverItemDoPedidoResultDto(Guid pedidoId, decimal valorTotal, string status)
        {
            PedidoId = pedidoId;
            ValorTotal = valorTotal;
            Status = status;
        }
    }
}
