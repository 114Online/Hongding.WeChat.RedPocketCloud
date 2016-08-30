using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RedPocketCloud.Models
{
    /// <summary>
    /// 商户/用户实体
    /// </summary>
    [DataNode("dn0,dn1,dn2,dn3")]
    public class User : IdentityUser<long>
    {
        /// <summary>
        /// 余额
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string Merchant { get; set; }

        /// <summary>
        /// 每日红包活动单用户中奖上限
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
