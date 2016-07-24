using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        public IActionResult Deliver() => View(DB.Templates.Where(x => x.UserId == User.Current.Id).ToList());
        
        public IActionResult Template() => View(DB.Templates
            .Where(x => x.UserId == User.Current.Id)
            .ToList());

        [HttpGet]
        public IActionResult AddTemplate() => View();

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
                        ContentType = "image/jpg",
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
                        ContentType = "image/jpg",
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
                UserId = User.Current.Id,
                Type = type
            };
            DB.Templates.Add(template);
            DB.SaveChanges();
            return RedirectToAction("Template", "RedPocket");
        }

        [HttpGet]
        public IActionResult EditTemplate(long id) => View(DB.Templates.Single(x => x.Id == id));

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
    }
}
