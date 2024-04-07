using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.OrderRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Permissions.Users.View)]

        public async Task<IActionResult> Index(int pg=1)
        {
            var usersFromDb = await _userManager.Users.ToListAsync();
            var users = usersFromDb
                .Where(u => !_userManager.IsInRoleAsync(u, "Representative").Result && !_userManager.IsInRoleAsync(u, "Trader").Result && !_userManager.IsInRoleAsync(u,Roles.SuperAdmin.ToString()).Result)
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


            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = users.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = users.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Employee";
            pager.Action = "Index";
            this.ViewBag.pager = pager;



            return View(data);
        }
        [Authorize(Permissions.Users.Create)]

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
        [Authorize(Permissions.Users.Create)]

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
            var user = new ApplicationUser();

            user.Name = userForm.Name;
            user.Email = userForm.Email;
            user.UserName = userForm.Email;
            user.PhoneNumber = userForm.PhoneNumber;
            user.Address = userForm.Address;
            user.BranchId = userForm.BranchId;
               
                
      
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
        [Authorize(Permissions.Users.Edit)]

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
        [Authorize(Permissions.Users.Edit)]

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
        [Authorize(Permissions.Users.Delete)]

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

        [Authorize(Permissions.Users.View)]

        public IActionResult Home()
        {

            var username = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(username).Result;
            ViewBag.EmployeeName = user.Name;

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
