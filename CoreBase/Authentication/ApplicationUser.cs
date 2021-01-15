using CoreBase.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Authentication
{
    public class ApplicationUser:IdentityUser
    {
        public virtual UserProfile UserProfile { get; set; }
    }
}
