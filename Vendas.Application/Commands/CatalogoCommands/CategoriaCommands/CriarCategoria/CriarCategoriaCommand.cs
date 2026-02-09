namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.CriarCategoria
{
    public sealed class CriarCategoriaCommand
    {
        public string Nome { get; }
        public string? Descricao { get; }

        public CriarCategoriaCommand(string nome, string? descricao = null)
        {
            Nome = nome;
            Descricao = descricao;
        }
    }
}
