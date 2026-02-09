namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.RenomearCategoria
{
    public class RenomearCategoriaResultDto
    {
        public Guid CategoriaId { get; init; }
        public string Nome {  get; init; } = string.Empty;
    }
}
