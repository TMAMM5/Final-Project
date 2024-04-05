using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.DiscountTypeRepo;
using Final_Project.Repository.GovernorateRepo;
using Final_Project.Repository.OrderRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.Repository.RepresintativeRepo;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class RepresentativeController : Controller
    {
        private readonly IRepresintativeRepository _represintativeRepository;
        private readonly IGovernorateRepository _governorateRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IDiscountTypeRepository _discountTypeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStateRepository _orderStateRepository;

        public RepresentativeController(IRepresintativeRepository represintativeRepository, IGovernorateRepository governorateRepository, IBranchRepository branchRepository, IDiscountTypeRepository discountTypeRepository, UserManager<ApplicationUser> userManager, IOrderRepository orderRepository, IOrderStateRepository orderStateRepository)
        {
            _represintativeRepository = represintativeRepository;
            _governorateRepository = governorateRepository;
            _branchRepository = branchRepository;
            _discountTypeRepository = discountTypeRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderStateRepository = orderStateRepository;
        }
        [Authorize(Permissions.Users.View)]

        public IActionResult Index(int pg = 1)
        {
            List<Representative> representatives = _represintativeRepository.GetAll();
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = representatives.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = representatives.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Representative";
            pager.Action = "Index";
            this.ViewBag.pager = pager;
            return View(data);
        }


        [Authorize(Permissions.Representatives.View)]

        public IActionResult Home()
        {
            var username = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(username).Result;
            var RepresentativeId = user.Id.ToString();
            List<Order> orders = _orderRepository.GetByRepresentativeId(RepresentativeId);
            return View(orders);
        }




        [Authorize(Permissions.Users.Create)]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            RepresentativeGovBranchPercentageViewModel representativeVM = new RepresentativeGovBranchPercentageViewModel()
            {
                Governorates = _governorateRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                DiscountTypes = _discountTypeRepository.GetAll(),
            };
            return View(representativeVM);
        }
        [Authorize(Permissions.Users.Create)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepresentativeGovBranchPercentageViewModel representativeVM)
        {
            representativeVM.Governorates = _governorateRepository.GetAll();
            representativeVM.Branchs = _branchRepository.GetAll();
            representativeVM.DiscountTypes = _discountTypeRepository.GetAll();
            if (!ModelState.IsValid)
            {
                return View(representativeVM);
            }
            if (await _userManager.FindByEmailAsync(representativeVM.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View(representativeVM);
            }
            var user = new ApplicationUser
            {
                Email = representativeVM.Email,
                UserName = representativeVM.Email,
                Name = representativeVM.Name,
                PhoneNumber = representativeVM.Phone,
                Address = representativeVM.Address,
                BranchId = representativeVM.BranchId
            };
            var result = await _userManager.CreateAsync(user, representativeVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(representativeVM);
                }
            }
            await _userManager.AddToRoleAsync(user, Roles.Representative.ToString());
            await _userManager.SetPhoneNumberAsync(user, representativeVM.Phone);

            var representative = new Representative
            {
                AppUserId = user.Id,
                CompanyPercentageOfOrder = representativeVM.CompanyPercentageOfOrder,
                GovernorateId = representativeVM.GovernorateId,
                BranchId = representativeVM.BranchId,
                DiscountTypeId = representativeVM.DiscountTypeId,
                IsDeleted = representativeVM.IsDeleted,
            };
            _represintativeRepository.Add(representative);
            _represintativeRepository.Save();
            return RedirectToAction("Index");

        }
        [Authorize(Permissions.Users.Edit)]

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var representative = _represintativeRepository.GetById(id);
            if (representative == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            var model = new RepresentativeGovBranchPercentageViewModel
            {
                AppUserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Address = user.Address,
                CompanyPercentageOfOrder = (decimal)representative.CompanyPercentageOfOrder,
                //Password = "Admin2024",
                BranchId = user.BranchId,
                GovernorateId = (int)representative.GovernorateId,
                DiscountTypeId = (int)representative.DiscountTypeId,
                IsDeleted = representative.IsDeleted,
                Governorates = _governorateRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                DiscountTypes = _discountTypeRepository.GetAll(),
            };
            return View(model);
        }

        [Authorize(Permissions.Users.Edit)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RepresentativeGovBranchPercentageViewModel model)
        {
            //model.Password = "Admin2024";
            var user = await _userManager.FindByIdAsync(model.AppUserId);
            if (user == null)
            {
                return NotFound();
            }
            model.Governorates = _governorateRepository.GetAll();
            model.Branchs = _branchRepository.GetAll();
            model.DiscountTypes = _discountTypeRepository.GetAll();
            if (!ModelState.IsValid) { return View(model); }
            var checkUser = await _userManager.FindByEmailAsync(model.Email);
            if (checkUser != null && checkUser.Id != model.AppUserId)
            {
                ModelState.AddModelError("Email", "Email is already exist");
                return View(model);
            }
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Address = model.Address;
            user.BranchId = (int)model.BranchId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(model);
                }
            }
            var representativeFDB = _represintativeRepository.GetById(model.AppUserId);

            representativeFDB.GovernorateId = model.GovernorateId;
            representativeFDB.BranchId = model.BranchId;
            representativeFDB.DiscountTypeId = model.DiscountTypeId;
            representativeFDB.CompanyPercentageOfOrder = model.CompanyPercentageOfOrder;
            _represintativeRepository.Save();
            return RedirectToAction("Index");
        }
        [Authorize(Permissions.Users.Delete)]

        public async Task<IActionResult> changeState(string id)
        {
            if (id == null)
                return BadRequest();
            var representative = _represintativeRepository.GetById(id);
            if (representative == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            user.IsDeleted = !representative.IsDeleted;
            representative.IsDeleted = !representative.IsDeleted;
            _represintativeRepository.Save();
            return Ok();
        }

        [Authorize(Permissions.Representatives.View)]

        public ActionResult ChangeStatus(int orderId, int statusId)
        {
            if (orderId == null || statusId == null)
                return BadRequest();
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                return NotFound();
            order.OrderStateId = statusId;
            _orderRepository.Save();
            order = _orderRepository.GetById(orderId);
            return Ok(order.OrderState.Name);
        }

        [Authorize(Permissions.Representatives.View)]


        [HttpGet]
        public IActionResult ChangeOrderStatus(int id)
        {

            Order order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            List<OrderState> orderStates = _orderStateRepository.GetOrders();
            OrderWithOrderStateVM orderWithOrderStateVM = new OrderWithOrderStateVM
            {
                OrderID = order.Id,
                OrderStateId = order.OrderStateId
            };
            ViewBag.OrderState = orderStates;
            return View(orderWithOrderStateVM);
        }
        [Authorize(Permissions.Representatives.View)]


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeOrderStatus(OrderWithOrderStateVM orderVM)
        {
            if (ModelState.IsValid)
            {

                Order existingOrder = _orderRepository.GetById(orderVM.OrderID);
                if (existingOrder == null)
                {
                    return NotFound();
                }


                existingOrder.OrderStateId = orderVM.OrderStateId;

                _orderRepository.Update(existingOrder);
                _orderRepository.Save();

                return RedirectToAction("Home");
            }


            List<OrderState> orderStates = _orderStateRepository.GetOrders();
            ViewBag.OrderState = orderStates;
            return View(orderVM);
        }



    }
}
