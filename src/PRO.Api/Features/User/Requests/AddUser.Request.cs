using System;
using System.ComponentModel.DataAnnotations;

namespace PRO.Api.Features.User.Requests
{
    public class AddUserRequest
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(125)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public short DepartmentId { get; set; }
        public string Password { get; set; }
    }
}