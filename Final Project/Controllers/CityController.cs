using Final_Project.Models;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.GovernorateRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class CityController : Controller
    {
        ICityRepository cityRepository;
        IGovernorateRepository governorateRepository;
        public CityController(IGovernorateRepository governorateRepository , ICityRepository cityRepository)
        {
            this.cityRepository = cityRepository;
            this.governorateRepository = governorateRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            List<City> cityList = cityRepository.GetAll();

            return View(cityList);
        }
        [Authorize]
        public IActionResult Create(int id) 
        {
            ViewData["Governrates"]= governorateRepository.GetAll();
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create(City city)
        {
            if(ModelState.IsValid)
            {
                cityRepository.Add(city);
                cityRepository.Save();
                return RedirectToAction("Index");
            }
                ViewData["Governrates"] = governorateRepository.GetAll();
                return View(city);     
        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            City city = cityRepository.GetById(id);
            ViewData["Governrates"] = governorateRepository.GetAll();
            return View(city) ;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(City city)
        {
            if (ModelState.IsValid)
            {
                var newCity = cityRepository.GetById(city.Id);
                city.IsDeleted = newCity.IsDeleted;
                newCity.Name = city.Name;
                newCity.ShippingCost = city.ShippingCost;
                newCity.PickUpCost = city.PickUpCost;
                newCity.Governorate = city.Governorate;
                cityRepository.Edit(newCity);
                cityRepository.Save();
                return RedirectToAction("Index");
            }
                ViewData["Governrates"] = governorateRepository.GetAll();
                return View(city);   
        }
        [Authorize]
        public IActionResult Delete(int id)
        {
            City city = cityRepository.GetById(id);
            if (city == null)
            {
                return NotFound();
            }
            city.IsDeleted = !city.IsDeleted;
            cityRepository.Save();
            return Ok();
        }
    }
}
