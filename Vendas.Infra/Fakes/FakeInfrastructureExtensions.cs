using Microsoft.Extensions.DependencyInjection;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido;
using Vendas.Application.Commands.PedidosCommands.CancelarPedido;
using Vendas.Application.Commands.PedidosCommands.CriarPedido;
using Vendas.Application.Commands.PedidosCommands.IniciarPagamento;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEmSeparacao;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEntregue;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEnviado;
using Vendas.Domain.Pedidos.Integration.Catalogo;
using Vendas.Domain.Pedidos.Integration.Clientes;

namespace Vendas.Infra.Fakes
{
    public static class FakeInfrastructureExtensions
    {
        public static IServiceCollection AddFakeInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<FakePedidoRepository>();
            services.AddSingleton<IPedidoRepository>(sp => sp.GetRequiredService<FakePedidoRepository>());

            services.AddSingleton<ICatalogoGateway, FakeCatalogoGateway>();
            services.AddSingleton<IClientesGateway, FakeClientesGateway>();

            services.AddSingleton<CatalogoAcl>();
            services.AddSingleton<ClientesAcl>();

            services.AddScoped<CriarPedidoCommandHandler>();
            services.AddScoped<AdicionarItemAoPedidoCommandHandler>();
            services.AddScoped<IniciarPagamentoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEmSeparacaoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEnviadoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEntregueCommandHandler>();
            services.AddScoped<CancelarPedidoCommandHandler>();
            return services;
        }
    }
}