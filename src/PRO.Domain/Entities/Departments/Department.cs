using PRO.Domain.Base;
using PRO.Domain.Entities.Users;
using System.Collections.Generic;

namespace PRO.Domain.Entities.Departments
{
    public partial class Department : BaseEntity<short>
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }

        public virtual ICollection<User> Users { get; internal set; }
    }
}