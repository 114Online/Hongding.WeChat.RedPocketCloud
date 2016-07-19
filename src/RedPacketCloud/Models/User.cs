using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace RedPacketCloud.Models
{
    public class User : IdentityUser
    {
        public double Balance { get; set; }

        public string Name { get; set; }
    }
}
