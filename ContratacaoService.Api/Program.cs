using ContratacaoService.Application.Consumers;
using ContratacaoService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PropostaService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();


// 1. Entity Framework Core para este servi�o
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ContratacaoService.Infrastructure.Data.AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Registro da interface e sua implementa��o
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();

// Configura��o do MassTransit
builder.Services.AddMassTransit(x =>
{
    // Registra o consumidor no MassTransit
    x.AddConsumer<PropostaAprovadaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // L� a vari�vel de ambiente. Se n�o existir (rodando na IDE), usa "localhost".
        var rabbitMqHost = Environment.GetEnvironmentVariable("MassTransit__RabbitMqHost") ?? "localhost";

        cfg.Host(rabbitMqHost, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // Configura a fila e diz ao MassTransit para usar nosso consumidor nela
        cfg.ReceiveEndpoint("contratacao-queue", e =>
        {
            e.ConfigureConsumer<PropostaAprovadaConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// L� a vari�vel de ambiente no docker-compose, se executando via "docker-compose up --build"
var isRunningInContainer = builder.Configuration.GetValue<bool>("RUNNING_IN_CONTAINER");

// S� executa a migra��o autom�tica se a vari�vel for 'true' (ou seja, dentro de um cont�iner).
if (isRunningInContainer)
{
    // Adiciona espera para garantir que o BD est� pronto
    await Task.Delay(5000);

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ContratacaoService.Infrastructure.Data.AppDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();