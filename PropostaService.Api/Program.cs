using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropostaService.Application.Propostas.Commands;
using PropostaService.Domain.Interfaces;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy =>
        {
            policy.WithOrigins(
                    "https://localhost:7189", // Porta do WebApp ao rodar pela IDE (verifique)
                    "http://localhost:8082"   // Porta do WebApp ao rodar via Docker Compose
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Configuração do MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CriarPropostaCommand).Assembly));

// 2. Configuração do Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Registro das nossas interfaces e implementações
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// 4. Configuração do MassTransit para PUBLICAR eventos
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Lê a variável de ambiente. Se não existir (rodando na IDE), usa "localhost".
        var rabbitMqHost = Environment.GetEnvironmentVariable("MassTransit__RabbitMqHost") ?? "localhost";

        cfg.Host(rabbitMqHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthorization();
app.MapControllers();

// Lê a variável de ambiente no docker-compose.
var isRunningInContainer = builder.Configuration.GetValue<bool>("RUNNING_IN_CONTAINER");

// Só executa a migração automática se a variável for 'true' (ou seja, dentro de um contêiner).
if (isRunningInContainer)
{
    await Task.Delay(5000);

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();