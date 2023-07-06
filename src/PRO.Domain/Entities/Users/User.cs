using PRO.Domain.Base;
using PRO.Domain.Entities.Departments;

namespace PRO.Domain.Entities.Users
{
    public partial class User : BaseEntity<int>
    {
        public User()
        {
            PaySlips = new HashSet<Payslip>();
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public short DepartmentId { get; set; }
        public float CoefficientsSalary { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Payslip> PaySlips { get; set; }
    }
}