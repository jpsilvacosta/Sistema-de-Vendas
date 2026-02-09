using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalogo.ValueObjects;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtualizarPrecoProduto
{
    public sealed class AtualizarPrecoProdutoCommandHandler
    {
        private readonly IProdutoRepository _produtoRepository;

        public AtualizarPrecoProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<AtualizarPrecoProdutoResultDto> HandleAsync(AtualizarPrecoProdutoCommand command,  CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(command.ProdutoId, cancellationToken)
                ?? throw new DomainException("Produto não encontrado.");

            Guard.Against<DomainException>(command.Preco <= 0, "Não é possível atualizar um preço para menor ou igual a zero.");

            var novoPreco = new PrecoProduto(produto.Preco.Valor);

            produto.AlterarPreco(novoPreco);

            await _produtoRepository.AtualizarAsync(produto, cancellationToken);

            return new AtualizarPrecoProdutoResultDto
            {
                ProdutoId = produto.Id,
                NovoPreco = produto.Preco.Valor
            };
        }
    }
}
