using System.Data;

namespace PRO.Domain.Interfaces;

public interface IDapperRepository{
    IDbConnection connection{ get; }
}