using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.AtivarCategoria
{
    public sealed class AtivarCategoriaCommandHandler
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public AtivarCategoriaCommandHandler(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<AtivarCategoriaResultDto> HandleAsync(AtivarCategoriaCommand command, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(command.CategoriaId, cancellationToken)
                ?? throw new DomainException("Categoria não existe.");

            categoria.Ativar();

            await _categoriaRepository.AtualizarAsync(categoria, cancellationToken);

            return new AtivarCategoriaResultDto { CategoriaId =  categoria.Id, Ativa = categoria.Ativa };
        }
    }
}
