using BethanysPieShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Controllers
{

    // [Authorize] attribute allows only logged in users to place orders
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShoppingCart _shoppingCart;

        public OrderController(IOrderRepository orderRepository, ShoppingCart shoppingCart)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
        }

        // HTTP Get
        public IActionResult Checkout()
        {
            return View();
        }

        // gets called when HTTP Post request is made
        /* Order object is going to be created through process called
         * model binding.
         * Model binding uses model binders to create required object from
         * HTTP data.
         * Model binders that are used are Form data, Route variables and Query string in that order.
         * Tag helpers help us connect form data to object properties.
         */
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                // manual validation rules
                ModelState.AddModelError("", "Your cart is empty, add some pies first");
            }

            /* Model binding engine can perform a check to see if the values provided by the model binder
             * meet the requirements of the model object.
             * We can do this using an explicit call in the action method to ModelState.IsValid.
             * ModelState is a side product of model binding and it will contain data about how the model binding actually went.
             */
            if (ModelState.IsValid)
            {
                _orderRepository.CreateOrder(order);
                _shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }
            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thanks for your order. You'll soon enjoy our delicious pies.";
            return View();
        }

    }
}
