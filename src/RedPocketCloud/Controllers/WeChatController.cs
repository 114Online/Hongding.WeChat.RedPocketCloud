using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using RedPocketCloud.Hubs;
using static RedPocketCloud.Common.Wxpay;

namespace RedPocketCloud.Controllers
{
    public enum Operation
    {
        CallBack,
        RedPocket,
        Wallet
    }

    public class WeChatController : BaseController
    {
        #region Infrastructures
        private long RequestCount = 0;

        private string OperationToRoute(string Merchant, Operation Operation)
        {
            var prefix = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/WeChat/";
            var postfix = "/" + Merchant;
            switch (Operation)
            {
                case Operation.CallBack:
                    return prefix + "CallBack" + postfix;
                case Operation.RedPocket:
                    return prefix + "RedPocket" + postfix;
                case Operation.Wallet:
                    return prefix + "Wallet" + postfix;
                default:
                    return null;
            }
        }

        private bool NeedAuthorize => string.IsNullOrWhiteSpace(HttpContext.Session.GetString("OpenId")) || Convert.ToDateTime(HttpContext.Session.GetString("Expire")) <= DateTime.Now;

        [NonAction]
        private IActionResult RedirectToEntry() => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = Enum.Parse(typeof(Operation), HttpContext.Request.Query["Operation"].ToString()) });

        [NonAction]
        private IActionResult RedirectToEntry(Operation operation) => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = operation });

        private async Task<bool> CheckActivityEnd(long activity_id, string Merchant, IDistributedCache Cache, IHubContext<RedPocketHub> Hub)
        {
            if (DB.Briberies.Count(x => x.ActivityId == activity_id) > 0) // 没有红包了
            {
                DB.Activities
                    .Where(x => x.Id == activity_id)
                    .SetField(x => x.End).WithValue(DateTime.Now)
                    .UpdateAsync();

                Hub.Clients.Group(activity_id.ToString()).OnShaked(Convert.ToInt64(await Cache.GetStringAsync("MERCHANT_CURRENT_ACTIVITY_ATTEND_" + Merchant)));
                Hub.Clients.Group(activity_id.ToString()).OnActivityEnd();

                // 清空缓存
                Cache.Remove("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant);
                Cache.Remove("MERCHANT_CURRENT_ACTIVITY_ATTEND_" + Merchant);
                Cache.Remove("MERCHANT_CURRENT_ACTIVITY_" + Merchant);
                return true;
            }
            return false;
        }
        #endregion

        [HttpGet]
        [Route("[controller]/Entry/{Merchant}/{Operation}")]
        public IActionResult Entry(string Merchant, Operation Operation)
        {
            if (NeedAuthorize)
                return Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + Startup.Config["WeChat:AppId"] + "&redirect_uri=" + UrlEncoder.Default.Encode(OperationToRoute(Merchant, Operation.CallBack) + "?next=" + OperationToRoute(Merchant, Operation)) + "&response_type=code&scope=snsapi_userinfo");
            else
                return Redirect(OperationToRoute(Merchant, Operation));
        }

        [HttpGet]
        [Route("[controller]/CallBack/{Merchant}")]
        public async Task<IActionResult> CallBack(string Merchant, string code, string next)
        {
            try
            {
                var oid = await AuthorizeAsync(code);
                HttpContext.Session.SetString("OpenId", oid.Id);
                HttpContext.Session.SetString("AccessToken", oid.AccessToken);
                HttpContext.Session.SetString("Expire", oid.AccessTokenExpire.ToString());
                HttpContext.Session.SetString("Nickname", oid.NickName);
                HttpContext.Session.SetString("AvatarUrl", oid.AvatarUrl);
                return Redirect(next);
            }
            catch
            {
                return RedirectToEntry(Operation.RedPocket);
            }
        }

        [HttpGet]
        public IActionResult RedPocket(string Merchant)
        {
            if (NeedAuthorize)
                return RedirectToEntry(Operation.RedPocket);
            return View(GetActivityByUserId(Merchant));
        }

        [HttpPost]
        [Route("[controller]/Draw/{Merchant}")]
        public async Task<IActionResult> Draw(string Merchant, [FromServices] IHubContext<RedPocketHub> Hub, [FromServices] IDistributedCache Cache)
        {
            // 触发GC
            RequestCount++;
            if (RequestCount >= 600)
            {
                RequestCount = 0;
                GC.Collect();
            }

            if (NeedAuthorize)
                return Content("AUTH");

            var OpenId = HttpContext.Session.GetString("OpenId");

            // 微信平台要求15秒内不能给同一个用户再次发红包
            var cooldown = DateTime.Now.AddSeconds(-15);
            try
            {
                if (Convert.ToDateTime(await Cache.GetStringAsync("REDPOCKET_COOLDOWN_" + OpenId)) >= DateTime.Now.AddSeconds(15))
                    return Content("RETRY");
            }
            catch
            {
                return Content("RETRY");
            }

            // 获取商户制定的每日上限
            int limit;
            var limit_str = await Cache.GetStringAsync("MERCHANT_LIMIT_" + Merchant);
            if (limit_str == null)
            {
                limit = DB.Users
                    .Where(x => x.UserName == Merchant)
                    .Select(x => x.Limit)
                    .Single();
                await Cache.SetStringAsync("MERCHANT_LIMIT_" + Merchant, limit.ToString());
            }
            else
            {
                limit = Convert.ToInt32(limit_str);
            }

            // 判断是否中奖超过每日最大次数
            var beg = DateTime.Now.Date;
            var logs_str = await Cache.GetStringAsync("REDPOCKET_LOGS_" + OpenId);
            if (logs_str == null)
            {
                await Cache.SetStringAsync("REDPOCKET_LOGS_" + OpenId, "[]");
                logs_str = "[]";
            }
            var logs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(logs_str);

            if (logs.Count > limit)
                return Content("EXCEEDED");

            // 获取活动信息
            var activity_id_str = await Cache.GetStringAsync("MERCHANT_CURRENT_ACTIVITY_" + Merchant);
            if (activity_id_str == null)
                return Content("NO");
            var activity_id = Convert.ToInt64(activity_id_str);

            // 参与人数缓存
            // TODO: 每隔100个参与人数写入一次DB
            DB.Activities
                .Where(x => x.Id == activity_id)
                .SetField(x => x.Attend).Plus(1)
                .UpdateAsync();
            
            await Cache.SetStringAsync("MERCHANT_CURRENT_ACTIVITY_ATTEND_" + Merchant, Convert.ToInt64(await Cache.GetStringAsync("MERCHANT_CURRENT_ACTIVITY_ATTEND_" + Merchant) + 1).ToString());

            // 抽奖
            var ratio = Convert.ToDouble(await Cache.GetStringAsync("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant));
            var rand = new Random();
            var num = rand.Next(0, 10000);
            if (num < ratio * 10000)
            {
                var prize = DB.Briberies
                    .Where(x => x.ActivityId == activity_id && !x.ReceivedTime.HasValue)
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault();

                // 检查剩余红包数量
                if (prize == null && await CheckActivityEnd(activity_id, Merchant, Cache, Hub)) 
                    return Content("RETRY");

                // 中奖发放红包
                prize.OpenId = HttpContext.Session.GetString("OpenId");
                prize.NickName = HttpContext.Session.GetString("Nickname");
                prize.AvatarUrl = HttpContext.Session.GetString("AvatarUrl");
                prize.ReceivedTime = DateTime.Now;
                DB.SaveChanges();

                // 微信转账
                await TransferMoneyAsync(prize.Id, HttpContext.Session.GetString("OpenId"), prize.Price, Startup.Config["WeChat:TransferDescription"]);
                Hub.Clients.Group(prize.ActivityId.ToString()).OnDelivered(new { time = prize.ReceivedTime, avatar = HttpContext.Session.GetString("AvatarUrl"), name = HttpContext.Session.GetString("Nickname"), price = prize.Price, id = HttpContext.Session.GetString("OpenId") });

                try
                {
                    // 检查剩余红包数
                    await CheckActivityEnd(activity_id, Merchant, Cache, Hub);
                }
                catch { }

                // 扣除红包费用
                DB.Users
                    .Where(x => x.UserName == Merchant)
                    .SetField(x => x.Balance).Subtract(prize.Price / 100.0)
                    .UpdateAsync();

                // 返回中奖信息
                if (prize.Type == Models.RedPocketType.Cash)
                    return Json(new { Type = prize.Type, Display = (prize.Price / 100).ToString("0.00") + "元" });
                else if (prize.Type == Models.RedPocketType.Coupon)
                {
                    // TODO: Cache the coupons
                    var coupon = DB.Coupons.Where(x => x.Id == prize.CouponId).Select(x => x.Title).Single(); 
                    return Json(new { Type = prize.Type, Display = coupon });
                }
                else
                {
                    return Json(new { Type = prize.Type, Display = prize.Url });
                }
            }

            return Content("RETRY");
        }
    }
}
