using Final_Project.Models;
using Final_Project.Needs;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.DiscountTypeRepo;
using Final_Project.Repository.GovernorateRepo;
using Final_Project.Repository.RepresintativeRepo;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class RepresentativeController : Controller
    {
        private readonly IRepresintativeRepository _represintativeRepository;
        private readonly IGovernorateRepository _governorateRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IDiscountTypeRepository _discountTypeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RepresentativeController(IRepresintativeRepository represintativeRepository, IGovernorateRepository governorateRepository, IBranchRepository branchRepository, IDiscountTypeRepository discountTypeRepository, UserManager<ApplicationUser> userManager)
        {
            _represintativeRepository = represintativeRepository;
            _governorateRepository = governorateRepository;
            _branchRepository = branchRepository;
            _discountTypeRepository = discountTypeRepository;
            _userManager = userManager;
        }
        public IActionResult Index(int pg=1)
        {
            List<Representative> representatives = _represintativeRepository.GetAll();
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;
            int recsCount = representatives.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = representatives.Skip(recSkip).Take(pager.PageSize).ToList();
            pager.Controller = "Representative";
            pager.Action = "Index";
            this.ViewBag.pager = pager;
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            RepresentativeGovBranchPercentageViewModel representativeVM = new RepresentativeGovBranchPercentageViewModel()
            {
                Governorates = _governorateRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                DiscountTypes = _discountTypeRepository.GetAll(),
            };
            return View(representativeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RepresentativeGovBranchPercentageViewModel representativeVM)
        {
            representativeVM.Governorates = _governorateRepository.GetAll();
            representativeVM.Branchs = _branchRepository.GetAll();
            representativeVM.DiscountTypes = _discountTypeRepository.GetAll();
            if (!ModelState.IsValid)
            {
                return View(representativeVM);
            }
            if(await _userManager.FindByEmailAsync(representativeVM.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already exist");
                return View(representativeVM);
            }
            var user = new ApplicationUser
            {               
                Email = representativeVM.Email,
                UserName = representativeVM.Email,
                Name = representativeVM.Name,
                PhoneNumber = representativeVM.Phone,
                Address = representativeVM.Address,
                BranchId = representativeVM.BranchId
            };
            var result = await _userManager.CreateAsync(user, representativeVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(representativeVM);
                }
            }
            await _userManager.AddToRoleAsync(user, Roles.Representative.ToString());
            await _userManager.SetPhoneNumberAsync(user, representativeVM.Phone);

            var representative = new Representative
            {
                AppUserId = user.Id,
                CompanyPercentageOfOrder = representativeVM.CompanyPercentageOfOrder,
                GovernorateId = representativeVM.GovernorateId,
                BranchId = representativeVM.BranchId,
                DiscountTypeId = representativeVM.DiscountTypeId,
                IsDeleted = representativeVM.IsDeleted,
            };
            _represintativeRepository.Add(representative);
            _represintativeRepository.Save();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var representative = _represintativeRepository.GetById(id);
            if (representative == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            var model = new RepresentativeGovBranchPercentageViewModel
            {
                AppUserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Address = user.Address,
                CompanyPercentageOfOrder = (decimal)representative.CompanyPercentageOfOrder,
                //Password = "Admin2024",
                BranchId = user.BranchId,
                GovernorateId = (int)representative.GovernorateId,
                DiscountTypeId = (int)representative.DiscountTypeId,
                IsDeleted = representative.IsDeleted,
                Governorates = _governorateRepository.GetAll(),
                Branchs = _branchRepository.GetAll(),
                DiscountTypes = _discountTypeRepository.GetAll(),
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RepresentativeGovBranchPercentageViewModel model)
        {
            //model.Password = "Admin2024";
            var user = await _userManager.FindByIdAsync(model.AppUserId);
            if (user == null)
            {
                return NotFound();
            }
            model.Governorates = _governorateRepository.GetAll();
            model.Branchs = _branchRepository.GetAll();
            model.DiscountTypes = _discountTypeRepository.GetAll();
            if (!ModelState.IsValid) { return View(model); }
            var checkUser = await _userManager.FindByEmailAsync(model.Email);
            if (checkUser != null && checkUser.Id != model.AppUserId)
            {
                ModelState.AddModelError("Email", "Email is already exist");
                return View(model);
            }
            user.Name = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Address = model.Address;
            user.BranchId = (int)model.BranchId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("BranchId", error.Description);
                    return View(model);
                }
            }
            var representativeFDB = _represintativeRepository.GetById(model.AppUserId);

            representativeFDB.GovernorateId = model.GovernorateId;
            representativeFDB.BranchId = model.BranchId;
            representativeFDB.DiscountTypeId = model.DiscountTypeId;
            representativeFDB.CompanyPercentageOfOrder = model.CompanyPercentageOfOrder;
            _represintativeRepository.Save();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> changeState(string id)
        {
            if (id == null)
                return BadRequest();
            var representative = _represintativeRepository.GetById(id);
            if (representative == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            user.IsDeleted = !representative.IsDeleted;
            representative.IsDeleted = !representative.IsDeleted;
            _represintativeRepository.Save();
            return Ok();
        }
    }
}
