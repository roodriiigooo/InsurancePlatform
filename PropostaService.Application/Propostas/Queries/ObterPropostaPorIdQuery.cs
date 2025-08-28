using Dapper;
using MediatR;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Data;

namespace PropostaService.Application.Propostas.Queries;

// A Query: carrega o ID da proposta que queremos buscar
public class ObterPropostaPorIdQuery : IRequest<PropostaDto?>
{
    public Guid Id { get; set; }
}

// O Handler: executa a busca no banco de dados usando Dapper
public class ObterPropostaPorIdQueryHandler : IRequestHandler<ObterPropostaPorIdQuery, PropostaDto?>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ObterPropostaPorIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<PropostaDto?> Handle(ObterPropostaPorIdQuery request, CancellationToken cancellationToken)
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
                END AS Status
            FROM Propostas
            WHERE Id = @Id";

        return await connection.QuerySingleOrDefaultAsync<PropostaDto>(sql, new { request.Id });
    }
}