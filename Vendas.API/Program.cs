using Vendas.API.Endpoints.Pedidos;
using Vendas.Infra.Extensions;
using Vendas.Infra.Fakes;
using Vendas.Infra.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddFakeIntegration();

var app = builder.Build();

app.MapPedidosEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "Vendas API v1"));
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VendasDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();