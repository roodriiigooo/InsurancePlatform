using System.Data;

namespace ContratacaoService.Infrastructure;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}