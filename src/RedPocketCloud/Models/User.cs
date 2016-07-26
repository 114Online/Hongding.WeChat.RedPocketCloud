using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Models
{
    public class User : IdentityUser<long>
    {
        public double Balance { get; set; }

        public string Name { get; set; }

        public int Limit { get; set; } = 10;
    }
}
