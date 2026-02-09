using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Commands.CatalogoCommands.CategoriaCommands.InativarCategoria
{
    public sealed class InativarCategoriaCommandHandler
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public InativarCategoriaCommandHandler(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<InativarCategoriaResultDto> HandleAsync (InativarCategoriaCommand command, CancellationToken cancellationToken = default)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(command.CategoriaId, cancellationToken)
                ?? throw new DomainException("Categoria não encontrada.");

            Guard.Against<DomainException>(!categoria.Ativa, "Não é possível inativar uma categoria já inativa.");

            categoria.Inativar();

            await _categoriaRepository.AtualizarAsync(categoria, cancellationToken);

            return new InativarCategoriaResultDto {
                CategoriaId = categoria.Id,
                Ativa = categoria.Ativa,
            };
        }
    }
}
