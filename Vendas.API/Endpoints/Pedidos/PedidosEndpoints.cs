using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido;
using Vendas.Application.Commands.PedidosCommands.CancelarPedido;
using Vendas.Application.Commands.PedidosCommands.CriarPedido;
using Vendas.Application.Commands.PedidosCommands.IniciarPagamento;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEmSeparacao;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEntregue;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEnviado;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Infra.Fakes;
namespace Vendas.API.Endpoints.Pedidos

{
    public static class PedidosEndpoints
    {
        public static WebApplication MapPedidosEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/pedidos")
                        .WithTags("Pedidos")
                        .WithOpenApi();

            group.MapGet("/fake-ids", () => Results.Ok(new
            {
                clientes = new[]
                {
                    new
                    {
                        clienteId = Guid.Parse("22222222-0000-0000-0000-000000000001"),
                        enderecos = new[]
                        {
                            new { enderecoId = Guid.Parse ("33333333-0000-0000-0000-000000000001"),
                                 descricao = "Av. Paulista, 1578, Bela Vista, São Paulo" },
                            new { enderecoId = Guid.Parse ("33333333-0000-0000-0000-000000000002"),
                                 descricao = "Rua das Flores, Vila Olímpia, São Paulo" }
                            }
                        },
                        new
                        {
                            clienteId = Guid.Parse("22222222-0000-0000-0000-000000000002"),
                            enderecos = new[]
                            {
                                new { enderecoId = Guid.Parse ("33333333-0000-0000-0000-000000000003"),
                                     descricao = "Rua dos Pinheiros, Pinheiros, São Paulo" }
                            }
                        }
                },
                produtos = new[]
                {
                    new { produtoId = Guid.Parse("11111111-0000-0000-0000-000000000001"),
                          descricao = "Notebook" },
                    new { produtoId = Guid.Parse("11111111-0000-0000-0000-000000000002"),
                          descricao = "Mouse" },
                    new { produtoId = Guid.Parse("11111111-0000-0000-0000-000000000003"),
                          descricao = "Monitor" },
                    new { produtoId = Guid.Parse("11111111-0000-0000-0000-000000000004"),
                          descricao = "Teclado" }
                }
            })).WithSummary("Exibe os IDs dos dados disponíveis nos Fakes para usar nos testes.");

            //group.MapGet("/", async (IPedidoRepository repo, CancellationToken ct) =>
            //{
            //    var pedidos = await repo.ListarTodosAsync(ct);
            //    var resultado = pedidos.Select(p => new
            //    {
            //        p.Id,
            //        p.NumeroPedido,
            //        p.ClienteId,
            //        p.ValorTotal,
            //        Status = p.StatusPedido.ToString(),
            //        p.DataCriacao,
            //        TotalItens = p.Itens.Count
            //    });
            //    return Results.Ok(resultado);
            //}).WithSummary("Lista todos os pedidos em memória.");

            group.MapGet("/{id:guid}", async (Guid id, IPedidoRepository repo, CancellationToken ct) =>
            {
                var pedido = await repo.ObterPorIdAsync(id, ct);
                if (pedido is null) return Results.NotFound();

                var resultado = new
                {
                    pedido.Id,
                    pedido.NumeroPedido,
                    pedido.ClienteId,
                    pedido.ValorTotal,
                    Status = pedido.StatusPedido.ToString(),
                    pedido.DataCriacao,
                    pedido.DataAtualizacao,
                    Endereco = new
                    {
                        pedido.EnderecoEntrega.Logradouro,
                        pedido.EnderecoEntrega.Numero,
                        pedido.EnderecoEntrega.Bairro,
                        pedido.EnderecoEntrega.Cidade,
                        pedido.EnderecoEntrega.Estado,
                        pedido.EnderecoEntrega.Cep
                    },
                    Itens = pedido.Itens.Select(i => new
                    {
                        i.Id,
                        i.ProdutoId,
                        i.NomeProduto,
                        i.PrecoUnitario,
                        i.Quantidade,
                        i.ValorTotal
                    }),
                    Pagamentos = pedido.Pagamentos.Select(pg => new
                    {
                        pg.Id,
                        Metodo = pg.MetodoPagamento.ToString(),
                        Status = pg.StatusPagamento.ToString(),
                        pg.Valor,
                        pg.CodigoTransacao,
                        pg.DataPagamento
                    })
                };
                return Results.Ok(resultado);
            }).WithSummary("Retorna detalhes completos de um pedido.");

