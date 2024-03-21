using Final_Project.Models;
using Final_Project.Repository.WeightSettingRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class WeightSettingController : Controller
    {
        private readonly IWeightSettingRepo _WeightSettingRepo;


        public WeightSettingController(IWeightSettingRepo weightSettingRepo)
        {
            _WeightSettingRepo = weightSettingRepo;

        }
        public IActionResult Index()
        {
            List<WeightSetting> WeightSetting = _WeightSettingRepo.GetAll();
            return View(WeightSetting);
        }
        public IActionResult Edit(int id)
        {
            WeightSetting weightSetting = _WeightSettingRepo.GetById(id);
            if (weightSetting == null)
            {
                return NotFound();
            }
            return View(weightSetting);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,DefaultSize,PriceForEachExtraKilo")] WeightSetting weightSetting)
        {
            if (id != weightSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _WeightSettingRepo.Update(weightSetting);
                    _WeightSettingRepo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeightSettingExists(weightSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(weightSetting);
        }

        private bool WeightSettingExists(int id)
        {
            return _WeightSettingRepo.GetById(id) != null;
        }
    }
}