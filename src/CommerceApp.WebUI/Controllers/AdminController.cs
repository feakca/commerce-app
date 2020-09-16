using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.Entity;
using CommerceApp.WebUI.Extensions;
using CommerceApp.WebUI.Identity;
using CommerceApp.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApp.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    [AutoValidateAntiforgeryToken]
    public class AdminController : Controller
    {
        private IOrderService _orderService;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController(IOrderService orderService, IProductService productService, ICategoryService categoryService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Admin");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var members = new List<User>();
            var nonMembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    }
                }
                foreach (var userId in model.IdsToRemove ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                    }
                }
            }
            return RedirectToAction("RoleEdit", "Admin", new
            {
                id = model.RoleId
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductList()
        {
            ProductListViewModel productListViewModel = new ProductListViewModel()
            {
                Products = _productService.GetAll()
            };
            return View(productListViewModel);
        }

        public IActionResult CategoryList()
        {
            CategoryListViewModel categoryListViewModel = new CategoryListViewModel()
            {
                Categories = _categoryService.GetAll()
            };
            return View(categoryListViewModel);
        }

        public IActionResult ProductCreate()
        {
            ViewBag.categories = _categoryService.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model, int[] categoryIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description,
                    Price = model.Price,
                    ImageUrl = model.ImageUrl,
                    Url = Helper.UrlCreater(model.Name)
                };

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var randomName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/images", randomName);
                    entity.ImageUrl = randomName;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                if (_productService.Create(entity, categoryIds))
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Info",
                        Message = $"{entity.Name} isimli ürün eklendi.",
                        AlertType = "success"
                    });
                    return RedirectToAction("ProductList");
                }
                else
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Warning",
                        Message = "En az bir kategori seçiniz.",
                        AlertType = "warning"
                    });
                    ViewBag.categories = _categoryService.GetAll();
                    return View(model);
                }
            }
            ViewBag.categories = _categoryService.GetAll();
            return View(model);
        }

        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name.Trim(),
                    Url = Helper.UrlCreater(model.Name)
                };

                _categoryService.Create(entity);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Info",
                    Message = $"{entity.Name} isimli kategori eklendi.",
                    AlertType = "success"
                });
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }

        public IActionResult ProductEdit(int? id)
        {
            if (id == null) return NotFound();

            var entity = _productService.GetByIdWithCategories((int)id);
            if (entity == null) return NotFound();

            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };
            ViewBag.categories = _categoryService.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model, int[] categoryIds, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.ProductId);
                if (entity == null) return NotFound();

                entity.Name = model.Name.Trim();
                entity.Description = model.Description;
                entity.Price = model.Price;
                entity.Url = Helper.UrlCreater(model.Name);
                entity.IsApproved = model.IsApproved;
                entity.IsHome = model.IsHome;

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var randomName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/images", randomName);
                    entity.ImageUrl = randomName;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                if (_productService.Update(entity, categoryIds))
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Info",
                        Message = $"{entity.Name} isimli ürün güncellendi.",
                        AlertType = "success"
                    });
                    return RedirectToAction("ProductList");
                }
                else
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Warning",
                        Message = "En az bir kategori seçiniz.",
                        AlertType = "warning"
                    });
                    ViewBag.categories = _categoryService.GetAll();
                    return View(model);
                }
            }
            ViewBag.categories = _categoryService.GetAll();
            return View(model);
        }

        public IActionResult CategoryEdit(int? id)
        {
            if (id == null) return NotFound();

            var entity = _categoryService.GetByIdWithProducts((int)id);
            if (entity == null) return NotFound();

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p => p.Product).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _categoryService.GetById(model.CategoryId);
                if (entity == null) return NotFound();

                entity.Name = model.Name.Trim();
                entity.Url = Helper.UrlCreater(model.Name);

                _categoryService.Update(entity);

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Info",
                    Message = $"{entity.Name} isimli kategori güncellendi.",
                    AlertType = "success"
                });
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ProductDelete(int? id)
        {
            if (id == null) return NotFound();

            var entity = _productService.GetById((int)id);
            if (entity == null) return NotFound();

            _productService.Delete(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Info",
                Message = $"{entity.Name} isimli ürün silindi.",
                AlertType = "danger"
            });
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public IActionResult CategoryDelete(int? id)
        {
            if (id == null) return NotFound();

            var entity = _categoryService.GetById((int)id);
            if (entity == null) return NotFound();

            _categoryService.Delete(entity);


            TempData.Put("message", new AlertMessage()
            {
                Title = "Info",
                Message = $"{entity.Name} isimli kategori silindi.",
                AlertType = "danger"
            });
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult ProductDeleteFromCategory(int? productId, int? categoryId)
        {
            if (productId == null) return NotFound();

            var entityProduct = _productService.GetById((int)productId);
            if (entityProduct == null) return NotFound();

            if (categoryId == null) return NotFound();

            var entityCategory = _categoryService.GetById((int)categoryId);
            if (entityCategory == null) return NotFound();

            _categoryService.ProductDeleteFromCategory((int)productId, (int)categoryId);
            return RedirectToAction("CategoryEdit", new { id = categoryId });
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users.ToList());
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i => i.Name).ToList();

                ViewBag.roles = roles;
                return View(new UserDetailModel()
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles.ToList()
                });
            }

            TempData.Put("message", new AlertMessage()
            {
                Title = "Info",
                Message = "User not found.",
                AlertType = "danger"
            });
            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailModel model, string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName.Trim();
                    user.LastName = model.LastName.Trim();
                    user.UserName = model.UserName.Trim();
                    user.Email = model.Email.Trim();
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles ?? new string[] { };
                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray<string>());

                        TempData.Put("message", new AlertMessage()
                        {
                            Title = "Info",
                            Message = $"{model.FirstName} {model.LastName} isimli kullanıcı bilgileri güncellendi.",
                            AlertType = "success"
                        });
                        return RedirectToAction("UserList");
                    }
                }
            }
            else
            {
                var userGet = await _userManager.FindByIdAsync(model.UserId);
                if (userGet != null)
                {
                    var selectedRolesGet = await _userManager.GetRolesAsync(userGet);
                    var rolesGet = _roleManager.Roles.Select(i => i.Name).ToList();

                    ViewBag.roles = rolesGet;
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Info",
                        Message = "Model is not valid.",
                        AlertType = "Danger"
                    });
                    return View(new UserDetailModel()
                    {
                        UserId = userGet.Id,
                        FirstName = userGet.FirstName,
                        LastName = userGet.LastName,
                        UserName = userGet.UserName,
                        Email = userGet.Email,
                        EmailConfirmed = userGet.EmailConfirmed,
                        SelectedRoles = selectedRolesGet.ToList()
                    });
                }
            }

            TempData.Put("message", new AlertMessage()
            {
                Title = "Info",
                Message = "User not found.",
                AlertType = "warning"
            });
            return RedirectToAction("UserEdit", new
            {
                id = model.UserId
            });
        }

        public IActionResult GetOrders()
        {
            var orders = _orderService.GetOrders(null);

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

        [HttpPost]
        public IActionResult GetOrders(int orderId, string[] orderStates)
        {
            _orderService.UpdateOrderState(orderId, orderStates);

            var orders = _orderService.GetOrders(null);

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
