namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.AtivarCategoria
{
    public sealed class AtivarCategoriaCommand
    {
        public Guid CategoriaId { get; }

        public AtivarCategoriaCommand(Guid categoriaId)
        {
            CategoriaId = categoriaId;
        }
    }
}
