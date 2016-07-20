using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RedPocketCloud.Models;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class RedPocketController : BaseController
    {
        public IActionResult Index(string title, DateTime? begin, DateTime? end, string merchant)
        {
            IEnumerable<Activity> ret = DB.Activities;
            if (!string.IsNullOrEmpty(title))
                ret = ret.Where(x => x.Title.Contains(title) || title.Contains(x.Title));
            if (begin.HasValue)
                ret = ret.Where(x => x.Begin >= begin.Value);
            if (end.HasValue)
                ret = ret.Where(x => x.Begin <= end.Value || x.End.HasValue && x.End.Value <= end.Value);
            if (!string.IsNullOrEmpty(merchant))
                ret = ret.Where(x => x.Owner.Name.Contains(merchant) || merchant.Contains(x.Owner.Name));
            if (!User.IsInRole("Root"))
                ret = ret.Where(x => x.OwnerId == User.Current.Id);
            return PagedView(ret, 20);
        }

        [HttpGet]
        public IActionResult Deliver() => View();
        
        public IActionResult Template() => View(DB.Templates
            .Where(x => x.UserId == User.Current.Id)
            .ToList());

        [HttpGet]
        public IActionResult AddTemplate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTemplate(Guid? bg, Guid? top, Guid? bottom, TemplateType type)
        {
            var template = new Template
            {
                BackgroundId = bg,
                TopPartId = top,
                BottomPartId = bottom,
                UserId = User.Current.Id,
                Type = type
            };
            DB.Templates.Add(template);
            return RedirectToAction("Template", "RedPocket");
        }
    }
}
