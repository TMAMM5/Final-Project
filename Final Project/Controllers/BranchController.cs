using Final_Project.Models;
using Final_Project.Needs;
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

        [Authorize(Permissions.Branches.View)]

        public IActionResult Index(string childname, int pg = 1)
        {
            if (String.IsNullOrEmpty(childname))
            {
                List<Branch> branchs = branchRepository.GetAll();
                const int pageSize = 9;
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
            else
            {
                var searchItems = branchRepository.GetAll().Where(s => s.Name.ToLower().Contains(childname.ToLower())).ToList();
                return View(searchItems);
            }
        }
        [Authorize(Permissions.Branches.Create)]

        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Permissions.Branches.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Authorize(Permissions.Branches.Edit)]

        public IActionResult Edit(int id)
        {
            Branch branch = branchRepository.GetById(id);
            return View(branch);
        }
        [Authorize(Permissions.Branches.Edit)]

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [Authorize(Permissions.Branches.Delete)]
        
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
