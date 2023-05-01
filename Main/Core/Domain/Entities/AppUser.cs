using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class AppUser : IdentityUser  
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expenditure> Expenditure { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
