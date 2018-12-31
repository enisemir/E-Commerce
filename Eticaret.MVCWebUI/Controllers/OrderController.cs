using Eticaret.MVCWebUI.Entity;
using Eticaret.MVCWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Eticaret.MVCWebUI.Models.OrderDetailsModel;

namespace Eticaret.MVCWebUI.Controllers
{
    [Authorize(Roles = "admin")]
    public class OrderController : Controller
    {
        DataContext db = new DataContext();
        // GET: Order
        public ActionResult Index()
        {
            var orders = db.Orders.Select(i=>new AdminOrderModel()
            {
                Id=i.Id,
                OrderNumber=i.OrderNumber,
                OrderDate=i.OrderDate,
                OrderState=i.OrderState,
                Total=i.Total,
                Count=i.OrderLines.Count
            }).OrderByDescending(i=>i.OrderDate).ToList();
            return View(orders);
        }
        public ActionResult Details (int id)
        {
            var entity = db.Orders
               .Where(i => i.Id == id)
               .Select(i => new OrderDetailsModel()
               {
                   OrderId = i.Id,
                   OrderNumber = i.OrderNumber,
                   Total = i.Total,
                   OrderState = i.OrderState,
                   OrderDate = i.OrderDate,
                   AdresBasligi = i.AdresBasligi,
                   Adres = i.Adres,
                   Sehir = i.Sehir,
                   Semt = i.Semt,
                   Mahalle = i.Mahalle,
                   PostaKodu = i.PostaKodu,
                   OrderLines = i.OrderLines
                   .Select(a => new OrderLineModel()
                   {
                       ProductId = a.ProductId,
                       ProductName = a.Product.Name.Length > 50 ? a.Product.Name.Substring(0, 47) + "..." : a.Product.Name,
                       Quantity = a.Quantity,
                       Image = a.Product.Image,
                       Price = a.Price
                   }).ToList()
               }).FirstOrDefault();
            return View(entity);
        }

        public ActionResult UpdateOrderState(int OrderId,EnumOrderState OrderState)
        {

            return View();
        }
    }
}