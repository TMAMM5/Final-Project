using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        [Authorize(Permissions.Roles.View)]

        public IActionResult Index(string childname, string word, int pg = 1)
        {
            if (String.IsNullOrEmpty(childname))
            {
                List<IdentityRole> roles;
                if (string.IsNullOrEmpty(word))
                {
                    roles = _roleManager.Roles.Where(r => r.Name != "Representative" && r.Name != "Trader").ToList();
                }
                else
                {
                    roles = _roleManager.Roles
                       .Where(e => e.Name.ToLower().Contains(word.ToLower())).Where(r => r.Name != "Representative" && r.Name != "Trader").ToList();
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
            else {
                var searchItems = _roleManager.Roles.Where(s => s.Name.ToLower().Contains(childname.ToLower())).ToList();
                return View(searchItems);
            }
        }
        [Authorize(Permissions.Roles.Create)]

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

        [Authorize(Permissions.Roles.Edit)]

        public async Task<IActionResult> ManagePermissions(string roleId)
        {
            if (roleId == null)
                return BadRequest();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var roleClaims = _roleManager.GetClaimsAsync(role).Result.Select(c => c.Value).ToList();
            var allClaims = Permissions.GenerateAllPermissions();
            var allPermissions = allClaims.Select(p => new CheckBoxVM { Value = p }).ToList();

            foreach (var permission in allPermissions)
            {
                if (roleClaims.Any(c => c == permission.Value))
                    permission.IsSelected = true;
            }

            var viewModel = new PermissionVM
            {
                RoleId = roleId,
                RoleName = role.Name,
                MyRoles = allPermissions
            };

            return View(viewModel);
        }
        [Authorize(Permissions.Roles.Edit)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePermissions(PermissionVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
                return NotFound();

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in roleClaims)
                await _roleManager.RemoveClaimAsync(role, claim);

            var selectedClaims = model.MyRoles.Where(c => c.IsSelected).ToList();

            foreach (var claim in selectedClaims)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.Value));

            return RedirectToAction(nameof(Index));
        }



        [Authorize(Permissions.Roles.Edit)]

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        [Authorize(Permissions.Roles.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IdentityRole roleModel)
        {
            if (roleModel == null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Fetch the existing role from the database
                var roleInDb = await _roleManager.FindByIdAsync(roleModel.Id);
                if (roleInDb == null)
                {
                    return NotFound($"Unable to load role with ID '{roleModel.Id}'.");
                }

                // Update the properties of the existing role from the model
                roleInDb.Name = roleModel.Name;
                roleInDb.NormalizedName = roleModel.NormalizedName;
                roleInDb.ConcurrencyStamp = roleModel.ConcurrencyStamp; // Ensure this line is included to update the ConcurrencyStamp

                var result = await _roleManager.UpdateAsync(roleInDb);
                if (!result.Succeeded)
                {
                    // Add error details to ModelState or return an appropriate response
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(roleModel); // Return the view with errors
                }
                return RedirectToAction("Index");
            }
            return View(roleModel);
        }
        [Authorize(Permissions.Roles.Delete)]

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
        [Authorize(Permissions.Roles.Delete)]

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
