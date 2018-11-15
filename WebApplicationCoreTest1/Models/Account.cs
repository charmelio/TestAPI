using System.ComponentModel.DataAnnotations;

namespace WebApplicationCoreTest1.Models
{
    public class Account
    {
        [EmailAddress]
        public string Email { get; set; }

        public int Id { get; set; }

        /// <summary>
        /// TODO: Change this to an enum with permission set ids
        /// </summary>
        public int Level { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "", MinimumLength = 7)]
        public string Password { get; set; }

        public string Token { get; set; }

        public string Username { get; set; }
    }
}