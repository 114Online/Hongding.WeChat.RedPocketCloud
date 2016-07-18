using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RedPacketCloud.Models
{
    public class Node
    {
        public long Id { get; set; }

        [MaxLength(32)]
        public string Alias { get; set; }

        public string Ip { get; set; }

        public bool IsOnline { get; set; }
    }
}
