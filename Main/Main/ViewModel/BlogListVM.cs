using Domain.Entities;
using System.Collections.Generic;
using System;

namespace Main.ViewModel
{
    public class BlogListVM
    {
        public List<Blog> Blogs { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<int> Dates { get; set; }
    }
}
