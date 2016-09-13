﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;
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

    public class WeChatController : BaseController<RpcContext, User, long>
    {
        #region Infrastructures
        /// <summary>
        /// 请求数
        /// </summary>
        public static long RequestCount = 0;
        public static double Limiting = 1;

        /// <summary>
        /// 计算回调路径
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Operation"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 判断是否需要微信授权
        /// </summary>
        private bool NeedAuthorize => string.IsNullOrWhiteSpace(HttpContext.Session.GetString("OpenId")) || Convert.ToDateTime(HttpContext.Session.GetString("Expire")) <= DateTime.Now;

        /// <summary>
        /// 跳转至入口点
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private IActionResult RedirectToEntry() => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = Enum.Parse(typeof(Operation), HttpContext.Request.Query["Operation"].ToString()) });

        /// <summary>
        /// 跳转至入口点
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [NonAction]
        private IActionResult RedirectToEntry(Operation operation) => RedirectToAction("Entry", "WeChat", new { Merchant = RouteData.Values["Merchant"].ToString(), Operation = operation });

        /// <summary>
        /// 检查活动是否已经结束
        /// </summary>
        /// <param name="activity_id"></param>
        /// <param name="Merchant"></param>
        /// <param name="Cache"></param>
        /// <param name="Hub"></param>
        /// <returns></returns>
        [NonAction]
        private void CheckActivityEnd(long activity_id, string Merchant, IDistributedCache Cache, IHubContext<RedPocketHub> Hub)
        {
            lock (this)
            {
                var affected = DB.Activities
                    .Where(x => x.Id == activity_id && x.IsBegin && x.BriberiesCount == x.ReceivedCount)
                    .SetField(x => x.End).WithValue(DateTime.Now)
                    .Update();

                if (affected == 1)
                {
                    // 推送活动结束消息
                    Hub.Clients.Group(activity_id.ToString()).OnActivityEnd();

                    // 清空缓存
                    try
                    {
                        Cache.Remove("MERCHANT_CURRENT_ACTIVITY_" + Merchant);
                        Cache.Remove("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant);
                    }
                    catch { }

                    // 生成扣费记录
                    var price = DB.RedPockets.Where(x => x.ActivityId == activity_id && x.Type == RedPocketType.Cash).Sum(x => x.Price);
                    var merchant = DB.Users.Single(x => x.UserName == Merchant);
                    DB.PayLogs.Add(new PayLog
                    {
                        Balance = merchant.Balance,
                        MerchantId = merchant.Id,
                        Price = -price,
                        Time = DateTime.Now
                    });
                    DB.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 获取红包活动页面模板缓存
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [NonAction]
        private TemplateViewModel GetTemplateCache(string Merchant, IDistributedCache Cache)
        {
            var cache = Cache.GetObject<TemplateViewModel>("MERCHANT_CURRENT_ACTIVITY_TEMPLATE_" + Merchant);
            if (cache == null)
            {
                var uid = DB.Users
                    .Where(x => x.UserName == Merchant)
                    .Select(x => x.Id)
                    .SingleOrDefault();
                if (uid == default(long))
                    return null;
                var tid = DB.Activities
                    .Where(x => x.MerchantId == uid)
                    .Select(x => x.TemplateId)
                    .LastOrDefault();

                // 如果没有找到活动
                if (tid == default(long))
                {
                    cache = new TemplateViewModel();
                    Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_TEMPLATE_" + Merchant, cache);
                }
                else
                {
                    cache = DB.Templates
                        .Where(x => x.Id == tid)
                        .Select(x => new TemplateViewModel
                        {
                            Type = x.Type,
                            Top = x.TopPartId,
                            Bottom = x.BottomPartId,
                            Background = x.BackgroundId,
                            Pending = x.PendingId,
                            Drawn = x.DrawnId,
                            Undrawn = x.UndrawnId
                        })
                        .Single();
                    Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_TEMPLATE_" + Merchant, cache);
                }
            }
            return cache;
        }
        #endregion

        /// <summary>
        /// 处理微信入口请求
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Operation"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/Entry/{Merchant}/{Operation}")]
        public IActionResult Entry(string Merchant, Operation Operation)
        {
            if (NeedAuthorize)
                return Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + Startup.Config["WeChat:AppId"] + "&redirect_uri=" + UrlEncoder.Default.Encode(OperationToRoute(Merchant, Operation.CallBack) + "?next=" + OperationToRoute(Merchant, Operation)) + "&response_type=code&scope=snsapi_userinfo");
            else
                return Redirect(OperationToRoute(Merchant, Operation));
        }

        /// <summary>
        /// 处理微信授权回调
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="code"></param>
        /// <param name="next"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 展示红包活动页面
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/RedPocket/{Merchant}")]
        public IActionResult RedPocket(string Merchant, [FromServices] IDistributedCache Cache)
        {
            if (NeedAuthorize)
                return RedirectToEntry(Operation.RedPocket);
            var ret = GetTemplateCache(Merchant, Cache);
            ViewBag.Limit = Limiting;
            if (ret.Type == TemplateType.Shake)
                return View("Shake", ret);
            else
                return View("Shoop", ret);
        }

        /// <summary>
        /// 处理抽红包请求
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Hub"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/Drawn/{Merchant}")]
        public async Task<IActionResult> Drawn(string Merchant, [FromServices] IHubContext<RedPocketHub> Hub, [FromServices] IDistributedCache Cache)
        {
            if (NeedAuthorize)
                return Content("AUTH");
            
            var OpenId = HttpContext.Session.GetString("OpenId");

            // 微信平台要求15秒内不能给同一个用户再次发现金红包
            try
            {
                var openIdCoolDown = await Cache.GetObjectAsync<DateTime?>("REDPOCKET_COOLDOWN_" + OpenId);
                if (openIdCoolDown.HasValue && openIdCoolDown.Value >= DateTime.Now.AddSeconds(-15))
                    return Content("RETRY");
            }
            catch
            {
                return Content("RETRY");
            }
            
            // 获取活动ID
            var activityId = await Cache.GetObjectAsync<long?>("MERCHANT_CURRENT_ACTIVITY_" + Merchant);
            if (activityId == null)
                return Content("NO");

            // 获取商户制定的每日上限
            var limit = await Cache.GetObjectAsync<int?>("MERCHANT_LIMIT_" + Merchant);
            if (!limit.HasValue)
            {
                limit = DB.Users
                    .AsNoTracking()
                    .Where(x => x.UserName == Merchant)
                    .Select(x => x.Limit)
                    .Single();
                await Cache.SetObjectAsync("MERCHANT_LIMIT_" + Merchant, limit);
            }

            // 获取当前活动上限
            var activity_limit = await Cache.GetObjectAsync<int?>("MERCHANT_CURRENT_ACTIVITY_LIMIT" + Merchant);
            if (!activity_limit.HasValue)
            {
                activity_limit = DB.Activities
                    .AsNoTracking()
                    .Where(x => x.Id == activityId)
                    .Select(x => x.Limit)
                    .Single();
                await Cache.SetObjectAsync("MERCHANT_CURRENT_ACTIVITY_LIMIT" + Merchant, activity_limit);
            }

            // 判断是否中奖超过每日最大次数
            var logs = await Cache.GetObjectAsync<List<DrawnLogViewModel>>("REDPOCKET_LOGS_" + OpenId);
            if (logs == null)
            {
                logs = new List<DrawnLogViewModel>();
                await Cache.SetObjectAsync("REDPOCKET_LOGS_" + OpenId, logs);
            }

            logs = logs.Where(x => x.Time >= DateTime.Now.AddDays(-1)).ToList();
            if (logs.Count >= limit.Value || logs.Where(x => x.ActivityId == activityId).Count() >= activity_limit.Value)
                return Content("EXCEEDED");

            // 参与人数缓存
            DB.Activities
                .Where(x => x.Id == activityId.Value)
                .SetField(x => x.Attend).Plus(1)
                .UpdateAsync();

            // 抽奖
            var ratio = await Cache.GetObjectAsync<double?>("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant);
            if (ratio == null)
            {
                ratio = DB.Activities
                    .AsNoTracking()
                    .Single(x => x.Id == activityId.Value)
                    .Ratio;
                Cache.SetObjectAsync("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant, ratio);
            }
            var rand = new Random();
            var num = rand.Next(0, 10000);
            if (num < ratio.Value * 10000)
            {
                var prize = DB.RedPockets
                    .AsNoTracking()
                    .Where(x => x.ActivityId == activityId.Value && x.NickName == null)
                    .OrderBy(x => Guid.NewGuid())
                    .FirstOrDefault();

                // 检查剩余红包数量
                if (prize == null)
                {
                    CheckActivityEnd(activityId.Value, Merchant, Cache, Hub);
                    return Content("RETRY");
                }

                // 中奖发放红包
                DB.RedPockets
                    .Where(x => x.Id == prize.Id)
                    .SetField(x => x.OpenId).WithValue(HttpContext.Session.GetString("OpenId"))
                    .SetField(x => x.NickName).WithValue(HttpContext.Session.GetString("Nickname"))
                    .SetField(x => x.AvatarUrl).WithValue(HttpContext.Session.GetString("AvatarUrl"))
                    .SetField(x => x.ReceivedTime).WithValue(DateTime.Now)
                    .UpdateAsync();

                // 分发奖品
                Coupon coupon = null;
                if (prize.Type == RedPocketType.Cash)
                {
                    await TransferMoneyAsync(prize.Id, HttpContext.Session.GetString("OpenId"), prize.Price, Startup.Config["WeChat:RedPocket:TransferDescription"]);
                }
                else if (prize.Type == RedPocketType.Coupon)
                {
                    // 为微信用户添加优惠券
                    coupon = await Cache.GetObjectAsync<Coupon>("COUPON_" + prize.CouponId);
                    if (coupon == null)
                    {
                        coupon = DB.Coupons
                            .AsNoTracking()
                            .Where(x => x.Id == prize.CouponId)
                            .Single();
                        await Cache.SetObjectAsync("COUPON_" + prize.CouponId, coupon);
                    }
                    DB.Wallets.Add(new Wallet
                    {
                        CouponId = prize.CouponId.Value,
                        Expire = DateTime.Now.AddDays(coupon.Time),
                        OpenId = HttpContext.Session.GetString("OpenId"),
                        Time = DateTime.Now,
                        MerchantId = coupon.MerchantId
                    });
                    await DB.SaveChangesAsync();
                }

                // 中奖人数更新
                DB.Activities
                    .Where(x => x.Id == activityId.Value)
                    .SetField(x => x.ReceivedCount).Plus(1)
                    .UpdateAsync();

                // 推送中奖消息
                if (prize.Type == RedPocketType.Cash)
                {
                    Hub.Clients.Group(prize.ActivityId.ToString()).OnDelivered(new
                    {
                        type = prize.Type,
                        time = DateTime.Now,
                        avatar = HttpContext.Session.GetString("AvatarUrl"),
                        name = HttpContext.Session.GetString("Nickname"),
                        price = prize.Price,
                        id = HttpContext.Session.GetString("OpenId")
                    });
                }
                else if (prize.Type == RedPocketType.Url)
                {
                    Hub.Clients.Group(prize.ActivityId.ToString()).OnDelivered(new
                    {
                        type = prize.Type,
                        time = DateTime.Now,
                        avatar = HttpContext.Session.GetString("AvatarUrl"),
                        name = HttpContext.Session.GetString("Nickname"),
                        id = HttpContext.Session.GetString("OpenId")
                    });
                }
                else
                {
                    Hub.Clients.Group(prize.ActivityId.ToString()).OnDelivered(new
                    {
                        type = prize.Type,
                        time = DateTime.Now,
                        avatar = HttpContext.Session.GetString("AvatarUrl"),
                        name = HttpContext.Session.GetString("Nickname"),
                        id = HttpContext.Session.GetString("OpenId"),
                        coupon = coupon.Title
                    });
                }

                // 如果抽中现金红包，则写入冷却时间
                if (prize.Type == RedPocketType.Cash)
                    Cache.SetObjectAsync("REDPOCKET_COOLDOWN_" + OpenId, DateTime.Now);

                // 添加logs
                logs.Add(new DrawnLogViewModel
                {
                    ActivityId = activityId.Value,
                    Time = DateTime.Now
                });
                logs = logs.Where(x => x.Time >= DateTime.Now.AddDays(-1)).ToList();
                Cache.SetObjectAsync("REDPOCKET_LOGS_" + OpenId, logs);

                try
                {
                    // 检查剩余红包数
                    CheckActivityEnd(activityId.Value, Merchant, Cache, Hub);
                }
                catch { }

                // 返回中奖信息
                if (prize.Type == RedPocketType.Cash)
                    return Json(new { Type = prize.Type, Display = (prize.Price / 100.0).ToString("0.00") + "元" });
                else if (prize.Type == RedPocketType.Coupon)
                {
                    return Json(new { Type = prize.Type, Display = coupon.Title });
                }
                else
                {
                    return Json(new { Type = prize.Type, Display = prize.Url });
                }
            }

            return Content("RETRY");
        }

        /// <summary>
        /// 展示中奖次数超出上限界面
        /// </summary>
        /// <returns></returns>
        public IActionResult Exceeded() => View();

        /// <summary>
        /// 展示优惠券列表
        /// </summary>
        /// <param name="Merchant"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/Wallet/{Merchant}")]
        public IActionResult Wallet(string Merchant)
        {
            if (NeedAuthorize)
                return RedirectToEntry(Operation.RedPocket);
            var coupons = DB.Coupons.ToList();
            var ret = DB.Wallets
                .Where(x => x.OpenId == HttpContext.Session.GetString("OpenId"))
                .Join(coupons, x => x.CouponId, x => x.Id, (x, y) => new WalletViewModel
                {
                    Id = x.Id,
                    ImageId = y.ImageId
                })
                .ToList();
            return View(ret);
        }

        /// <summary>
        /// 展示优惠券二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Coupon(long id)
        {
            if (NeedAuthorize)
                return RedirectToEntry(Operation.RedPocket);
            var coupons = DB.Coupons.ToList();
            var ret = DB.Wallets
                .Where(x => x.Id == id && x.OpenId == HttpContext.Session.GetString("OpenId"))
                .Join(coupons, x => x.CouponId, x => x.Id, (x, y) => new WalletViewModel
                {
                    Id = x.Id,
                    Description = y.Description,
                    ImageId = y.ImageId,
                    VerifyCode = x.VerifyCode,
                    Title = y.Title
                })
                .Single();
            ViewBag.QrCode = Newtonsoft.Json.JsonConvert.SerializeObject(new CouponQrCodeViewModel
            {
                Id = ret.Id,
                VerifyCode = ret.VerifyCode
            });
            return View(ret);
        }
    }
}
