using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;
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
        public IActionResult AddTemplate(Guid? bg, Guid? top, Guid? bottom, TemplateType type, [FromServices] IHostingEnvironment env)
        {
            if (type == TemplateType.Shake)
            {
                if (!bg.HasValue)
                {
                    bg = Guid.NewGuid();
                    var blob = new Blob
                    {
                        Id = bg.Value,
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bg.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shake-bg.jpg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                }

                if (!top.HasValue)
                {
                    top = Guid.NewGuid();
                    var blob = new Blob
                    {
                        Id = top.Value,
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-top.jpg")),
                        ContentType = "image/jpg",
                        FileName = "shake-top.jpeg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                }

                if (!bottom.HasValue)
                {
                    bottom = Guid.NewGuid();
                    var blob = new Blob
                    {
                        Id = bottom.Value,
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shake-bottom.jpg")),
                        ContentType = "image/jpg",
                        FileName = "shake-bottom.jpeg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                }
            }
            else
            {
                if (!bg.HasValue)
                {
                    bg = Guid.NewGuid();
                    var blob = new Blob
                    {
                        Id = bg.Value,
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-bg.jpg")),
                        ContentType = "image/jpeg",
                        FileName = "shoop-bg.jpg",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                }

                if (!top.HasValue)
                {
                    top = Guid.NewGuid();
                    var blob = new Blob
                    {
                        Id = top.Value,
                        Bytes = System.IO.File.ReadAllBytes(Path.Combine("wwwroot", "assets", "img", "shoop-btn.png")),
                        ContentType = "image/png",
                        FileName = "shoop-btn.png",
                        Time = DateTime.Now
                    };
                    blob.ContentLength = blob.Bytes.Length;
                    DB.Blobs.Add(blob);
                    DB.SaveChanges();
                }
            }

            var template = new Template
            {
                BackgroundId = bg,
                TopPartId = top,
                BottomPartId = bottom,
                UserId = User.Current.Id,
                Type = type
            };
            DB.Templates.Add(template);
            DB.SaveChanges();
            return RedirectToAction("Template", "RedPocket");
        }

        [HttpGet]
        public IActionResult EditTemplate(Guid id) => View(DB.Templates.Single(x => x.Id == id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTemplate(Guid id, Guid? bg, Guid? top, Guid? bottom)
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
            DB.SaveChanges();
            return RedirectToAction("Template", "RedPocket");
        }
    }
}
