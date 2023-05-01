using Domain.Entities.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Writer : BaseEntity
    {
        public string Name { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public string Adress { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }
}
