using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class About : BaseEntity
    {
        public string Details { get; set; }
        public string Image { get; set; }
        public string MapLocation { get; set; }
        public bool Status { get; set; }
    }
}
