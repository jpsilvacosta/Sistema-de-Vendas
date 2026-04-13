using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Infra.Persistence.Context;
using Vendas.Infra.Repositories;

namespace Vendas.Infra.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=vendas.db";

            services.AddDbContext<VendasDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<IPedidoRepository, PedidoRepository>();

            return services;
        }
    }
}
