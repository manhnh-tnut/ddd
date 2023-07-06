using PRO.Domain.Entities.Departments;

namespace PRO.Infrastructure.Data.Repositories
{
    public class DepartmentRepository : EFRepository<Department>
        , IDepartmentRepository
    {
        public DepartmentRepository(EFContext context) : base(context)
        {
        }
    }
}