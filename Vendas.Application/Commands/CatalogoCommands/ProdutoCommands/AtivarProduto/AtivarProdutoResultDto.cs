using System;
using System.Collections.Generic;
using System.Text;

namespace Vendas.Application.Commands.CatalogoCommands.ProdutoCommands.AtivarProduto
{
    public sealed class AtivarProdutoResultDto
    {
        public Guid ProdutoId { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
