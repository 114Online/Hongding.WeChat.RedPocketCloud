﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class RedPocketController : BaseController
    {
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
                var ret = query.Join(DB.Users, x => x.MerchantId, x => x.Id, (x,y)=>new ActivityViewModel
                {
                    Begin = x.Begin,
                    BriberiesCount = x.BriberiesCount,
                    End = x.End,
                    Id = x.Id,
                    Price = x.Price,
                    Ratio = x.Ratio,
                    Title = x.Title,
                    Merchant = y.Merchant
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
                    Title = x.Title
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
                    var blob = new Blob
                    {
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bg.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shake-bg.jpg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bg = blob.Id;
                }

                if (!top.HasValue)
                {
                    var blob = new Blob
                    {
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-top.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shake-top.jpeg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    top = blob.Id;
                }

                if (!bottom.HasValue)
                {
                    var blob = new Blob
                    {
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bottom.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shake-bottom.jpeg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bottom = blob.Id;
                }
            }
            else
            {
                if (!bg.HasValue)
                {
                    var blob = new Blob
                    {
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-bg.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shoop-bg.jpg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    bg = blob.Id;
                }

                if (!top.HasValue)
                {
                    var blob = new Blob
                    {
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-btn.png")),
                        ContentType = "image/png",
                        FileName = "shoop-btn.png",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                    top = blob.Id;
                }
            }

            if (!drawn.HasValue)
            {
                var blob = new Blob
                {
                    Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "drawn.png")),
                    ContentType = "image/png",
                    FileName = "drawn.png",
                    Time = DateTime.Now
                };
                blob.ContentLength = blob.Bytes.Length;
                DB.Blobs.Add(blob);
                DB.SaveChanges();
                drawn = blob.Id;
            }

            if (!undrawn.HasValue)
            {
                var blob = new Blob
                {
                    Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "undrawn.png")),
                    ContentType = "image/png",
                    FileName = "undrawn.png",
                    Time = DateTime.Now
                };
                blob.ContentLength = blob.Bytes.Length;
                DB.Blobs.Add(blob);
                DB.SaveChanges();
                undrawn = blob.Id;
            }

            if (!pending.HasValue)
            {
                var blob = new Blob
                {
                    Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "pending.png")),
                    ContentType = "image/png",
                    FileName = "pending.png",
                    Time = DateTime.Now
                };
                blob.ContentLength = blob.Bytes.Length;
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
        public IActionResult Deliver(string Title, string Rules, double Ratio, int Limit, long TemplateId, [FromServices] IDistributedCache Cache)
        {
            if (DB.Activities.Count(x => x.MerchantId == User.Current.Id && !x.End.HasValue) > 0)
                return Prompt(x =>
                {
                    x.Title = "创建失败";
                    x.Details = "还有活动正在进行，请等待活动结束后再创建新活动！";
                    x.StatusCode = 400;
                });
            JsonObject<List<ViewModels.RuleViewModel>> rules = Rules;
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
                    DB.RedPockets.Add(new RedPocket
                    {
                        Type = RedPocketType.Cash,
                        ActivityId = act.Id,
                        Price = random.Next(x.From, x.To)
                    });
                }
            }
            // 创建Url红包
            foreach(var x in rules.Object.Where(x => x.Type == RedPocketType.Url))
            {
                for(var i = 0; i < x.Count; i++)
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
            Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_" + User.Current.UserName, act.Id);
            Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_TEMPLATE_" + User.Current.UserName, template);
            Cache.SetObject("MERCHANT_CURRENT_ACTIVITY_RATIO_" + User.Current.UserName, act.Ratio);

            // 计算红包统计
            act.Price = DB.RedPockets.Where(x => x.ActivityId == act.Id).Sum(x => x.Price);
            act.BriberiesCount = DB.RedPockets.Count(x => x.ActivityId == act.Id);
            act.IsBegin = true;
            DB.SaveChanges();

            return RedirectToAction("Activity", "RedPocket", new { id = act.Id });
        }

        public IActionResult Activity(long id)
        {
            var act = DB.Activities.Single(x => x.Id == id);
            if (!User.IsInRole("Root") && User.Current.Id != act.MerchantId)
                return Prompt(x =>
                {
                    x.StatusCode = 403;
                    x.Title = "权限不足";
                    x.Details = "您没有权限查看这个活动";
                });
            var coupons = act.Rules.Object.Where(x => x.Type == RedPocketType.Coupon).Select(x => x.Coupon).ToList();
            ViewBag.Price = DB.RedPockets.Where(x => x.ActivityId == id && x.ReceivedTime.HasValue).Sum(x => x.Price);
            ViewBag.Amount = DB.RedPockets.Count(x => x.ActivityId == id && x.ReceivedTime.HasValue);
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

            // TODO: Clean up cache
            var Merchant = DB.Users.Single(x => x.Id == act.MerchantId).UserName;
            Cache.Remove("MERCHANT_CURRENT_ACTIVITY_RATIO_" + Merchant);
            Cache.Remove("MERCHANT_CURRENT_ACTIVITY_" + Merchant);

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
    }
}
