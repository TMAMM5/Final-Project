using Final_Project.Repository.OrderRepo;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class OrderController : Controller
    {
        IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
