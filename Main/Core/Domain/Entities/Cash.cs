using Domain.Entities.Common;
using System;

namespace Domain.Entities
{
    public class Cash : BaseEntity
    {
        public float Balance { get; set; }

        public float LastMotifiedMoney { get; set; }

        public string LastModified { get; set; }

        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }

    }
}
