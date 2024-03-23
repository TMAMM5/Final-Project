using Final_Project.Models;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.GovernorateRepo;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class GovernorateController : Controller
    {
        IGovernorateRepository _governorateRepository;
        ICityRepository _cityRepository;
        public GovernorateController(IGovernorateRepository governorateRepository, ICityRepository cityRepository)
        {
            _governorateRepository = governorateRepository;
            _cityRepository = cityRepository;
        }
        public IActionResult Index()
        {
            List<Governorate> governorates = _governorateRepository.GetAll();
            return View(governorates);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Governorate governorate)
        {
            if (ModelState.IsValid)
            {
                _governorateRepository.Add(governorate);
                _governorateRepository.Save();
                return RedirectToAction("Index");
            }
            return View(governorate);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Governorate governorate = _governorateRepository.GetById(id);
            return View(governorate);
        }
        [HttpPost]
        public IActionResult Edit(Governorate governorate)
        {
            if (ModelState.IsValid)
            {
            Governorate newGovernorate = _governorateRepository.GetById(governorate.Id);
                newGovernorate.Name = governorate.Name;
            _governorateRepository.Update(newGovernorate);
            _governorateRepository.Save();
            return RedirectToAction("Index");
            }
            return View(governorate);
        }

        public IActionResult Details(int id)
        {
            List<City> citys = _cityRepository.GetAllCitiesByGovId(id);
            ViewBag.GovName = _governorateRepository.GetById(id).Name;
            return View(citys);
        }

        public IActionResult ChangeState(int id)
        {
            Governorate governorate = _governorateRepository.GetById(id);
            if(governorate == null)
            {
                return NotFound();
            }
            else
            {
                governorate.IsDeleted = !governorate.IsDeleted;
                _governorateRepository.Save();
                return Ok();

            }

        }

    }
}
