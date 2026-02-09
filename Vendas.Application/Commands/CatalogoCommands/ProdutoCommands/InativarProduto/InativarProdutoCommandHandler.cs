using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.InativarProduto
{
    public sealed class InativarProdutoCommandHandler
    {
        private readonly IProdutoRepository _produtoRepository;
        public InativarProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<InativarProdutoResultDto> HandleAsync(InativarProdutoCommand command,  CancellationToken cancellationToken= default)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(command.ProdutoId, cancellationToken)
                ?? throw new DomainException("Produto não encontrado.");

            Guard.Against<DomainException>(produto.Status != Domain.Catalogo.Enums.StatusProduto.Ativo, "Não é possível inativar um produto já inativado.");

            produto.Inativar();

            return new InativarProdutoResultDto
            {
                ProdutoId = produto.Id,
                Status = produto.Status.ToString()
            };
        }
    }
}
