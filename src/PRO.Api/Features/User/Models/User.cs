namespace PRO.Api.Features.User.Models
{
    public class User
    {
        public User(string userName, string firstName, string lastName, string address, DateTime? birthDate, int value)
        {
            this.UserName = userName;
            this.firstName = firstName;
            this.lastName = lastName;
            this.address = address;
            this.birthDate = birthDate;
            this.value = value;
        }

        public int Id { get; internal set; }
        public string UserName { get; internal set; }
        public string firstName { get; internal set; }
        public string lastName { get; internal set; }
        public string address { get; internal set; }
        public DateTime? birthDate { get; internal set; }
        public int value { get; internal set; }
    }
}