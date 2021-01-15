using CoreBase.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Models
{
    public class UserProfile
    {
        public Nullable<DateTime> DOB { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser User { get; set; }
        
    }
}
