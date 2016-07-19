using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedPacketCloud.Models;
using RedPacketCloud.ViewModels;

namespace RedPacketCloud.Controllers
{
    public class BaseController : BaseController<RpcContext, User, string>
    {
        public override void Prepare()
        {
            base.Prepare();

            var notifications = new List<NotificationViewModel>();
            if (User.IsInRole("Root"))
                notifications.AddRange(DB.Activities
                    .Where(x => !x.End.HasValue)
                    .Select(x => new NotificationViewModel
                    {
                        Text = x.Title + " 正在进行",
                        Url = Url.Action("Show", "Activity", new { id = x.Id })
                    })
                    .ToList());
            else
                notifications.AddRange(DB.Activities
                    .Where(x => !x.End.HasValue && x.OwnerId == User.Current.Id)
                    .Select(x => new NotificationViewModel
                    {
                        Text = x.Title + " 正在进行",
                        Url = Url.Action("Show", "Activity", new { id = x.Id })
                    })
                    .ToList());
            ViewBag.Notifications = notifications;
        }
    }
}
