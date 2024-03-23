using Final_Project.Models;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.OrderRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBranchRepository _branchRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStateRepository _orderStateRepository;

        public EmployeeController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOrderRepository orderRepository,
            IOrderStateRepository orderStateRepository,
            IBranchRepository branchRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _orderRepository = orderRepository;
            _branchRepository = branchRepository;
            _orderStateRepository = orderStateRepository;
        }


        public async Task<IActionResult> Index()
        {
            var usersFromDb = await _userManager.Users.ToListAsync();
            var users = usersFromDb
                .Where(u => !_userManager.IsInRoleAsync(u, "Representative").Result && !_userManager.IsInRoleAsync(u, "Trader").Result)
                .Select(user => new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    creationDate = user.creationDate,
                    Branch = user.Branch,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    IsDeleted = user.IsDeleted
                })
                .ToList();
            return View(users);
        }
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Where(r=>r.Name !="Representative" &&r.Name !="Trader").Select(r=>new RoleVM
            {
                Id=r.Id,
                Name =r.Name
            }).ToList();
            var user = new UserFormVM
            {
                Roles = roles,
                Branches = _branchRepository.GetAll(),
            };
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(UserFormVM userForm)
        {
            var roles = _roleManager.Roles.Where(r => r.Name != "Representative" && r.Name != "Trader").Select(r => new RoleVM
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            userForm.Roles = roles;
            userForm.Branches = _branchRepository.GetAll();
            if(!ModelState.IsValid)
            {
                return View(userForm);

            }
            if(await _userManager.FindByEmailAsync(userForm.Email) != null)
            {
                ModelState.AddModelError("Email", "This Email Is Already Taken");
                return View(userForm);
            }
            var user = new ApplicationUser
            {
                Name = userForm.Name,
                Email = userForm.Email,
                UserName = userForm.Name,
                PhoneNumber = userForm.PhoneNumber,
                Address = userForm.Address,
                BranchId = userForm.BranchId,
            };
            var result = await _userManager.CreateAsync(user, userForm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Role", error.Description);
                    return View(userForm);
                }
            }
            await _userManager.AddToRoleAsync(user, userForm.RoleName);
            await _userManager.SetPhoneNumberAsync(user, userForm.PhoneNumber);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(string id)
        {
            

            if (id == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var roles = _roleManager.Roles.Where(r => r.Name != "Representative" && r.Name != "Trader").Select(r => new RoleVM
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            var model = new UpdateUserVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                BranchId = user.BranchId,
                RoleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                Roles = roles,
                Branches = _branchRepository.GetAll()

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserVM model)
        {
            var roles = _roleManager.Roles.Where(r => r.Name != "Representative" && r.Name != "Trader").Select(r => new RoleVM
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
            model.Roles = roles;
            model.Branches = _branchRepository.GetAll();
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);
            var checkUser = await _userManager.FindByEmailAsync(model.Email);
            if (checkUser != null && checkUser.Id != model.Id)
            {
                ModelState.AddModelError("Email", "This Email Is Already Taken");
                return View(model);
            }
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Address = model.Address;
            user.BranchId = model.BranchId;
            var userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, model.RoleName);
            await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Role", error.Description);
                    return View(model);
                }
            }


            return RedirectToAction(nameof(Index));
        }
       
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            else
            {
                user.IsDeleted = !user.IsDeleted;
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Content("Error On Delete");
            }

            return Ok(result);
        }

     
        public IActionResult Home()
        {

            var username = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(username).Result;
            ViewBag.EmployeeName = user;

            List<Order> orders = _orderRepository.GetAll();

            List<int> OrderStatusNumbers = new List<int>();

            foreach (var orderStatus in _orderStateRepository.GetOrders())
            {
                int count = 0;
                foreach (var order in orders)
                {
                    if (order.OrderStateId == orderStatus.Id)
                    {
                        count++;
                    }
                }
                OrderStatusNumbers.Add(count);

            }


            ViewBag.OrderStatus = _orderStateRepository.GetOrders();
            ViewBag.OrderStatusNumbers = OrderStatusNumbers;


            return View();

        }
    }
}
