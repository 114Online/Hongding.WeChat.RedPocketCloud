using RedPocketCloud.Models;

namespace RedPocketCloud.ViewModels
{
    public class RuleViewModel
    {
        public RedPocketType Type { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Url { get; set; }
        public long Coupon { get; set; }
        public long Count { get; set; }
    }
}
