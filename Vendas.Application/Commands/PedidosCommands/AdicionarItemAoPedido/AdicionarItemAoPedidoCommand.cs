namespace Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoCommand
    {
        public Guid PedidoId { get; }
        public Guid ProdutoId { get; }
        public int Quantidade { get; }

        public AdicionarItemAoPedidoCommand(Guid pedidoId, Guid produtoId, int quantidade)
        {
            PedidoId = pedidoId;
            ProdutoId = produtoId;
            Quantidade = quantidade;
        }
    }
}
