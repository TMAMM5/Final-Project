using Final_Project.Models;
using Final_Project.Repository.BranchRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class BranchController : Controller
    {
        IBranchRepository branchRepository;
        public BranchController(IBranchRepository branchRepository)
        {
            this.branchRepository = branchRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            List<Branch> branchs = branchRepository.GetAll();
            return View(branchs);
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public  IActionResult Create(Branch branch)
        {
            if (ModelState.IsValid) 
            {
                 branchRepository.Add(branch);
                 branchRepository.Save();
                 return RedirectToAction("Index");
            }
            return View(branch);
        }


        [Authorize]
        public IActionResult Edit(int id)
        {
            Branch branch = branchRepository.GetById(id);
            return View(branch);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                branchRepository.Edit(branch);
                branchRepository.Save();
                return RedirectToAction("Index");
            }
            return View(branch);

        }
        [Authorize]
        public IActionResult Delete(int id)
        {
            Branch branch = branchRepository.GetById(id);
            if(branch == null)
            {
                return NotFound();
            }
            else
            {
                branch.IsDeleted = !branch.IsDeleted;
                branchRepository.Save();
                return Ok();
            }
        }
    }
}
