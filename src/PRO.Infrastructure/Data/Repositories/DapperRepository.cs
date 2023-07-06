using System.Data;
using Microsoft.Data.SqlClient;
using PRO.Domain.Interfaces;

namespace PRO.Infrastructure.Data.Repositories;

public class DapperRepository : IDapperRepository
{
    private string connectionString = "Server=127.0.0.1,1433;Database=ddd;User Id=sa;Password=Pa$$w0rd;TrustServerCertificate=True;Integrated Security=false;MultipleActiveResultSets=true;";
    public IDbConnection connection => new SqlConnection(connectionString);
}