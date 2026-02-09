namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AlterarNomeProduto
{
    public sealed class AlterarNomeProdutoCommand
    {
        public Guid ProdutoId { get; }
        public string Nome { get; }

        public AlterarNomeProdutoCommand(Guid produtoId, string nome)
        {
            ProdutoId = produtoId;
            Nome = nome;
        }
    }
}
