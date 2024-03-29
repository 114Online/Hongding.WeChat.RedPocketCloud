﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Pomelo.Data.Excel;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;
using static RedPocketCloud.Common.Wxpay;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class RedPocketController : BaseController
    {
        #region 红包活动
        /// <summary>
        /// 展示红包活动列表界面
        /// </summary>
        /// <param name="title"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="merchant"></param>
        /// <returns></returns>
        public IActionResult Index(string title, DateTime? begin, DateTime? end, string merchant)
        {
            IQueryable<Activity> query = DB.Activities;
            if (!string.IsNullOrEmpty(title))
                query = query.Where(x => x.Title.Contains(title) || title.Contains(x.Title));
            if (begin.HasValue)
                query = query.Where(x => x.Begin >= begin.Value);
            if (end.HasValue)
                query = query.Where(x => x.Begin <= end.Value || x.End.HasValue && x.End.Value <= end.Value);
            if (!string.IsNullOrEmpty(merchant))
                query = query.Where(x => DB.Users.Where(y => y.Merchant.Contains(merchant) || merchant.Contains(y.Merchant)).Select(y => y.Id).Contains(x.MerchantId));
            if (!User.IsInRole("Root"))
                query = query.Where(x => x.MerchantId == User.Current.Id);
            query = query.OrderByDescending(x => x.Begin);
            if (User.IsInRole("Root"))
            {
                var users = DB.Users.ToList();
                var ret = query.Join(users, x => x.MerchantId, x => x.Id, (x, y) => new ActivityViewModel
                {
                    Begin = x.Begin,
                    BriberiesCount = x.BriberiesCount,
                    End = x.End,
                    Id = x.Id,
                    Price = x.Price,
                    Ratio = x.Ratio,
                    Title = x.Title,
                    Merchant = y.Merchant,
                    Type = x.Type
                });
                return PagedView(ret, 20);
            }
            else
            {
                var ret = query.Select(x => new ActivityViewModel
                {
                    Begin = x.Begin,
                    BriberiesCount = x.BriberiesCount,
                    End = x.End,
                    Id = x.Id,
                    Price = x.Price,
                    Ratio = x.Ratio,
                    Title = x.Title,
                    Type = x.Type
                });
                return PagedView(ret, 20);
            }
        }

        /// <summary>
        /// 展示创建红包活动界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Deliver() => View(DB.Templates.Where(x => x.MerchantId == User.Current.Id).ToList());

        /// <summary>
        /// 导出红包中奖纪录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Export(long id)
        {
            Activity activity;
            if (User.IsInRole("Root"))
                activity = DB.Activities.Single(x => x.Id == id);
            else
                activity = DB.Activities.Single(x => x.Id == id && x.MerchantId == User.Current.Id);

            var src = DB.RedPockets
                .Where(x => x.ActivityId == id && x.ReceivedTime.HasValue)
                .OrderBy(x => x.ReceivedTime)
                .ToList();

            var nonawarded = activity.BriberiesCount - activity.ReceivedCount;

            var tmp = Guid.NewGuid().ToString();
            var path = Path.Combine(Directory.GetCurrentDirectory(), tmp + ".xlsx");
            using (var excel = ExcelStream.Create(path))
            using (var sheet1 = excel.LoadSheet(1))
            {
                // Headers
                sheet1.Add(new Pomelo.Data.Excel.Infrastructure.Row { "Open Id", "昵称", "类型", "红包", "领取时间" });
                var ids = activity.Rules.Object.Where(x => x.Type == RedPocketType.Coupon).Select(x => x.Coupon).ToList();
                var coupon = DB.Coupons.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Title);
                foreach (var x in src)
                {
                    string rp;
                    switch (x.Type)
                    {
                        case RedPocketType.Cash:
                            rp = (x.Price / 100.0).ToString("0.00");
                            break;
                        case RedPocketType.Coupon:
                            rp = coupon[x.CouponId.Value];
                            break;
                        case RedPocketType.Url:
                            rp = x.Url;
                            break;
                        default:
                            rp = "";
                            break;
                    }
                    sheet1.Add(new Pomelo.Data.Excel.Infrastructure.Row { x.OpenId ?? "", x.NickName ?? "", x.Type.ToString(), rp, x.ReceivedTime.Value.ToString("yyyy-MM-dd HH:mm:ss") });
                }
                sheet1.Add(new Pomelo.Data.Excel.Infrastructure.Row());
                sheet1.Add(new Pomelo.Data.Excel.Infrastructure.Row { "未领取金额（元）", "未领取红包（个）", "总参与人数" });
                sheet1.Add(new Pomelo.Data.Excel.Infrastructure.Row { ((activity.Price - src.Sum(x => x.Price)) / 100.0).ToString("0.00"), nonawarded.ToString(), activity.Attend.ToString() });
                sheet1.SaveChanges();
            }

            var ret = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return File(ret, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", activity.Title + ".xlsx");
        }
        #endregion

        #region 红包页面模板
        /// <summary>
        /// 展示红包活动页面模板列表界面
        /// </summary>
        /// <returns></returns>
        public IActionResult Template() => View(DB.Templates
            .Where(x => x.MerchantId == User.Current.Id)
            .ToList());

        /// <summary>
        /// 展示创建红包活动页面模板界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddTemplate() => View();

        /// <summary>
        /// 处理创建红包活动页面模板请求
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="drawn"></param>
        /// <param name="undrawn"></param>
        /// <param name="pending"></param>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTemplate(long? bg, long? top, long? bottom, long? drawn, long? undrawn, long? pending, TemplateType type, [FromServices] IHostingEnvironment env)
        {
            if (type == TemplateType.Shake)
            {
                if (!bg.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bg.jpg"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/jpeg",
                        FileName = "shake-bg.jpg",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bg = blob.Id;
                }

                if (!top.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-top.jpg"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/jpeg",
                        FileName = "shake-top.jpeg",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    top = blob.Id;
                }

                if (!bottom.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bottom.jpg"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/jpeg",
                        FileName = "shake-bottom.jpeg",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bottom = blob.Id;
                }
            }
            else if (type == TemplateType.Shoop)
            {
                if (!bg.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-bg.jpg"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/jpeg",
                        FileName = "shoop-bg.jpg",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bg = blob.Id;
                }

                if (!top.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-btn.png"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/png",
                        FileName = "shoop-btn.png",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    top = blob.Id;
                }
            }
            else if (type == TemplateType.Command)
            {
                if (!bg.HasValue)
                {
                    var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "command-bg.jpg"));
                    var blob = new Blob
                    {
                        Bytes = bytes,
                        ContentType = "image/jpeg",
                        FileName = "command-bg.jpg",
                        Time = DateTime.Now,
                        ContentLength = bytes.Length
                    };
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bg = blob.Id;
                }
            }

            if (!drawn.HasValue)
            {
                var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "drawn.png"));
                var blob = new Blob
                {
                    Bytes = bytes,
                    ContentType = "image/png",
                    FileName = "drawn.png",
                    Time = DateTime.Now,
                    ContentLength = bytes.Length
                };
                DB.Blobs.Add(blob);
                DB.SaveChanges();
                drawn = blob.Id;
            }

            if (!undrawn.HasValue)
            {
                var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "undrawn.png"));
                var blob = new Blob
                {
                    Bytes = bytes,
                    ContentType = "image/png",
                    FileName = "undrawn.png",
                    Time = DateTime.Now,
                    ContentLength = bytes.Length
                };
                DB.Blobs.Add(blob);
                DB.SaveChanges();
                undrawn = blob.Id;
            }

            if (!pending.HasValue)
            {
                var bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "pending.png"));
                var blob = new Blob
                {
                    Bytes = bytes,
                    ContentType = "image/png",
                    FileName = "pending.png",
                    Time = DateTime.Now,
                    ContentLength = bytes.Length
                };
                DB.Blobs.Add(blob);
                DB.SaveChanges();
                pending = blob.Id;
            }

            var template = new Template
            {
                BackgroundId = bg,
                TopPartId = top,
                BottomPartId = bottom,
                DrawnId = drawn,
                UndrawnId = undrawn,
                PendingId = pending,
                MerchantId = User.Current.Id,
                Type = type
            };
            DB.Templates.Add(template);
            DB.SaveChanges();
            return RedirectToAction("Template", "RedPocket");
        }

        /// <summary>
        /// 展示编辑红包页面模板界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditTemplate(long id) => View(DB.Templates.Single(x => x.Id == id));

        /// <summary>
        /// 处理编辑红包页面模板请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bg"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="drawn"></param>
        /// <param name="undrawn"></param>
        /// <param name="pending"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTemplate(long id, long? bg, long? top, long? bottom, long? drawn, long? undrawn, long? pending)
        {
            var template = DB.Templates.Single(x => x.Id == id);
            if (bg.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.BackgroundId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.BackgroundId = bg.Value;
            }
            if (top.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.TopPartId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.TopPartId = top.Value;
            }
            if (bottom.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.BottomPartId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.BottomPartId = bottom.Value;
            }
            if (drawn.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.DrawnId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.DrawnId = drawn.Value;
            }
            if (undrawn.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.UndrawnId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.UndrawnId = undrawn.Value;
            }
            if (pending.HasValue)
            {
                var origin = DB.Blobs.SingleOrDefault(x => x.Id == template.PendingId);
                if (origin != null)
                    DB.Blobs.Remove(origin);
                template.PendingId = pending.Value;
            }
            DB.SaveChanges();
            return RedirectToAction("Template", "RedPocket");
        }

        /// <summary>
        /// 处理创建红包活动请求
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Rules"></param>
        /// <param name="Ratio"></param>
        /// <param name="Limit"></param>
        /// <param name="TemplateId"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Deliver(string Title, string Rules, double Ratio, int Limit, string Command, long TemplateId, ActivityType Type, [FromServices] IDistributedCache Cache)
        {
            var last = DB.Activities.LastOrDefault(x => x.MerchantId == User.Current.Id && !x.End.HasValue && x.Type == Type);
            if (last != null)
                return Prompt(x =>
                {
                    x.Title = "创建失败";
                    x.Details = $"还有{ (Type == ActivityType.Command ? "经典红包" : "口令红包") }活动正在进行，请等待活动结束后再创建新活动！";
                    x.StatusCode = 400;
                });
            JsonObject<List<RuleViewModel>> rules = Rules;
            // 检查余额
            if (rules.Object.Count == 0)
                return Prompt(x =>
                {
                    x.Title = "创建失败";
                    x.Details = "您没有设定红包发放规则";
                });
            if (rules.Object.Any(x => x.Type == RedPocketType.Cash && x.From < 100))
                return Prompt(x =>
                {
                    x.Title = "创建失败";
                    x.Details = "每个红包金额最少为1元";
                });
            var total = rules.Object.Where(x => x.Type == RedPocketType.Cash).Sum(x => x.To * x.Count);
            if (total / 100.0 > User.Current.Balance)
                return Prompt(x =>
                {
                    x.Title = "余额不足";
                    x.Details = $"您的余额不足以支付本轮活动的￥{ total.ToString("0.00") }";
                    x.StatusCode = 400;
                });
            if (rules.Object.Sum(x => x.Count) > 100000)
                return Prompt(x => 
                {
                    x.Title = "操作失败";
                    x.Details = "每场活动最多发放十万个红包！";
                    x.StatusCode = 400;
                });

            var money = 0L;

            // 存储活动信息
            var act = new Activity
            {
                Begin = DateTime.Now,
                Rules = Rules,
                Title = Title,
                Ratio = Ratio / 100.0,
                MerchantId = User.Current.Id,
                IsBegin = false,
                Limit = Limit,
                Type = Type,
                Command = Command,
                TemplateId = TemplateId
            };

            DB.Activities.Add(act);
            DB.SaveChanges();

            // 创建现金红包
            var random = new Random();
            foreach (var x in rules.Object.Where(x => x.Type == RedPocketType.Cash))
            {
                for (var i = 0; i < x.Count; i++)
                {
                    var p = random.Next(x.From, x.To);
                    money += p;
                    DB.RedPockets.Add(new RedPocket
                    {
                        Type = RedPocketType.Cash,
                        ActivityId = act.Id,
                        Price = p
                    });
                }
            }
            // 创建Url红包
            foreach (var x in rules.Object.Where(x => x.Type == RedPocketType.Url))
            {
                for (var i = 0; i < x.Count; i++)
                {
                    DB.RedPockets.Add(new RedPocket
                    {
                        Type = RedPocketType.Url,
                        ActivityId = act.Id,
                        Url = x.Url
                    });
                }
            }
            // 创建优惠券红包
            foreach (var x in rules.Object.Where(x => x.Type == RedPocketType.Coupon))
            {
                for (var i = 0; i < x.Count; i++)
                {
                    DB.RedPockets.Add(new RedPocket
                    {
                        Type = RedPocketType.Coupon,
                        ActivityId = act.Id,
                        CouponId = x.Coupon
                    });
                }
            }
            DB.SaveChanges();

            // 设置缓存
            var template = DB.Templates
                .Where(x => x.Id == act.TemplateId)
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
            if (Type == ActivityType.Convention)
            {
                Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_" + User.Current.UserName, act.Id);
                Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_TEMPLATE_" + User.Current.UserName, template);
            }
            else
            {
                Cache.SetObject("MERCHANT_CURRENT_COMMAND_ACTIVITY_" + User.Current.UserName, act.Id);
                Cache.SetString("MERCHANT_COMMAND_ACTIVITY_PWD_" + act.Id, act.Command);
                Cache.SetObject("MERCHANT_CURRENT_COMMAND_ACTIVITY_TEMPLATE_" + User.Current.UserName, template);
            }
            Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_LIMIT" + User.Current.UserName, act.Limit);
            Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_RATIO_" + User.Current.UserName, act.Ratio);

            // 生成红包抽取情况Redis缓存
            Common.Drawning.GetRedPocketCaching(DB, Cache, act.Id);

            // 计算红包统计
            await DB.Activities
                .Where(x => x.Id == act.Id)
                .SetField(x => x.Price).WithValue(money)
                .SetField(x => x.BriberiesCount).WithValue(rules.Object.Sum(x => x.Count))
                .SetField(x => x.IsBegin).WithValue(true)
                .UpdateAsync();

            return RedirectToAction("Activity", "RedPocket", new { id = act.Id });
        }

        /// <summary>
        /// 展示红包活动界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Activity(long id)
        {
            var act = DB.Activities.Single(x => x.Id == id);
            if (act.BriberiesCount == 0)
            {
                act.BriberiesCount = act.Rules.Object.Sum(x => x.Count);
                act.Price = DB.RedPockets.Where(x => x.ActivityId == id).Sum(x => x.Price);
                DB.SaveChanges();
            }
            if (!User.IsInRole("Root") && User.Current.Id != act.MerchantId)
                return Prompt(x =>
                {
                    x.StatusCode = 403;
                    x.Title = "权限不足";
                    x.Details = "您没有权限查看这个活动";
                });
            var coupons = act.Rules.Object.Where(x => x.Type == RedPocketType.Coupon).Select(x => x.Coupon).ToList();
            ViewBag.Price = DB.RedPockets.Where(x => x.ActivityId == id && x.ReceivedTime.HasValue).Sum(x => x.Price);
            ViewBag.Coupons = DB.Coupons.Where(x => coupons.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Title);
            ViewBag.Briberies = DB.RedPockets
                .Where(x => x.ActivityId == id && x.ReceivedTime.HasValue)
                .OrderByDescending(x => x.ReceivedTime)
                .ToList();
            return View(act);
        }

        /// <summary>
        /// 处理强制终止红包活动请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Stop(long id, [FromServices] IDistributedCache Cache)
        {
            Activity act;
            if (User.IsInRole("Root"))
            {
                act = DB.Activities
                    .Single(x => x.Id == id);
            }
            else
            {
                act = DB.Activities
                    .Single(x => x.Id == id && x.MerchantId == User.Current.Id);
            }
            act.End = DateTime.Now;
            DB.SaveChanges();

            // 清空缓存
            var Merchant = DB.Users.Single(x => x.Id == act.MerchantId).UserName;
            Cache.Remove("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant);
            if (act.Type == ActivityType.Convention)
                Cache.Remove("MERCHANT_CURRENT_ACTIVITY_" + Merchant);
            else
            {
                Cache.Remove("MERCHANT_CURRENT_COMMAND_ACTIVITY_" + Merchant);
                Cache.Remove("MERCHANT_COMMAND_ACTIVITY_PWD_" + act.Id);
            }

            // 生成扣费记录
            var price = DB.RedPockets.Where(x => x.ActivityId == act.Id && x.Type == RedPocketType.Cash).Sum(x => x.Price);
            var merchant = DB.Users.Single(x => x.UserName == Merchant);
            DB.PayLogs.Add(new PayLog
            {
                Balance = merchant.Balance,
                MerchantId = merchant.Id,
                Price = -(price / 100.0),
                Time = DateTime.Now
            });
            DB.SaveChanges();

            return RedirectToAction("Activity", "RedPocket", new { id = id });
        }

        /// <summary>
        /// Ajax获取参与人数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        public string AttendCount(long id, [FromServices] IDistributedCache Cache)
        {
            return DB.Activities.Single(x => x.Id == id).Attend.ToString();
        }

        /// <summary>
        /// 处理移除红包模板请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Cache"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveTemplate(long id, [FromServices] IDistributedCache Cache)
        {
            var template = DB.Templates.Single(x => x.Id == id);
            if (!User.IsInRole("Root") && template.MerchantId != User.Current.Id)
            {
                return Prompt(x =>
                {
                    x.Title = "操作失败";
                    x.Details = "没有找到相关的红包页面模板";
                    x.StatusCode = 404;
                });
            }
            DB.Templates.Remove(template);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "操作成功";
                x.Details = "红包页面模板已经成功删除";
            });
        }
        #endregion

        #region 红包补发
        /// <summary>
        /// 展示补发红包界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult Compensate()
        {
            return View();
        }

        /// <summary>
        /// 处理补发红包请求
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        public async Task<IActionResult> Compensate(double price, string openid)
        {
            var p = Convert.ToInt32(price * 100);
            if (User.Current.Balance >= price)
            {
                // 微信转账
                if (!await TransferMoneyAsync(0, openid, p, Startup.Config["WeChat:RedPocket:TransferDescription"]))
                {
                    return Prompt(x =>
                    {
                        x.Title = "操作失败";
                        x.Details = "在转账时发生错误，补发红包操作没有成功，请返回重试！";
                    });
                }

                // 从账户中扣除
                DB.Users
                    .Where(x => x.UserName == User.Current.UserName)
                    .SetField(x => x.Balance).Subtract(price)
                    .UpdateAsync();

                return Prompt(x =>
                {
                    x.Title = "补发成功";
                    x.Details = "该用户已收到补发红包款项。";
                });
            }
            else
            {
                return Prompt(x =>
                {
                    x.Title = "操作失败";
                    x.Details = $"您的余额只剩￥{ User.Current.Balance.ToString("0.00") }，本次操作需要您的账号至少有￥{ price.ToString("0.00") }，请充值后再尝试";
                    x.StatusCode = 400;
                });
            }
        }
        #endregion

        #region 黑名单
        /// <summary>
        /// 展示黑名单列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult BlackList(string OpenId)
        {
            if (string.IsNullOrWhiteSpace(OpenId))
                return PagedView(DB.BlackLists.OrderByDescending(x => x.Id));
            else
                return PagedView(DB.BlackLists.Where(x => x.OpenId.Contains(OpenId)).OrderByDescending(x => x.Id));
        }

        /// <summary>
        /// 展示添加黑名单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Root")]
        public IActionResult AddBlackList() => View();

        /// <summary>
        /// 处理将指定Open ID添加至黑名单中请求
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="unlock"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ValidateAntiForgeryToken]
        public IActionResult AddBlackList(string openid, DateTime? unlock)
        {
            if (DB.BlackLists.FirstOrDefault(x => x.OpenId == openid) != null)
                return Prompt(x =>
                {
                    x.Title = "添加失败";
                    x.Details = $"Open ID为{ openid }的用户已经在黑名单中了，请勿重复添加！";
                    x.RedirectText = "返回黑名单列表";
                    x.RedirectUrl = Url.Action("BlackList", "RedPocket");
                });
            DB.BlackLists.Add(new BlackList
            {
                OpenId = openid,
                Unlock = unlock
            });
            DB.SaveChanges();
            return Prompt(x => 
            {
                x.Title = "操作成功";
                x.Details = $"Open ID为{ openid }的用户成功添加至黑名单中，新的黑名单规则将在次日2:00生效。";
                x.RedirectText = "返回黑名单列表";
                x.RedirectUrl = Url.Action("BlackList", "RedPocket");
            });
        }

        /// <summary>
        /// 处理从黑名单中移除请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Root")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveBlackList(long id)
        {
            DB.BlackLists
                .Where(x => x.Id == id)
                .Delete();
            return Prompt(x =>
            {
                x.Title = "操作成功";
                x.Details = "已将其从黑名单中移除！";
                x.RedirectText = "返回黑名单列表";
                x.RedirectUrl = Url.Action("BlackList", "RedPocket");
            });
        }
        #endregion
    }
}
