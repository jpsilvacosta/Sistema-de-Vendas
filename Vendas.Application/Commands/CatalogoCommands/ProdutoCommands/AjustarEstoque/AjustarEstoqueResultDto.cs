namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AjustarEstoque
{
    public sealed class AjustarEstoqueResultDto
    {
        public Guid ProdutoId { get; init; }
        public int EstoqueAtualizado { get; init; }
        public string Motivo { get; init; } = string.Empty;
    }
}
