using Microsoft.Extensions.DependencyInjection;
using Vendas.Domain.Pedidos.Integration.Catalogo;
using Vendas.Domain.Pedidos.Integration.Clientes;
using Vendas.Infra.Fakes;

namespace Vendas.Infra.Extensions
{
    public static class IntegrationExtensions
    {
        public static IServiceCollection AddFakeIntegration(this IServiceCollection services)
        {
            services.AddSingleton<ICatalogoGateway, FakeCatalogoGateway>();
            services.AddSingleton<IClientesGateway, FakeClientesGateway>();

            services.AddSingleton<CatalogoAcl>();
            services.AddSingleton<ClientesAcl>();

            return services;
        }
    }
}