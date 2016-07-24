using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace RedPocketCloud.Models
{
    public class User : IdentityUser<long>
    {
        public double Balance { get; set; }

        public string Name { get; set; }

        public int Limit { get; set; } = 10;
    }
}
