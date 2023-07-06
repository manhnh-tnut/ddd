using PRO.Domain.Base;
using PRO.Domain.Entities.Users;

namespace PRO.Domain.Entities.Users.Events
{
    public class OnRegisteredEvent : BaseEvent
    {
        public User User { get; set; }
    }
}