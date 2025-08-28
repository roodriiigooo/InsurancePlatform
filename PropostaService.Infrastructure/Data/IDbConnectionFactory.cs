using System.Data;

namespace PropostaService.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}