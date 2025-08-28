using Dapper;
using MediatR;
using PropostaService.Infrastructure.Data;


namespace PropostaService.Application.Propostas.Queries;

/// <summary>
/// Representa a solicitação para listar todas as propostas.
/// Ela espera receber uma coleção de PropostaDto como resposta.
/// </summary>
public class ListarPropostasQuery : IRequest<IEnumerable<PropostaDto>>
{
    // Esta query não precisa de parâmetros, pois queremos listar tudo
}

/// <summary>
/// Handler que processa a ListarPropostasQuery buscando os dados no bd com Dapper
/// </summary>
public class ListarPropostasQueryHandler : IRequestHandler<ListarPropostasQuery, IEnumerable<PropostaDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ListarPropostasQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<PropostaDto>> Handle(ListarPropostasQuery request, CancellationToken cancellationToken)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = @"
            SELECT
                Id,
                NomeCliente,
                Valor,
                CASE Status
                    WHEN 0 THEN 'EmAnalise'
                    WHEN 1 THEN 'Aprovada'
                    WHEN 2 THEN 'Rejeitada'
                    ELSE 'Desconhecido'
                END AS Status,
                MotivoRecusa
            FROM Propostas
            ORDER BY DataCriacao DESC";

        return await connection.QueryAsync<PropostaDto>(sql);
    }
}