using Blazored.Toast;
using InsurancePlatform.WebApp;
using InsurancePlatform.WebApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl!)
});

// Configura o HttpClient para encontrar a API do backend.
// Garanta que esta porta (7999) é a porta HTTPS correta do seu PropostaService.Api!
//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri("https://localhost:7999")
//});

builder.Services.AddScoped<PropostaApiService>();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();