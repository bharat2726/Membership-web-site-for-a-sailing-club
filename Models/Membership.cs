using System;
using System.Collections.Generic;

namespace Sail.Models
{
    public partial class Membership
    {
        public int MembershipId { get; set; }
        public int MemberId { get; set; }
        public int Year { get; set; }
        public string MembershipTypeName { get; set; }
        public double Fee { get; set; }
        public string Comments { get; set; }
        public bool Paid { get; set; }

        public virtual Member Member { get; set; }
        public virtual MembershipType MembershipTypeNameNavigation { get; set; }
    }
}
