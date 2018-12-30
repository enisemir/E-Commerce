﻿using Eticaret.MVCWebUI.Entity;
using Eticaret.MVCWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eticaret.MVCWebUI.Controllers
{
    public class CartController : Controller
    {
        private DataContext db = new DataContext();
        // GET: Cart
        public ActionResult Index()
        {
            return View(GetCart());
        }
        public ActionResult AddToCart(int id)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == id);
            if (product != null)
            {
                GetCart().AddProduct(product, 1);
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemoveToCart(int id)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == id);
            if (product != null)
            {
                GetCart().DeleteProduct(product);
            }
            return RedirectToAction("Index");
        }
        public Cart GetCart()
        {
            var cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public PartialViewResult Summary()
        {
            return PartialView(GetCart());
        }

        public ActionResult Checkout()
        {
            return View(new ShippingDetails());
        }
        [HttpPost]
        public ActionResult Checkout(ShippingDetails entity)
        {
            var cart = GetCart();

            if (cart.Cardlines.Count == 0)
            {
                ModelState.AddModelError("", "Sepetinizde Ürün Bulunmamaktadır.");
            }

            if (ModelState.IsValid)
            {
                //Siparişi Veri Tabanına Kayıt Etmeliyiz.
                SaveOrder(cart, entity);
                // cart'ı sıfırla
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(entity);
            }


           
        }

        private void SaveOrder(Cart cart, ShippingDetails entity)
        {
            var order = new Order();
            order.OrderNumber = "A" + (new Random()).Next(11111,99999).ToString();
            order.Total = cart.Total();
            order.OrderDate = DateTime.Now;
            order.OrderState = EnumOrderState.Waiting;

            order.Username = entity.Username;
            order.AdresBasligi = entity.AdresBasligi;
            order.Adres = entity.Adres;
            order.Sehir = entity.Sehir;
            order.Semt = entity.Semt;
            order.Mahalle = entity.Mahalle;
            order.PostaKodu = entity.PostaKodu;
            order.OrderLines = new List<OrderLine>();

            foreach (var pr in cart.Cardlines)
            {
                var orderline = new OrderLine();
                orderline.Quantity = pr.Quantity;
                orderline.Price = pr.Product.Price * pr.Quantity;
                orderline.ProductId = pr.Product.Id;
                order.OrderLines.Add(orderline);
            }
            db.Orders.Add(order);
            db.SaveChanges();
        }
    }
}