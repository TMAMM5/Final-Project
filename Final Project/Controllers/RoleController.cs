using Final_Project.Models;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoleController(RoleManager<IdentityRole> roleManager , UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            
        }
        public IActionResult Index(string word , int pg=1)
        {
            List<IdentityRole> roles;    
            if (string.IsNullOrEmpty(word))
            {
                roles = _roleManager.Roles.ToList();
            }
            else
            {
                roles = _roleManager.Roles
                   .Where(e => e.Name.ToLower().Contains(word.ToLower())).ToList();
            }
            var superAdminRole = roles.SingleOrDefault(r => r.Name == "SuperAdmin");
            if (superAdminRole != null)
            {
                roles.Remove(superAdminRole);
                roles.Insert(0, superAdminRole);
            }
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = roles.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = roles.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Role";
            pager.Action = "Index";
            this.ViewBag.pager = pager;
            return View(data);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormVM model)
        {
            var roles = _roleManager.Roles.ToList();
            var superAdminRole = roles.SingleOrDefault(r => r.Name == "SuperAdmin");
            if (superAdminRole != null)
            {
                roles.Remove(superAdminRole);
                roles.Insert(0, superAdminRole);
            }
            if (!ModelState.IsValid)
                return View("Index", roles);


            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Role is exists!");
                return View("Index", roles);

            }
            await _roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IdentityRole role)
        {
            if (role == null)
                return BadRequest();

            if (ModelState.IsValid)
            {

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    return Content("Error On Update");
                }
                return RedirectToAction("Index");
            }
            return View(role);
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
             await _roleManager.DeleteAsync(role);

            }
            return RedirectToAction("Index");
        }

    }
}
