using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.Entity;
using CommerceApp.WebUI.Identity;
using CommerceApp.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderItem = CommerceApp.Entity.OrderItem;
using BasketItem = CommerceApp.Entity.BasketItem;

namespace CommerceApp.WebUI.Controllers
{
    [Authorize(Roles = "Customer")]
    [AutoValidateAntiforgeryToken]
    public class BasketController : Controller
    {
        private IBasketService _basketService;
        private IOrderService _orderService;
        private UserManager<User> _userManager;

        public BasketController(IBasketService basketService, IOrderService orderService, UserManager<User> userManager)
        {
            _basketService = basketService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var basket = _basketService.GetBasketByUserId(_userManager.GetUserId(User));
            return View(new BasketModel()
            {
                BasketId = basket.Id,
                BasketItems = basket.BasketItems.Select(i => new BasketItemModel()
                {
                    BasketItemId = i.Id,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToBasket(int productId, int quantity)
        {
            _basketService.AddToBasket(_userManager.GetUserId(User), productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromBasket(int productId)
        {
            _basketService.RemoveFromBasket(_userManager.GetUserId(User), productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var basket = _basketService.GetBasketByUserId(_userManager.GetUserId(User));

            var order = new OrderModel()
            {

                BasketModel = new BasketModel()
                {
                    BasketId = basket.Id,
                    BasketItems = basket.BasketItems.Select(i => new BasketItemModel()
                    {
                        BasketItemId = i.Id,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                },
            };
            return View(order);
        }

        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var basket = _basketService.GetBasketByUserId(userId);
                model.BasketModel = new BasketModel()
                {
                    BasketId = basket.Id,
                    BasketItems = basket.BasketItems.Select(i => new BasketItemModel()
                    {
                        BasketItemId = i.Id,
                        Name = i.Product.Name,
                        Price = (double)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };


                var payment = PaymentProcess(model);
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearBasket(model.BasketModel.BasketId);
                    return View("Success");
                }
                else return View(model);
            }
            else return View(model);
        }

        private void ClearBasket(int basketId)
        {
            _basketService.ClearBasket(basketId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            var order = new Order()
            {
                OrderNumber = new Random().Next(100000, 999999).ToString(),
                OrderState = OrderStateEnum.Waiting,
                PaymentId = payment.PaymentId,
                ConversationId = payment.ConversationId,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserId = userId,
                Address = model.Address,
                City = model.City,
                County = model.County,
                Email = model.Email,
                Phone = model.Phone,
                OrderNote = model.OrderNote
            };

            foreach (var item in model.BasketModel.BasketItems)
            {
                var orderItem = new OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }

            _orderService.Create(order);
        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "defaultApiKey";
            options.SecretKey = "defaultSecretKey";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(100000000, 999999999).ToString();
            request.Price = model.BasketModel.TotalPrice().ToString();
            request.PaidPrice = model.BasketModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = model.BasketModel.BasketId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = _userManager.GetUserId(User);
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = "11111111111";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = $"{model.FirstName} {model.LastName}";
            shippingAddress.City = model.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = model.Address;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = $"{model.FirstName} {model.LastName}";
            billingAddress.City = model.City;
            billingAddress.Country = "Turkey";
            billingAddress.Description = model.Address;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<Iyzipay.Model.BasketItem> basketItems = new List<Iyzipay.Model.BasketItem>();
            Iyzipay.Model.BasketItem basketItem;

            foreach (var item in model.BasketModel.BasketItems)
            {
                basketItem = new Iyzipay.Model.BasketItem()
                {
                    Id = item.ProductId.ToString(),
                    Name = item.Name,
                    Category1 = "Telefon",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Quantity).ToString()
                };
                basketItems.Add(basketItem);
            }
            request.BasketItems = basketItems;

            Payment payment = Payment.Create(request, options);
            return payment;
        }
    }
}
