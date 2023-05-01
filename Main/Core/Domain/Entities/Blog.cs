using Domain.Entities.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Blog : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;
        public bool Status { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public Writer Writer { get; set; }
        public int WriterId { get; set; }
    }
}
