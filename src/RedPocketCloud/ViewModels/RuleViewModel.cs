using System.ComponentModel.DataAnnotations;
using RedPocketCloud.Models;

namespace RedPocketCloud.ViewModels
{
    public class RuleViewModel
    {
        public RedPocketType Type { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        public int To { get; set; }

        [MaxLength(256)]
        public string Url { get; set; }

        public long Coupon { get; set; }

        public long Count { get; set; }
    }
}
