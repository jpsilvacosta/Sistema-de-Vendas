namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.RenomearCategoria
{
    public sealed class RenomearCategoriaCommand
    {
        public Guid CategoriaId { get; }
        public string Nome { get; }

        public RenomearCategoriaCommand(Guid categoriaId, string nome)
        {
            CategoriaId = categoriaId;
            Nome = nome;
        }
    }
}
