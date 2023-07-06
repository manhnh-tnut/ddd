using PRO.Domain.Base;
using PRO.Domain.Entities.Users;

namespace PRO.Domain.Entities.Users.Events
{
    public class OnPayslipAddedEvent : BaseEvent
    {
        public Payslip Payslip { get; set; }
    }
}