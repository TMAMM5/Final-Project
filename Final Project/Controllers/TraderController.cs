using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.GovernorateRepo;
using Final_Project.Repository.OrderRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.Repository.TraderRepo;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class TraderController : Controller
    {
        ITraderRepository _traderRepository;
        IGovernorateRepository _governRepository;
        ICityRepository _cityRepository;
        IBranchRepository _branchRepository;
        IOrderRepository _orderRepository;
        UserManager<ApplicationUser> _userManager;
        IOrderStateRepository _orderStateRepository;

        public TraderController(ITraderRepository traderRepository,
            IGovernorateRepository governRepository,
            ICityRepository cityRepository,
            IBranchRepository branchRepository,
            IOrderRepository orderRepository,
            UserManager<ApplicationUser> userManager,
            IOrderStateRepository orderStateRepository)
        {
            _branchRepository = branchRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _traderRepository = traderRepository;
            _governRepository = governRepository;
            _cityRepository = cityRepository;
            _orderStateRepository = orderStateRepository;
        }

        public IActionResult Index()
        {
            var trader = _traderRepository.GetAll();
            return View(trader);
        }

        public async Task<IActionResult> Create()
        {
            var trader = new TraderVM
            {
                Governorates = _governRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                Cities = _cityRepository.GetAll()

            };
            return View(trader);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TraderVM trader)
        {
            trader.Governorates = _governRepository.GetAll();
            trader.Branchs = _branchRepository.GetAll();
            trader.Cities = _cityRepository.GetAll();
            if (!ModelState.IsValid)
            {
                return View(trader);
            }
            if (await _userManager.FindByEmailAsync(trader.Email) != null)
            {
                ModelState.AddModelError("Email", "Email is already exist");
                return View(trader);
            }
            var user = new ApplicationUser
            {
                Email = trader.Email,
                UserName = trader.Email,
                Name = trader.Name,
                PhoneNumber = trader.Phone,
                Address = trader.Address,
                BranchId = trader.BranchId
            };

            var result = await _userManager.CreateAsync(user, trader.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(trader);
                }
            }
            await _userManager.AddToRoleAsync(user, Roles.Trader.ToString());
            await _userManager.SetPhoneNumberAsync(user, trader.Phone);

            var traders = new Trader
            {
                AppUserId = user.Id,
                GoverId = trader.GoverId,
                BranchId = trader.BranchId,
                CityId = trader.CityId,
                SpecialPickupCost = trader.SpecialPickupCost,
                StoreName = trader.StoreName,
                TraderTaxForRejectedOrders = trader.TraderTaxForRejectedOrders,
                IsDeleted = trader.IsDeleted,
            };
            _traderRepository.Add(traders);
            _traderRepository.Save();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var trader = _traderRepository.GetById(id);
            if (trader == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            var model = new TraderVM
            {
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Address = user.Address,
                Password = "Admin2024",
                BranchId = user.BranchId,
                GoverId = (int)trader.GoverId,
                CityId = (int)trader.CityId,
                SpecialPickupCost = (int)trader.SpecialPickupCost,
                StoreName = trader.StoreName,
                TraderTaxForRejectedOrders = (int)trader.TraderTaxForRejectedOrders,
                IsDeleted = trader.IsDeleted,
                Governorates = _governRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                Cities = _cityRepository.GetAll()
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TraderVM model)
        {
            var user = await _userManager.FindByIdAsync(model.AppUserId);
            if (user == null)
            {
                return NotFound();
            }
            model.Governorates = _governRepository.GetAll();
            model.Branchs = _branchRepository.GetAll();
            model.Cities = _cityRepository.GetAll();
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
            user.BranchId = model.BranchId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(model);
                }
            }
            var traderFDB = _traderRepository.GetById(model.AppUserId);

            traderFDB.GoverId = model.GoverId;
            traderFDB.BranchId = model.BranchId;
            traderFDB.CityId = model.CityId;
            traderFDB.SpecialPickupCost = model.SpecialPickupCost;
            traderFDB.StoreName = model.StoreName;
            traderFDB.TraderTaxForRejectedOrders = model.TraderTaxForRejectedOrders;
            _traderRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return BadRequest();
            var trader = _traderRepository.GetById(id);
            if (trader == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            user.IsDeleted = !user.IsDeleted;
            trader.IsDeleted = !trader.IsDeleted;
            _traderRepository.Save();
            return Ok();
        }


        public IActionResult Home()
        {

            var username = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(username).Result;
            ViewBag.TraderName = user.Name;

            List<Order> orders = _orderRepository.GetAll().Where(o => o.TraderId == user.Id).ToList();

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
