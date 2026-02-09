using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AjustarEstoque
{
    public sealed class AjustarEstoqueCommandHandler
    {
        private readonly IProdutoRepository _produtoRepository;

        public AjustarEstoqueCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<AjustarEstoqueResultDto> HandleAsync(AjustarEstoqueCommand command, CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(command.ProdutoId, cancellationToken)
                ?? throw new DomainException("Produto não encontrado.");

            Guard.Against<DomainException>(command.Quantidade > produto.Estoque, "Estoque não pode ser menor que 0.");

            produto.AjustarEstoque(command.Quantidade, command.Motivo);

            await _produtoRepository.AtualizarAsync(produto, cancellationToken);

            return new AjustarEstoqueResultDto
            {
                ProdutoId = produto.Id,
                EstoqueAtualizado = produto.Estoque,
                Motivo = command.Motivo
            };
        }
    }
}
