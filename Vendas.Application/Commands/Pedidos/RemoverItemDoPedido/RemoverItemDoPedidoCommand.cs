namespace Vendas.Application.Commands.Pedidos.RemoverItemDoPedido
{
    public sealed class RemoverItemDoPedidoCommand
    {
        public Guid PedidoId { get; }
        public Guid ItemId { get; }

        public RemoverItemDoPedidoCommand(Guid pedidoId, Guid itemId)
        {
            PedidoId = pedidoId;
            ItemId = itemId;
        }
    }
}
