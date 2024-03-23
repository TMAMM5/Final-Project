﻿using Final_Project.Models;
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


        public IActionResult Index()
        {
            List<Branch> branchs = branchRepository.GetAll();
            return View(branchs);
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
