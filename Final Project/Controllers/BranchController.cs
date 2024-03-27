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


        public IActionResult Index(int pg = 1)
        {
            List<Branch> branchs = branchRepository.GetAll();
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = branchs.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = branchs.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Branch";
            pager.Action = "Index";
            this.ViewBag.pager = pager;
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

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


        public IActionResult Edit(int id)
        {
            Branch branch = branchRepository.GetById(id);
            return View(branch);
        }

        [HttpPost]
        public IActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                    Branch newBranch = branchRepository.GetById(branch.Id);
                    newBranch.Name = branch.Name;
                    branchRepository.Edit(newBranch);
                    branchRepository.Save();
                    return RedirectToAction("Index");
            }
            return View(branch);

        }
        public IActionResult ChangeState(int id)
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
