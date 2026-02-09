namespace Vendas.Application.Commands.PedidosCommands.RemoverItemDoPedido
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
