using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            // General informations
            var beg = DateTime.Now.AddDays(-30);
            var cnt = DB.Activities
                .Count(x => x.OwnerId == User.Current.Id && x.Begin >= beg);
            ViewBag.ActivityCount = cnt;
            ViewBag.TemplateCount = DB.Templates.Count(x => x.UserId == User.Current.Id);

            // Money statistics
            var labels = new List<string>();
            var money = new List<double>();
            var scope = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            for (var i = scope.AddHours(-11); i <= scope.AddHours(1); i = i.AddHours(1))
            {
                labels.Add(i.ToString("HH:mm"));
                var pl = DB.PayLogs
                    .Where(x => x.UserId == User.Current.Id)
                    .Where(x => x.Time <= i)
                    .LastOrDefault();
                if (pl == null)
                    money.Add(User.Current.Balance - DB.PayLogs.Where(x => x.UserId == User.Current.Id).Sum(x => x.Price));
                else
                    money.Add(pl.Current);
            }
            ViewBag.MoneyHeight = Convert.ToInt64(money.Max()) + 100;
            ViewBag.MoneyGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { labels = labels, series = new object[] { money } });

            // Activity statistics
            var beg2 = DateTime.Now.Date;
            var _activities = DB.Briberies
                .Include(x => x.Activity)
                .Where(x => x.Activity.OwnerId == User.Current.Id && x.Activity.Begin >= beg2)
                .GroupBy(x => x.Activity.Title)
                .Select(x => new { Title = x.Key, Money = x.Sum(y => y.Price) })
                .ToList();
            object activities;
            if (_activities.Count > 0)
                activities = new { labels = _activities.Select(x => x.Title).ToList(), series = _activities.Select(x => x.Money) };
            else
                activities = new { labels = new string[] { "无活动" }, series = new int[] { 1 } };
            ViewBag.ActivityGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(activities);

            // Attend statistics
            var _attends = DB.Activities
                .Where(x => x.OwnerId == User.Current.Id && x.Begin >= beg2)
                .Select(x => new { Title = x.Title, Attend = x.Attend, Prize = x.Rules.Object.Sum(y => y.Count) })
                .ToList();
            object attends;
            attends = new { labels = _attends.Select(x => x.Title), series = new object[] { _attends.Select(x => x.Attend), _attends.Select(x => x.Prize) } };
            ViewBag.AttendGraphicJson = Newtonsoft.Json.JsonConvert.SerializeObject(attends);

            return View();
        }
    }
}
