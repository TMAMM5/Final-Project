using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Final_Project.Repository.BranchRepo;
using Microsoft.AspNetCore.Authorization;
using Final_Project.Repository.DiscountTypeRepo;


namespace Final_Project.Controllers
{
    public class DiscountTypeController : Controller
    {
        IDiscountTypeRepository DiscountTypeRepository;
        public DiscountTypeController(IDiscountTypeRepository discountTypeRepository)
        {
            this.DiscountTypeRepository = discountTypeRepository;
        }

        

        public IActionResult Index()
        {
            List<DiscountType> discountTypes;
            discountTypes = DiscountTypeRepository.GetAll();
            return View(discountTypes);
        }
        public IActionResult Details(int id)
        {
            var city = DiscountTypeRepository.GetById(id);
            return View(city);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult create(DiscountType discountType)
        {
            if (ModelState.IsValid)
            {
                DiscountTypeRepository.Add(discountType);
                DiscountTypeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(discountType);
        }

        public IActionResult Update(int id)
        {
            DiscountType discount = DiscountTypeRepository.GetById(id);
            return View(discount);
        }
        [HttpPost]
        public IActionResult Update(DiscountType discount)
        {
            if (ModelState.IsValid)
            {
                DiscountTypeRepository.Update(discount);
                DiscountTypeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(discount);

        }
        public IActionResult Delete(int id)
        {
            DiscountTypeRepository.Delete(id);
            DiscountTypeRepository.Save();
            return RedirectToAction("Index");
        }


    }
}
