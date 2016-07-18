using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedPacketCloud.Models
{
    public class Bribery
    {
        public Guid Id { get; set; }

        [ForeignKey("Activity")]
        public Guid ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        public long Price { get; set; }

        [MaxLength(32)]
        public string OpenId { get; set; }

        [MaxLength(256)]
        public string AvatarUrl { get; set; }

        [MaxLength(64)]
        public string NickName { get; set; }

        [MaxLength(64)]
        public string Ip { get; set; }
    }
}
