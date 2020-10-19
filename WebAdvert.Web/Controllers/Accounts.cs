using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;
using Amazon.AspNetCore.Identity.Cognito;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private SignInManager<CognitoUser> _signInManager;
        private UserManager<CognitoUser> _userManager;
        private CognitoUserPool _pool;

        public Accounts(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<ActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Signup(SignupModel signupModel)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(signupModel.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with the same Email does already exists");
                    return View(signupModel);
                }

                user.Attributes.Add(CognitoAttribute.Name.ToString(), signupModel.Email);
                var createdUser = await _userManager.CreateAsync(user, signupModel.Password);
                if (createdUser.Succeeded)
                {
                    return RedirectToAction("Confirm", "Accounts");
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Confirm(ConfirmModel confirmModel)
        {
            return View(confirmModel);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<ActionResult> ConfirmEmail(ConfirmModel confirmModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email was not found");
                    return View(confirmModel);
                }

                //var result = await _userManager.ConfirmEmailAsync(user, confirmModel.Code);
                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, confirmModel.Code, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }

                    return View(confirmModel);
                }
            }

            return View(confirmModel);
        }

        [HttpGet]
        public async Task<ActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<ActionResult> LoginPost(LoginModel model)
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
                    ModelState.AddModelError("LoginError", "Email and password does not match");
                }
            }
            return View("Login", model);
        }
    }
}
