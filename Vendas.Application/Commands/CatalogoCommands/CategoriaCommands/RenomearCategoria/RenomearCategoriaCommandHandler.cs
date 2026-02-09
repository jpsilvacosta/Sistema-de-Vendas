using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.RenomearCategoria
{
    public sealed class RenomearCategoriaCommandHandler
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public RenomearCategoriaCommandHandler(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<RenomearCategoriaResultDto> HandleAsync(RenomearCategoriaCommand command, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(command.CategoriaId, cancellationToken)
                ?? throw new DomainException("Categoria não encontrada.");

            categoria.AlterarNome(command.Nome);

            await _categoriaRepository.AtualizarAsync(categoria, cancellationToken);

            return new RenomearCategoriaResultDto
            {
                CategoriaId = categoria.Id,
                Nome = categoria.Nome,
            };
        }
    }
}
