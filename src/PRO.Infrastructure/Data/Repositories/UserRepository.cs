
using PRO.Domain.Entities.Users;

namespace PRO.Infrastructure.Data.Repositories
{
    public class UserRepository : EFRepository<User>
        , IUserRepository
    {
        public UserRepository(EFContext context) : base(context)
        {
        }
    }
}