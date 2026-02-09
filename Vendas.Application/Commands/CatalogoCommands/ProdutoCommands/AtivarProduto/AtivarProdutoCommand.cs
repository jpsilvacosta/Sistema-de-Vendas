namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtivarProduto
{
    public sealed class AtivarProdutoCommand
    {
        public Guid ProdutoId { get; }

        public AtivarProdutoCommand(Guid produtoId)
        {
            ProdutoId = produtoId;
        }
    }
}
