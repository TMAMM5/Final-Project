using Final_Project.Repository.OrderRepo;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.DeliverTypeRepo;
using Final_Project.Repository.GovernorateRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.Repository.OrderTypeRepo;
using Final_Project.Repository.PaymentMethodRepo;
using Final_Project.Repository.TraderRepo;
using Final_Project.Repository.ProductRepo;
using Final_Project.Repository.RepresintativeRepo;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
using Final_Project.ViewModel;


namespace Final_Project.Controllers
{
    public class OrderController : Controller
    {
        IOrderRepository _orderRepository;
        IBranchRepository _branchRepository;
        IOrderTypeRepository _orderTypeRepository;
        IDeliverTypeRepository _deliverTypeRepository;
        IPaymentMethodRepository _paymentMethodRepository;
        IGovernorateRepository _governorateRepository;
        ICityRepository _cityRepository;
        IOrderStateRepository _orderStateRepository;
        IProductRepositoy _productRepository;
        ITraderRepository _traderRepository;
        UserManager<ApplicationUser> _userManager;
        IRepresintativeRepository _represintativeRepository;
        public OrderController(IOrderRepository orderRepository,
            IBranchRepository branchRepository,
            IOrderTypeRepository orderTypeRepository,
            IDeliverTypeRepository deliverTypeRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IGovernorateRepository governRepository,
            ICityRepository cityRepository,
            IOrderStateRepository orderStateRepository,
            IProductRepositoy productRepository,
            ITraderRepository traderRepository,
            UserManager<ApplicationUser> userManager,
           IRepresintativeRepository representativeRepository
            )
        {
            _orderRepository = orderRepository;
            _branchRepository = branchRepository;
            _orderTypeRepository = orderTypeRepository;
            _deliverTypeRepository = deliverTypeRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _governorateRepository = governRepository;
            _cityRepository = cityRepository;
            _orderStateRepository = orderStateRepository;
            _productRepository = productRepository;
            _traderRepository = traderRepository;
            _userManager = userManager;
            _represintativeRepository = representativeRepository;

        }
        public IActionResult Index()
        {
            List<Order> orders;
            if (User.IsInRole("Trader"))
            {
                var username = User.Identity.Name;
                var user = _userManager.FindByEmailAsync(username).Result;
                ViewBag.TraderId = user.Id.ToString();

                orders = _orderRepository.GetAll();
                orders = orders.Where(o => o.TraderId == user.Id).ToList();
            }
            else
            {
                orders = _orderRepository.GetAll();
            }
            List<OrderReporttWithOrderByStatusDateVM> OrdersViewModel = new List<OrderReporttWithOrderByStatusDateVM>();

            //error * **************************
            foreach (var item in orders)
            {
                City city = _cityRepository.GetById((int)item.ClientCityId);
                Governorate governorate = _governorateRepository.GetById((int)item.ClientGovernorateId);
                OrderState orderState = _orderStateRepository.getById((int)item.OrderStateId);
                Trader trader = _traderRepository.GetById(item.TraderId);

                OrderReporttWithOrderByStatusDateVM ordersViewModelItem = new OrderReporttWithOrderByStatusDateVM();
                
                
              
                ordersViewModelItem.Id= item.Id;
                ordersViewModelItem.Date = item.creationDate;
                ordersViewModelItem.Client = item.ClientName;
                ordersViewModelItem.PhoneNumber = item.Phone1;
                ordersViewModelItem.City = city.Name;
                ordersViewModelItem.Governorate = governorate.Name;
                ordersViewModelItem.ShippingPrice = (decimal)item.ShippingPrice;
                ordersViewModelItem.Status = orderState.Name;
                ordersViewModelItem.Trader = trader.AppUser.Name;

                OrdersViewModel.Add(ordersViewModelItem);
            }

            ViewData["OrderStates"] = _orderStateRepository.GetOrders();

            return View(OrdersViewModel);
        }
        public IActionResult Details(int id)
        {

            Order order = _orderRepository.GetById(id);
            return View(order);
        }


