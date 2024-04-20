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
using Final_Project.Needs;
using static Final_Project.Needs.Permissions;



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
        [Authorize(Permissions.Orders.View)]

        public IActionResult Index(int pg = 1)
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
                Models.City city = _cityRepository.GetById((int)item.ClientCityId);
                Models.Governorate governorate = _governorateRepository.GetById((int)item.ClientGovernorateId);
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
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = OrdersViewModel.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = OrdersViewModel.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Employee";
            pager.Action = "Index";
            this.ViewBag.pager = pager;

            return View(data);
        }
        [Authorize(Permissions.Orders.View)]

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
        [Authorize(Permissions.Orders.Create)]

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
        [Authorize(Permissions.Orders.Create)]

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
            List<Models.City> cities = _cityRepository.GetAllCitiesByGovId(govId);
            return Json(cities);
        }
        [Authorize(Permissions.Orders.Edit)]

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
        [Authorize(Permissions.Orders.Edit)]
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
            ViewData["City"] = _cityRepository.GetAll().Where(c => c.GoverId == order.ClientGovernorateId).ToList();
            return View(order);
        }
        [Authorize(Permissions.Orders.Delete)]

        public IActionResult Delete(int id)
        {
            Order order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            _orderRepository.Delete(id);
            _orderRepository.Save();
            return RedirectToAction("Index");
        }
        [Authorize(Permissions.Orders.View)]

        public IActionResult GetFilteredOrders(int orderState)
        {
            List<OrderReporttWithOrderByStatusDateVM> filteredOrdersViewModel =
                new List<OrderReporttWithOrderByStatusDateVM>();
            List<Order> filteredOrders;
            if (User.IsInRole("Trader"))
            {

                var username = User.Identity.Name;
                var user = _userManager.FindByNameAsync(username).Result;
                var TraderId = user.Id.ToString();

                filteredOrders = _orderRepository.GetAll();
                filteredOrders = filteredOrders.Where(o => o.TraderId == TraderId).ToList();
            }
            else
            {
                filteredOrders = _orderRepository.GetAll();
            }
            if (orderState == 0)
            {
            }
            else
            {
                filteredOrders = filteredOrders.Where(o => o.OrderStateId == orderState).ToList();
            }
            foreach (var item in filteredOrders)
            {

                Models.City city = _cityRepository.GetById((int)item.ClientCityId);
                Models.Governorate governorate = _governorateRepository.GetById((int)item.ClientGovernorateId);
                // try in view 
                OrderState orderS = _orderStateRepository.getById((int)item.OrderStateId);
                Trader trader = _traderRepository.GetById(item.TraderId);

                OrderReporttWithOrderByStatusDateVM ordersViewModelItem =
                    new OrderReporttWithOrderByStatusDateVM();
                ordersViewModelItem.Id = item.Id;
                ordersViewModelItem.Date = item.creationDate;
                ordersViewModelItem.Client = item.ClientName;
                ordersViewModelItem.PhoneNumber = item.Phone1;
                ordersViewModelItem.City = city.Name;
                ordersViewModelItem.Governorate = governorate.Name;
                ordersViewModelItem.ShippingPrice = (decimal)item.ShippingPrice;
                // try in view
                ordersViewModelItem.Status = orderS.Name;
                ordersViewModelItem.Trader = trader.AppUser.Name;


                filteredOrdersViewModel.Add(ordersViewModelItem);
            }



            return Json(filteredOrdersViewModel);
        }


        [Authorize(Permissions.Orders.Create)]

        public IActionResult AddProduct(string name, int quantity, decimal weight, decimal price, string orderno)
        {
            var pro = new Product
            {
                OrderNO = orderno,
                Name = name,
                Quantity = quantity,
                Weight = weight,
                Price = price
            };
            _productRepository.Add(pro);
            _productRepository.Save();

            return Ok(pro.Id);
        }

        [Authorize(Permissions.Orders.Delete)]

        public IActionResult DeleteProduct(int id)
        {
            if (id == null)
                return BadRequest();
            var pro = _productRepository.GetById(id);
            if (pro == null)
                return NotFound();
            _productRepository.Delete(id);
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

        public async Task<IActionResult> Status(int id)
        {
            Order order = _orderRepository.GetById(id);

            var representativesInSameCity = _represintativeRepository.GetByBranch((int)order.BranchId);

            List<RepresentativeGovBranchPercentageViewModel> viewmodels = new List<RepresentativeGovBranchPercentageViewModel>();

            foreach (var item in representativesInSameCity)
            {
                var user = await _userManager.FindByIdAsync(item.AppUserId);

                RepresentativeGovBranchPercentageViewModel viewmodel = new RepresentativeGovBranchPercentageViewModel()
                {
                    AppUserId = user.Id,
                    Name = user.Name,
                };
                viewmodels.Add(viewmodel);
            }


            ViewData["OrderStatus"] = _orderStateRepository.GetForEmp();
            ViewData["RepresentativesInSameCity"] = viewmodels;
            return View(order);
        }

        [Authorize(Permissions.Orders.View)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Status(Order order)
        {
            //GetAll=>GetOrders
            ViewData["OrderStatus"] = _orderStateRepository.GetOrders();
            Order orderFromDB = _orderRepository.GetById(order.Id);
            orderFromDB.OrderStateId = order.OrderStateId;
            _orderRepository.Save();

            return RedirectToAction("Index");
        }
        [Authorize(Permissions.Orders.View)]

        public IActionResult LinkOrderToRepresentative(string orderId, string repId)
        {
            var order = _orderRepository.GetById(int.Parse(orderId));

            order.RepresentativeId = repId;

            _orderRepository.Save();

            return RedirectToAction("Index", "Order");
        }
        [Authorize(Permissions.OrderReports.View)]

        public IActionResult OrderReport(string startDate, string endDate, int statusId)
        {
            List<OrderReporttWithOrderByStatusDateVM> ordersViewModel = new List<OrderReporttWithOrderByStatusDateVM>();

            var orders = _orderRepository.GetAll().ToList();

            foreach (var item in orders)
            {
                OrderReporttWithOrderByStatusDateVM ordersViewModelItem = new OrderReporttWithOrderByStatusDateVM();
                ordersViewModelItem.SerialNumber = item.Id;
                ordersViewModelItem.StatusId = (int)item.OrderStateId;
                ordersViewModelItem.Status = item.OrderState.Name;
                ordersViewModelItem.Trader = item.Trader.AppUser.Name;
                ordersViewModelItem.Client = item.ClientName;
                ordersViewModelItem.PhoneNumber = item.Phone1;
                ordersViewModelItem.Governorate = item.ClientGovernorate.Name;
                ordersViewModelItem.City = item.ClientCity.Name;
                ordersViewModelItem.OrderPrice = (decimal)item.OrderPrice;
                ordersViewModelItem.OrderPriceRecieved = (decimal)item.OrderPriceRecieved;
                ordersViewModelItem.ShippingPrice = (decimal)item.ShippingPrice;
                ordersViewModelItem.ShippingPriceRecived = (decimal)item.ShippingPriceRecived;
                ordersViewModelItem.CompanyRate = (decimal)(item.RepresentativeId == null ? 0 : (item.Representative.CompanyPercentageOfOrder * ordersViewModelItem.ShippingPrice) / 100M);
                ordersViewModelItem.Date = item.creationDate;
                ordersViewModel.Add(ordersViewModelItem);

            }

            if (statusId != 0)
            {
                ordersViewModel = ordersViewModel.Where(o => o.StatusId == statusId).ToList();
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate).AddDays(1);
                ordersViewModel = ordersViewModel.Where(o => o.Date >= start && o.Date < end).ToList();

            }

            ViewBag.status = _orderStateRepository.GetOrders();

            return View(ordersViewModel);
        }
    }
}

    

