using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPocketCloud.Models
{
    [DataNode("dn0")]
    public class BlackList
    {
        public long Id { get; set; }

        [MaxLength(64)]
        public string OpenId { get; set; }

        public DateTime? Unlock { get; set; }
    }
}
