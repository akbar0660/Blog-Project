
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Please enter a valid email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string ToEmail { get; set; }

        [Required(ErrorMessage = "Please enter a subject for your email")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public string Message { get; set; }
    }
}
