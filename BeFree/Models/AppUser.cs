﻿using Microsoft.AspNetCore.Identity;

namespace BeFree.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
    }
}
