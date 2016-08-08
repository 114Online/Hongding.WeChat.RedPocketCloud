using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Controllers
{
    public class BaseController : BaseController<RpcContext, User, long>
    {
        public override void Prepare()
        {
            base.Prepare();

            if (User.IsSignedIn())
            {
                var notifications = new List<NotificationViewModel>();
                if (User.IsInRole("Root"))
                    notifications.AddRange(DB.Activities
                        .Where(x => !x.End.HasValue)
                        .Select(x => new NotificationViewModel
                        {
                            Text = x.Title + " 正在进行",
                            Url = Url.Action("Activity", "RedPocket", new { id = x.Id })
                        })
                        .ToList());
                else
                    notifications.AddRange(DB.Activities
                        .Where(x => !x.End.HasValue && x.OwnerId == User.Current.Id)
                        .Select(x => new NotificationViewModel
                        {
                            Text = x.Title + " 正在进行",
                            Url = Url.Action("Activity", "RedPocket", new { id = x.Id })
                        })
                        .ToList());
                ViewBag.Notifications = notifications;
            }
        }
    }
}
