using Dapper;
using MediatR;
using ContratacaoService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContratacaoService.Application.Contratacoes.Queries;

#region DTO
public class ContratacaoDto
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public DateTime DataContratacao { get; set; }
}
#endregion

public class ListarContratacoesQuery : IRequest<IEnumerable<ContratacaoDto>>
{
}

public class ListarContratacoesQueryHandler : IRequestHandler<ListarContratacoesQuery, IEnumerable<ContratacaoDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ListarContratacoesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<ContratacaoDto>> Handle(ListarContratacoesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = "SELECT Id, PropostaId, DataContratacao FROM Contratacoes ORDER BY DataContratacao DESC";

        return await connection.QueryAsync<ContratacaoDto>(sql);
    }
}