using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RedPocketCloud.Models;
using RedPocketCloud.ViewModels;

namespace RedPocketCloud.Controllers
{
    [Authorize]
    public class CouponController : BaseController
    {
        /// <summary>
        /// 展示优惠券列表界面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            IQueryable<Coupon> query = DB.Coupons;
            if (User.IsInRole("Root"))
            {
                var users = DB.Users.ToList();
                var ret = query.Join(users, x => x.MerchantId, x => x.Id, (x, y) => new CouponViewModel
                {
                    Title = x.Title,
                    Description = x.Description,
                    Id = x.Id,
                    UserId = y.Id,
                    UserName = y.UserName,
                    Merchant = y.Merchant,
                    Provider = x.Provider,
                    Time = x.Time
                });
                return PagedView(ret.OrderBy(x => x.UserId).ThenByDescending(x => x.Id));
            }
            else
            {
                query = query
                    .Where(x => x.MerchantId == User.Current.Id)
                    .OrderBy(x => x.Id);
                return PagedView(query.OrderByDescending(x => x.Id));
            }
        }

        /// <summary>
        /// 展示创建优惠券界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add() => View();

        /// <summary>
        /// 处理创建优惠券请求
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="coupon"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Coupon Model, IFormFile coupon)
        {
            if (coupon == null)
                return Prompt(x =>
                {
                    x.Title = "添加失败";
                    x.Details = "必须上传优惠券图片！";
                });
            var _coupon = new Blob
            {
                Bytes = coupon.ReadAllBytes(),
                ContentLength = coupon.Length,
                ContentType = coupon.ContentType,
                FileName = coupon.FileName,
                Time = DateTime.Now
            };
            DB.Blobs.Add(_coupon);
            DB.SaveChanges();

            Model.ImageId = _coupon.Id;
            Model.MerchantId = User.Current.Id;
            DB.Coupons.Add(Model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = $"优惠券{ Model.Title }已经成功创建！";
            });
        }

        /// <summary>
        /// 展示编辑优惠券界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(long id)
        {
            var coupon = DB.Coupons.SingleOrDefault(x => x.Id == id);
            if (!User.IsInRole("Root") && coupon.MerchantId != User.Current.Id || coupon == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到优惠券";
                    x.Details = "优惠券的相关信息没有找到，请返回重试！";
                });
            return View(coupon);
        }

        /// <summary>
        /// 处理编辑优惠券请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Model"></param>
        /// <param name="coup"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Coupon Model, IFormFile coup)
        {
            var coupon = DB.Coupons.SingleOrDefault(x => x.Id == id);
            if (!User.IsInRole("Root") && coupon.MerchantId != User.Current.Id || coupon == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到优惠券";
                    x.Details = "优惠券的相关信息没有找到，请返回重试！";
                });
            coupon.Title = Model.Title;
            coupon.Description = Model.Description;
            coupon.Provider = Model.Provider;
            if (coup != null)
            {
                var _coupon = DB.Blobs.Single(x => x.Id == coupon.ImageId);
                _coupon.Bytes = coup.ReadAllBytes();
                _coupon.ContentLength = coup.Length;
                _coupon.ContentType = coup.ContentType;
                _coupon.FileName = coup.FileName;
                _coupon.Time = DateTime.Now;
            }
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "优惠券信息已更新成功！";
            });
        }

        /// <summary>
        /// Ajax返回备选优惠券
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CouponSelect() => View(DB.Coupons.Where(x => x.MerchantId == User.Current.Id).ToList());

        /// <summary>
        /// 处理删除优惠券请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var query = DB.Coupons.Where(x => x.Id == id);
            if (!User.IsInRole("Root"))
                query = query.Where(x => x.MerchantId == User.Current.Id);
            if (query.Delete() == 0)
                return Prompt(x => 
                {
                    x.Title = "删除失败";
                    x.Details = "没有找到相关优惠券！";
                });
            DB.Wallets
                .Where(x => x.CouponId == id)
                .Delete();
            return Prompt(x =>
            {
                x.Title = "删除成功";
                x.Details = "优惠券已经成功删除！";
            });
        }
    }
}