        public IActionResult Invoice(int id)
        {
            var order = _orderRepository.GetById(id);

            var products = _productRepository.GetByOrderNumber(order.OrderNo);

            ViewData["Products"] = products;
            return View(order);
        }
        public IActionResult Create()
        {
            ViewData["DeliverTypes"] = _deliverTypeRepository.GetAll();
            ViewData["OrderTypes"] = _orderTypeRepository.GetAll();
            ViewData["PaymentMethods"] = _paymentMethodRepository.GetAll();
            ViewData["Branches"] = _branchRepository.GetAll();
            ViewData["Governorates"] = _governorateRepository.GetAll();
            ViewBag.OrderNo = Guid.NewGuid().ToString();
            var username = User.Identity.Name;
            var user = _userManager.FindByEmailAsync(username).Result;
            ViewBag.TraderId = user.Id.ToString();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            ViewData["DeliverTypes"] = _deliverTypeRepository.GetAll();
            ViewData["OrderTypes"] = _orderTypeRepository.GetAll();
            ViewData["PaymentMethods"] = _paymentMethodRepository.GetAll();
            ViewData["Branches"] = _branchRepository.GetAll();
            ViewData["Governorates"] = _governorateRepository.GetAll();

            if (ModelState.IsValid)
            {
                order.TotalWeight = ProductsWeight(order.OrderNo);
                order.ShippingPrice = _orderRepository.CalculateTotalPrice(order) /*+ ProductsCost(order.OrderNo)*/;
                order.OrderPrice = order.ShippingPrice + ProductsCost(order.OrderNo);
                order.Products = _productRepository.GetByOrderNumber(order.OrderNo);
                _orderRepository.Add(order);
                _orderRepository.Save();
                return RedirectToAction("Index");
            }
            return View(order);
        }
        
        public IActionResult getCitesByGovernrate(int govId)
        {
            List<City> cities = _cityRepository.GetAllCitiesByGovId(govId);
            return Json(cities);
        }
        public IActionResult Edit(int id)
        {
            ViewData["DeliverTypes"] = _deliverTypeRepository.GetAll();
            ViewData["OrderTypes"] = _orderTypeRepository.GetAll();
            ViewData["PaymentMethods"] = _paymentMethodRepository.GetAll();
            ViewData["Branches"] = _branchRepository.GetAll();
            ViewData["Governorates"] = _governorateRepository.GetAll();

            Order order = _orderRepository.GetById(id);

            ViewData["City"] = _cityRepository.GetAll().Where(c => c.GoverId == order.ClientGovernorateId).ToList();
            //  EDIT=>REPO ..... Getproducts()=>Getall()
            order.Products = _productRepository.GetProducts().Where(p => p.OrderNO == order.OrderNo).ToList();
            
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                order.TotalWeight = ProductsWeight(order.OrderNo);
                order.ShippingPrice = _orderRepository.CalculateTotalPrice(order) /*+ ProductsCost(order.OrderNo)*/;
                
                order.OrderPrice = order.ShippingPrice + ProductsCost(order.OrderNo);
                //GetByOrderNumber=>GetByOrderNo
                order.Products = _productRepository.GetByOrderNumber(order.OrderNo);
                //update=>Edit
                _orderRepository.Update(order);
                _orderRepository.Save();
                return RedirectToAction("Index");
            }

            ViewData["DeliverTypes"] = _deliverTypeRepository.GetAll();
            ViewData["OrderTypes"] = _orderTypeRepository.GetAll();
            ViewData["PaymentMethods"] = _paymentMethodRepository.GetAll();
            ViewData["Branches"] = _branchRepository.GetAll();
            ViewData["Governorates"] = _governorateRepository.GetAll();

            return View(order);
        }
        public IActionResult Delete(int id)
        {
            Order order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            _orderRepository.Delete(id);
            _orderRepository.Save();
            return Ok();
        }
        public decimal? ProductsWeight(string orderNO)
        {
            var orderProducts = _productRepository.GetProducts().Where(p => p.OrderNO == orderNO);
            decimal? weight = 0;
            foreach (var product in orderProducts)
            {
                weight += product.Weight * product.Quantity;
            }
            return weight;

        }
        public decimal? ProductsCost(string orderNO)
        {
            var orderProducts = _productRepository.GetProducts().Where(p => p.OrderNO == orderNO);
            decimal? cost = 0;
            foreach (var product in orderProducts)
            {
                cost += product.Price * product.Quantity;
            }
            return cost;
        }
    }
}

    

