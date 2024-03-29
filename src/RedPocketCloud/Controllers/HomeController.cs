﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 展示仪表盘界面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index([FromServices] IConfiguration Config)
        {
            // General informations
            ViewBag.CurrentIp = Config["Host:Ip"].ToString();
            var beg = DateTime.Now.AddDays(-30);
            var cnt = DB.Activities
                .Count(x => x.MerchantId == User.Current.Id && x.Begin >= beg);
            ViewBag.ActivityCount = cnt;
            ViewBag.TemplateCount = DB.Templates.Count(x => x.MerchantId == User.Current.Id);

            // Money statistics
            var labels = new List<string>();
            var money = new List<double>();
            var scope = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            for (var i = scope.AddHours(-11); i <= scope.AddHours(1); i = i.AddHours(1))
            {
                labels.Add(i.ToString("HH:mm"));
                var pl = DB.PayLogs
                    .Where(x => x.MerchantId == User.Current.Id)
                    .Where(x => x.Time <= i)
                    .OrderBy(x => x.Time)
                    .LastOrDefault();
                if (pl == null)
                    money.Add(User.Current.Balance - DB.PayLogs.Where(x => x.MerchantId == User.Current.Id).Sum(x => x.Price));
                else
                    money.Add(pl.Balance);
            }
            ViewBag.MoneyHeight = Convert.ToInt64(money.Max()) + 100;
            ViewBag.MoneyGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { labels = labels, series = new object[] { money } });

            // Activity statistics
            var beg2 = DateTime.Now.Date;
            var ownedActivities = DB.Activities
                .Where(x => x.MerchantId == User.Current.Id && x.Begin >= beg2)
                .ToDictionary(x => x.Id, x => x.Title);
            var ownedActivityIds = ownedActivities
                .Select(x => x.Key)
                .ToList();
            var _activities = DB.RedPockets
                .Where(x => ownedActivityIds.Contains(x.ActivityId))
                .GroupBy(x => x.ActivityId)
                .Select(x => new { Id = x.Key, Money = x.Sum(y => y.Price) })
                .ToList();
            object activities;
            if (_activities.Count > 0)
                activities = new { labels = _activities.Select(x => ownedActivities[x.Id]).ToList(), series = _activities.Select(x => x.Money) };
            else
                activities = new { labels = new string[] { "无活动" }, series = new int[] { 1 } };
            ViewBag.ActivityGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(activities);

            // Attend statistics
            var _attends = DB.Activities
                .Where(x => x.MerchantId == User.Current.Id && x.Begin >= beg2)
                .Select(x => new { Title = x.Title, Attend = x.Attend, Prize = x.Rules.Object.Sum(y => y.Count) })
                .ToList();
            object attends;
            attends = new { labels = _attends.Select(x => x.Title), series = new object[] { _attends.Select(x => x.Attend), _attends.Select(x => x.Prize) } };
            ViewBag.AttendGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(attends);

            return View();
        }
    }
}
