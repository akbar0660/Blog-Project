using Domain.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Income : BaseEntity
    {
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Bu xana boş ola bilmez")]

        public float Money { get; set; }

        public DateTime IncomeTime { get; set; }

        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }

    }
}
