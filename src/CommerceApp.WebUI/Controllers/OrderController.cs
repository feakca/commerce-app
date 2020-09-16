using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.WebUI.Identity;
using CommerceApp.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class OrderController : Controller
    {
        private IOrderService _orderService;
        private UserManager<User> _userManager;

        public OrderController(IOrderService orderService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrders()
        {
            var orders = _orderService.GetOrders(_userManager.GetUserId(User));

            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;

            foreach (var order in orders)
            {
                orderModel = new OrderListModel()
                {
                    OrderId = order.Id,
                    Address = order.Address,
                    City = order.City,
                    County = order.County,
                    Email = order.Email,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    OrderDate = order.OrderDate,
                    Phone = order.Phone,
                    UserId = order.UserId,
                    OrderState = order.OrderState,
                    OrderNote = order.OrderNote,
                    OrderNumber = order.OrderNumber,
                    OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                    {
                        ImageUrl = i.Product.ImageUrl,
                        Name = i.Product.Name,
                        OrderItemId = i.ProductId,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList()
                };
                orderListModel.Add(orderModel);
            }

            return View(orderListModel);
        }
    }
}
