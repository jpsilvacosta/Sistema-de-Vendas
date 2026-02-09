namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.InativarCategoria
{
    public sealed class InativarCategoriaResultDto
    {
        public Guid CategoriaId { get; init; }
        public bool Ativa { get; init; }
    }
}
