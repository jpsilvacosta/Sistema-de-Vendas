using Microsoft.Extensions.DependencyInjection;
using Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido;
using Vendas.Application.Commands.PedidosCommands.CancelarPedido;
using Vendas.Application.Commands.PedidosCommands.CriarPedido;
using Vendas.Application.Commands.PedidosCommands.IniciarPagamento;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEmSeparacao;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEntregue;
using Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEnviado;

namespace Vendas.Infra.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<CriarPedidoCommandHandler>();
            services.AddScoped<AdicionarItemAoPedidoCommandHandler>();
            services.AddScoped<IniciarPagamentoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEnviadoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEntregueCommandHandler>();
            services.AddScoped<CancelarPedidoCommandHandler>();
            services.AddScoped<MarcarPedidoComoEmSeparacaoCommandHandler>();

            return services;
        }
    }
}
