using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using QualityShopping.Business.Abstract;
using QualityShopping.WebUI.Extensions;
using QualityShopping.WebUI.Identity;
using QualityShopping.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityShopping.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        }
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // generate token
                var _token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = _token

                });
                 
                await _emailSender.SendEmailAsync(model.Email, "Confirm our account!", $"Please <a href='http://localhost:58423{callbackUrl}'>click</a> link to confirm your account.");
             
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Account Confirmation!",
                    Message = "We have sent an email with a confirmation link to your email address. In order to complete the sign-up process, please click the confirmation link.",
                    Css = "warning"
                }); 
                return RedirectToAction("Login", "Account");
            }
             
            ModelState.AddModelError("", "Unknown error occurred, please try again.");
            return View(model);
        }


        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "This account has not been created.");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Not confirmed user!");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }

            ModelState.AddModelError("", "Incorrect Email or password!");
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new ResultMessage()
            {
                Title = "Signed out.",
                Message = "Your account signed out!",
                Css = "warning"
            });

            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {

                TempData.Put("message", new ResultMessage()
                {
                    Title = "Account Confirmation!",
                    Message = "Wrong information to confirm your account.",
                    Css = "danger"
                });
 
                return Redirect("~/");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData.Put("message", new ResultMessage()
                    {
                        Title = "Account Confirmation!",
                        Message = "Your account has been confirmed successfully.",
                        Css = "success"
                    }); 

                    _cartService.InitializeCart(user.Id);

                    return RedirectToAction("Login");
                }
            }
            TempData.Put("message", new ResultMessage()
            {
                Title = "Account Confirmation!",
                Message = "Your registration has not been confirmed.",
                Css = "danger"
            });
            return RedirectToAction("Login");
        }
        public IActionResult Accessdenied()
        {

            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Forgot Password",
                    Message = "Wrong information.",
                    Css = "danger"
                });
 
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Forgot Password",
                    Message = "Email address not found.",
                    Css = "danger"
                });
                return View();
            }
            var _token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = _token

            });


            await _emailSender.SendEmailAsync(Email, "Reset password", $"please <a href='http://localhost:58423{callbackUrl}'>click</a> link to reset your password");

            TempData.Put("message", new ResultMessage()
            {
                Title = "Forgot Password",
                Message = "Your password reset email has been sent.",
                Css = "warning"
            });
             
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                return RedirectToAction("Home", "Index");
            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

    }
}