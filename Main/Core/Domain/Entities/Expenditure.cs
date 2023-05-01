using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Expenditure : BaseEntity
    {
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]

        public float Money { get; set; }

        public DateTime ExpenditureTime { get; set; }

        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }

    }
}
