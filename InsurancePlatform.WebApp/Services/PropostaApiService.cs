using System.Net.Http.Json;

namespace InsurancePlatform.WebApp.Services;

#region  Modelos de dados (DTOs) que o Frontend utiliza
public class PropostaDto
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = "";
    public decimal Valor { get; set; }
    public string Status { get; set; } = "";
    public string? MotivoRecusa { get; set; }
}

public class EditarPropostaDto
{
    public string NomeCliente { get; set; } = "";
    public decimal Valor { get; set; }
}

public record RejeitarPropostaDto(string Motivo);

#endregion

public class PropostaApiService
{
    private readonly HttpClient _httpClient;

    public PropostaApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PropostaDto>> GetPropostasAsync() =>
        await _httpClient.GetFromJsonAsync<List<PropostaDto>>("api/propostas") ?? new();

    public async Task<PropostaDto> GetPropostaByIdAsync(Guid id) =>
        await _httpClient.GetFromJsonAsync<PropostaDto>($"api/propostas/{id}") ?? new();

    public async Task CreatePropostaAsync(EditarPropostaDto proposta) =>
        await _httpClient.PostAsJsonAsync("api/propostas", proposta);

    public async Task UpdatePropostaAsync(Guid id, EditarPropostaDto proposta) =>
        await _httpClient.PutAsJsonAsync($"api/propostas/{id}", proposta);

    public async Task ApprovePropostaAsync(Guid id) =>
        await _httpClient.PutAsync($"api/propostas/{id}/aprovar", null);
    
    public async Task RejectPropostaAsync(Guid id, RejeitarPropostaDto motivo) =>
       await _httpClient.PutAsJsonAsync($"api/propostas/{id}/recusar", motivo);
}