            group.MapPost("/", async (CriarPedidoRequest req, CriarPedidoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var command = new CriarPedidoCommand(req.ClienteId, req.EnderecoId);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Created($"/pedidos/{result.PedidoId}", result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { erro = ex.Message });
                }
                catch(DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Cria um novo pedido.");

            group.MapPost("/{id:guid}/itens", async (Guid id, AdicionarItemRequest req, AdicionarItemAoPedidoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var command = new AdicionarItemAoPedidoCommand(id, req.ProdutoId, req.Quantidade);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new { erro = ex.Message });
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Adiciona um item ao pedido.");

            group.MapPost("/{id:guid}/pagamento", async(Guid id, IniciarPagamentoRequest req, IniciarPagamentoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var metodo = (MetodoPagamento)req.MetodoPagamento;
                    var command = new IniciarPagamentoCommand(id, metodo);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Inicia o pagamento do pedido.");

            group.MapPost("/{id:guid}/pagamento/confirmacao", async (
                Guid id, 
                ConfirmarPagamentoRequest req, 
                IPedidoRepository repo, 
                CancellationToken ct) =>
            {
                try
                {
                    var pedido = await repo.ObterPorIdAsync(id, ct);
                    if (pedido is null) return Results.NotFound();

                    var pagamento = pedido.Pagamentos
                        .FirstOrDefault(p => p.Id == req.PagamentoId);

                    if (pagamento is null)
                        return Results.NotFound(new { erro = "Pagamento não encontrado." });

                    pagamento.GerarCodigoTransacaoLocal();
                    pagamento.ConfirmarPagamento();
                    pedido.HandlePagamentoAprovado(pagamento.Id);

                    await repo.AtualizarAsync(pedido, ct);

                    return Results.Ok(new
                    {
                        PedidoId = pedido.Id,
                        PagamentoId = pagamento.Id,
                        StatusPedido = pedido.StatusPedido.ToString(),
                        StatusPagamento = pagamento.StatusPagamento.ToString(),
                        CodigoTransacao = pagamento.CodigoTransacao
                    });
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Confirma o pagamento do pedido (simula gateway).")
              .WithDescription("SIMULAÇÃO - em produção este endpoint não existiria.\n" +
              "O gateway de pagamento enviaria um webhook para /webhook/pagamento\n" +
              "com o código de transação gerado externamente.\n" +
              "Aqui o código é gerado localmente via GerarTransacaoLocal().");

            group.MapPost("/{id:guid}/separacao", async(Guid id, MarcarPedidoComoEmSeparacaoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var command = new MarcarPedidoComoEmSeparacaoCommand(id);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Marca o pedido como em separação (Pagamento confirmado => EmSeparacao).");

            group.MapPost("/{id:guid}/enviado", async(Guid id, MarcarPedidoComoEnviadoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var command = new MarcarPedidoComoEnviadoCommand(id);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
             }).WithSummary("Marca o pedido como enviado (EmSeparacao => Enviado).");

            group.MapPost("/{id:guid}/entregue", async(Guid id, MarcarPedidoComoEntregueCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    MarcarPedidoComoEntregueCommand command = new MarcarPedidoComoEntregueCommand(id);
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Marca o pedido como entregue (Enviado => Entregue).");

            group.MapPost("/{id:guid}/cancelar", async (Guid id, CancelarPedidoRequest? req, CancelarPedidoCommandHandler handler, CancellationToken ct) =>
            {
                try
                {
                    var command = new CancelarPedidoCommand(id, req?.CodigoMotivo ?? "Outro");
                    var result = await handler.HandleAsync(command, ct);
                    return Results.Ok(result);
                }
                catch (DomainException ex)
                {
                    return Results.UnprocessableEntity(new { erro = ex.Message });
                }
            }).WithSummary("Cancela o pedido.")
              .WithDescription(
                "Body opcional. Códigos válidos para CodigoMotivo:\n" +
                " ClienteDesistiu  - Cliente desistiu da compra\n" +
                " ErroPagamento    - Erro no processamento do pagamento\n" +
                " ItemSemEstoque   - Item esgotado no estoque\n" +
                " EnderecoInvalido - Endereço de entrega inválido\n" +
                " Outro            - Outro motivo não especificado\n" +
                "Se omitido, o motivo padrão 'Outro' é aplicado.");

            return app;
        }
    }
}
