using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedPacketCloud.Models;

namespace RedPacketCloud.Controllers
{
    public class RedPacketController : BaseController
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
    }
}
