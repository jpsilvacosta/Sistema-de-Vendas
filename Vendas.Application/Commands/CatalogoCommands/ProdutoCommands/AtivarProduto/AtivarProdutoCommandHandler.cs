using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtivarProduto
{
    public sealed class AtivarProdutoCommandHandler
    {
        private readonly IProdutoRepository _produtoRepository;

        public AtivarProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<AtivarProdutoResultDto> HandleAsync(AtivarProdutoCommand command, CancellationToken cancellationToken= default)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(command.ProdutoId, cancellationToken)
                ?? throw new DomainException("Produto não encontrado.");

            Guard.Against<DomainException>(produto.Status == Domain.Catalogo.Enums.StatusProduto.Ativo, "Não é possível ativar um produto já ativo.");

            produto.Ativar();

            return new AtivarProdutoResultDto
            {
                ProdutoId = produto.Id,
                Status = produto.Status.ToString()
            };
        }
    }
}
