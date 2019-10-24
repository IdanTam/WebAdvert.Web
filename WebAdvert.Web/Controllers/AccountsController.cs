using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using Amazon.AspNetCore.Identity.Cognito;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {

        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {

            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExist", "User with this email already exist");
                    return View(model);
                }

                user.Attributes.Add("name", model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password);
                //await _userManager.CreateAsync(user)//option to set temporary passwod
                if (createdUser.Succeeded)
                {
                    //RedirectToPage("./ConfirmPassword");
                    return RedirectToAction("Confirm");                    
                }

                if (createdUser.Errors != null)
                {
                    foreach (var item in createdUser.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }

            }
            return View();
        }

        public async Task<IActionResult>  Confirm()
        {
            
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Not Found", "A user not found");
                    return View(model);
                }
                               

                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.Errors != null)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }                
            }

            return View(model);
        }


        public async Task<IActionResult> Login()
        {           
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    
                      ModelState.AddModelError("Login Error", "Email and Password do not match");
                    
                }
            }

            return View(model);
        }
    }
}