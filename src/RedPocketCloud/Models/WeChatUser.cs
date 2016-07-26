using System;
using System.Collections.Generic;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Models
{
    public class WeChatUser
    {
        public long Id { get; set; }

        public string OpenId { get; set; }

        public long MerchantId { get; set; }

        public JsonObject<List<CouponViewModel>> Coupons { get; set; } = "[]";
    }
}
