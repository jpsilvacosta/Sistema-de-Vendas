namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtualizarPrecoProduto
{
    public sealed class AtualizarPrecoProdutoCommand
    {
        public Guid ProdutoId { get; }
        public decimal Preco {  get; }

        public AtualizarPrecoProdutoCommand(Guid produtoId, decimal preco)
        {
            ProdutoId = produtoId;
            Preco = preco;
        }
    }
}
