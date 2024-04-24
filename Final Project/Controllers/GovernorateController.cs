using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.GovernorateRepo;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Permissions.Governorate.View)]

        public IActionResult Index(string childname, int pg = 1)
        {
            if (String.IsNullOrEmpty(childname))
            {

                List<Governorate> governorates = _governorateRepository.GetAll();
                const int pageSize = 9;
                if (pg < 1)
                    pg = 1;
                int recsCount = governorates.Count();
                var pager = new Pager(recsCount, pg, pageSize);
                int recSkip = (pg - 1) * pageSize;
                var data = governorates.Skip(recSkip).Take(pager.PageSize).ToList();
                pager.Controller = "Governorate";
                pager.Action = "Index";
                this.ViewBag.pager = pager;
                return View(data);
            }
            else
            {
                var searchItems = _governorateRepository.GetAll().Where(s => s.Name.ToLower().Contains(childname.ToLower())).ToList();
                return View(searchItems);
            }
        }
        [Authorize(Permissions.Governorate.Create)]

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize(Permissions.Governorate.Create)]

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [Authorize(Permissions.Governorate.Edit)]
        [HttpGet]
        
        public IActionResult Edit(int id)
        {
            Governorate governorate = _governorateRepository.GetById(id);
            return View(governorate);
        }
        [Authorize(Permissions.Governorate.Edit)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Governorate governorate)
        {
            var newGovernorate = _governorateRepository.GetById(governorate.Id);
            newGovernorate.Name = governorate.Name;
            if (ModelState.IsValid)
            {
                _governorateRepository.Update(newGovernorate);
                _governorateRepository.Save();
                return RedirectToAction("Index");
            }
            return View(governorate);
        }
        [Authorize(Permissions.Governorate.View)]

        public IActionResult Details(int id)
        {
            var citys = _cityRepository.GetAllCitiesByGovId(id);
            ViewBag.GovName = _governorateRepository.GetById(id).Name;
            return View(citys);
        }
        [Authorize(Permissions.Governorate.Delete)]

        public IActionResult ChangeState(int id)
        {
            Governorate governorate = _governorateRepository.GetById(id);
            if (governorate == null)
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
