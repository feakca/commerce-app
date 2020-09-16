using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.WebUI.EmailServices;
using CommerceApp.WebUI.Extensions;
using CommerceApp.WebUI.Identity;
using CommerceApp.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace CommerceApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private IBasketService _basketService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, IBasketService basketService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _basketService = basketService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "User name is wrong.",
                    AlertType = "danger"
                });
                return View();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Account was not confirmed.",
                    AlertType = "warning"
                });
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }
            TempData.Put("message", new AlertMessage()
            {
                Title = "Warning",
                Message = "Password is wrong.",
                AlertType = "danger"
            });
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "Customer");
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = token
                });
                await _emailSender.SendEmailAsync(model.Email, "Confirmation Mail", @$"Please <a href='https://localhost:5001{url}'>click</a> for confirmation.");
                return RedirectToAction("Login", "Account");
            }

            TempData.Put("message", new AlertMessage()
            {
                Title = "Warning",
                Message = "Password Rules: Uppercase, Lowercase, Nonalphanumeric, Min Lenght: 6",
                AlertType = "warning"
            });
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new AlertMessage()
            {
                Title = "Info",
                Message = "Succesfully logout.",
                AlertType = "success"
            });
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Invalid token.",
                    AlertType = "warning"
                });
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "User not found.",
                    AlertType = "warning"
                });
                return View();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                _basketService.InitializeBasket(user.Id);
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Info",
                    Message = "Account was confirmed.",
                    AlertType = "success"
                });
                return View();
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Email not found.",
                    AlertType = "warning"
                });
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = token
            });
            await _emailSender.SendEmailAsync(email, "Reset Password", @$"Please <a href='https://localhost:5001{url}'>click</a> for reset password.");
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Invalid token.",
                    AlertType = "warning"
                });
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "User not found.",
                    AlertType = "warning"
                });
                return View();
            }
            var model = new ResetPasswordModel() { Token = token };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Warning",
                    Message = "Password Rules: Uppercase, Lowercase, Nonalphanumeric, Min Lenght: 6",
                    AlertType = "warning"
                });
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
