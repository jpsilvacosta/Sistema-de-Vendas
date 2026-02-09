namespace Vendas.Application.Commands.Catalogo.ProdutoCommands.CriarProduto
{
    public sealed class CriarProdutoCommand
    {
        public string Nome { get; }
        public string Codigo { get; }
        public decimal Preco { get; }
        public Guid CategoriaId { get; }
        public int EstoqueInicial { get; }
        public string? Descricao { get; }

        public CriarProdutoCommand(string nome, string codigo, decimal preco, Guid categoriaId, int estoqueInicial, string? descricao = null)
        {
            Nome = nome;
            Codigo = codigo;
            Preco = preco;
            CategoriaId = categoriaId;
            EstoqueInicial = estoqueInicial;
            Descricao = descricao;
        }
    }
}
