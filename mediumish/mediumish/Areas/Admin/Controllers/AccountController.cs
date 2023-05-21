using mediumish.Helpers;
using mediumish.Models;
using mediumish.ViewsModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace mediumish.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _usermanager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _rolemanager;

        public AccountController(UserManager<AppUser> userManager,
								SignInManager<AppUser> signInManager,
								RoleManager<IdentityRole> roleManager)
        {
			_rolemanager = roleManager;
			_usermanager = userManager;
			_signInManager = signInManager;
        }

        #region Login
        public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> Login(LoginVM loginVM)
		{
			AppUser user = await _usermanager.FindByNameAsync(loginVM.UserName);
			if (user == null)
			{
				ModelState.AddModelError("", "Username or Passsword is wrong");
				return View();
			}
			if (user.IsDeactive)
			{
				ModelState.AddModelError("", "Your account Deactive from Admin");
				return View();
			}
			Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync
																	(user, loginVM.Password, loginVM.IsRemember, true);
			if(signInResult.IsLockedOut)
			{
                ModelState.AddModelError("", "Your Account is blocked");
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is wrong");
                return View();
            }
            return RedirectToAction("Index", "Dashboard");
		}
		#endregion

		#region Register
		//public IActionResult Register()
		//{
		//	return View();
		//}

		//[HttpPost]
		//[ValidateAntiForgeryToken]

		//public async Task<IActionResult> Register(RegisterVM registerVM)
		//{
		//	AppUser newuser = new AppUser
		//	{
		//		Email = registerVM.Email,
		//		UserName = registerVM.Username,
		//		FullName=registerVM.FullName,	
		//	};
		//	IdentityResult identityResult =await _usermanager.CreateAsync(newuser, registerVM.Password);
		//	if (!identityResult.Succeeded)
		//	{
		//		foreach (IdentityError error in identityResult.Errors)
		//		{
		//			ModelState.AddModelError("", error.Description);
		//		}
		//		return View();
		//	}

		//	await _usermanager.AddToRoleAsync(newuser, Roles.Admin.ToString());
		//	await _signInManager.SignInAsync(newuser, registerVM.IsRemember);
		//	return RedirectToAction("Index", "Dashboard");
		//}
		#endregion

		#region Signout
		public async Task<IActionResult> Signout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home", new { area = "default" });
		}
		#endregion

		#region Role
		//public async Task CreateRole()
		//{
		//	if(!await _rolemanager.RoleExistsAsync(Helpers.Roles.Admin.ToString()))
		//	{
		//		await _rolemanager.CreateAsync(new IdentityRole { Name = Helpers.Roles.Admin.ToString() });
		//	}
		//	if(!await _rolemanager.RoleExistsAsync(Helpers.Roles.User.ToString()))
		//	{
		//		await _rolemanager.CreateAsync(new IdentityRole { Name=Helpers.Roles.User.ToString() });
		//	}
		//}
		#endregion
	}
}
