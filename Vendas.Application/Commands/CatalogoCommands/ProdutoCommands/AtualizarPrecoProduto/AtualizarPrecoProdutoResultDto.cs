namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtualizarPrecoProduto
{
    public sealed class AtualizarPrecoProdutoResultDto
    {
        public Guid ProdutoId { get; init; }
        public decimal NovoPreco {  get; init; }
    }
}
