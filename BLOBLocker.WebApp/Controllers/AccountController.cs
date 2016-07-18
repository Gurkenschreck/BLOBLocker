
using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.WebApp.Controllers;
using BLOBLocker.Entities.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.Membership;
using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.WebApp;
using System.Text;
using BLOBLocker.Code.ViewModels.WebApp;
using BLOBLocker.Code.Extention;
using BLOBLocker.Code.ViewModels.Validation;
using BLOBLocker.Code.Data;
using BLOBLocker.Code.Web;
using BLOBLocker.Code.Security.Cryptography;
using BLOBLocker.Code.Media.Bitmaps;

namespace BLOBLocker.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/
        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignUp()
        {
            AccountViewModel accvm = new AccountViewModel();

            CaptchaFactory captchaFactory = new CaptchaFactory();
            accvm.Captcha = captchaFactory.CreateNew(200, 70);
            Session["latestCaptcha"] = accvm.Captcha.Value;

            ViewBag.IsRegistrationRestricted = HttpContext.Application["system.RestrictRegistration"].As<bool>();
            return View(accvm);
        }

        [RequiredParameters("acc")]
        [CustomValidate(typeof(AccountViewModelValidation))] // Validation for: Registration enabled, Registration Restricted, Account limit reached
        [ValidateAntiForgeryToken, AllowAnonymous, HttpPost]
        public ActionResult SignUp(AccountViewModel acc)
        {
            bool isRegRestricted = HttpContext.Application["system.RestrictRegistration"].As<bool>();
            ViewBag.IsRegistrationRestricted = isRegRestricted;

            // Check if info is correct
            if (ModelState.IsValid)
            {
                string latestCaptcha = Session["latestCaptcha"].ToString();
                if (!acc.UserCaptchaInput.Equals(latestCaptcha))
                {
                    ModelState.AddModelError("UserCaptchaInput", HttpContext.GetGlobalResourceObject(null, "Account.CaptchaInputDoesNotMatch").As<string>());
                }
                else
                {
                    AccountHandler accHandler = new AccountHandler();
                    AccountRepository accRepo = new AccountRepository(context);
                    Account newAcc = accRepo.GetByKey(acc.Alias);

                    if (newAcc == null)
                    {
                        int basicMemoryPoolSize = HttpContext.Application["account.InitialMemoryPoolSize"].As<int>();
                        int saltByteLength = HttpContext.Application["security.SaltByteLength"].As<int>();
                        int accountSymKeySize = HttpContext.Application["security.AccountKeySize"].As<int>();
                        int accountRSAKeySizeRSAKeySize = HttpContext.Application["security.AccountRSAKeySize"].As<int>();
                        int hashIterations = HttpContext.Application["security.HashIterationCount"].As<int>();
                        string basicRoleName = HttpContext.Application["account.DefaultRole"].As<string>();

                        AccountRole defaultAccRole = context.AccountRoles.First(p => p.Definition == basicRoleName);

                        var accProperties = new AccountHandler.AccountProperties
                        {
                            Alias = acc.Alias,
                            Password = acc.Password,
                            Roles = new List<AccountRole>()
                        };
                        accProperties.Roles.Add(defaultAccRole);

                        var cryptoProperties = new CryptoConfigHandler.CryptoConfigProperties
                        {
                            SaltByteLength = saltByteLength,
                            SymmetricKeySize = accountSymKeySize,
                            RSAKeySize = accountRSAKeySizeRSAKeySize,
                            HashIterations = hashIterations
                        };

                        var additionProperties = new AccountAdditionHandler.AccountAdditionProperties
                        {
                            Email = acc.ContactEmail
                        };

                        var memPoolProperties = new MemoryPoolHandler.MemoryPoolProperties
                        {
                            BasicMemory = basicMemoryPoolSize,
                            AdditionalMemory = 0
                        };

                        newAcc = accHandler.SetupAccount(accProperties,
                            cryptoProperties,
                            additionProperties,
                            memPoolProperties);

                        bool createPersistentCookie = HttpContext.Application["security.CreatePersistentAuthCookie"].As<bool>();
                        int cookieKeySize = HttpContext.Application["security.CookieCryptoKeySize"].As<int>();

                        BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(newAcc,
                            HttpContext.GetGlobalResourceObject(null, "Notification.WelcomeMessage").As<string>(),
                            newAcc.Alias);

                        accRepo.Add(newAcc);

                        using (var symC = new SymmetricCipher<AesManaged>(acc.Password, newAcc.Salt, newAcc.Config.IV,
                            accountSymKeySize, hashIterations))
                        {
                            using (var css = new CryptoSessionStore("AccPriKey",
                                Session, Request, Response))
                            {
                                css["PrivRSAKey"] = symC.Decrypt(newAcc.Config.PrivateKey);
                            }
                        }
                        FormsAuthentication.SetAuthCookie(newAcc.Alias, createPersistentCookie);
                        return RedirectToAction("Index", "Panel");
                    }
                    ModelState.AddModelError("Alias", HttpContext.GetGlobalResourceObject(null, "Account.AccountAlreadyExists").As<string>());
                }
            }

            CaptchaFactory captchaFactory = new CaptchaFactory();
            acc.Captcha = captchaFactory.CreateNew(200, 70);
            acc.UserCaptchaInput = string.Empty;
            Session["latestCaptcha"] = acc.Captcha.Value;

            return View(acc);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [RequiredParameters("acc")]
        [ValidateAntiForgeryToken, AllowAnonymous, HttpPost]
        public ActionResult SignIn(LoginViewModel acc, string ReturnUrl)
        {
            bool loginEnabled = HttpContext.Application["system.EnableLogin"].As<bool>();
            if (!loginEnabled)
            {
                ModelState.AddModelError("Alias", HttpContext.GetGlobalResourceObject(null, "Account.LoginDisabled").As<string>());
            }
            if(ModelState.IsValid)
            {
                AccountHandler accHandler = new AccountHandler();
                AccountRepository accRepo = new AccountRepository(context);
                Account correspondingAcc = accRepo.GetByKey(acc.Alias);
                
                if (correspondingAcc != null)
                {
                    byte[] priRSAKey;
                    switch (accHandler.Login(correspondingAcc, acc.Password, out priRSAKey))
                    {
                        case 1:
                            bool createPersistentAuthCookie = HttpContext.Application["security.CreatePersistentAuthCookie"].As<bool>();
                            int cookieKeySize = HttpContext.Application["security.CookieCryptoKeySize"].As<int>(); ;

                            using(var css = new CryptoSessionStore("AccPriKey",
                                Session, Request, Response))
                            {
                                css["PrivRSAKey"] = priRSAKey;
                            }
                            
                            FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, createPersistentAuthCookie);

                            if (string.IsNullOrWhiteSpace(ReturnUrl))
                                return RedirectToAction("Index", "Panel");
                            else
                                Response.Redirect(ReturnUrl);
                            break;
                        case 2:
                            ModelState.AddModelError("Alias", HttpContext.GetGlobalResourceObject(null, "Account.WrongLoginOrPassword").As<string>());
                            break;
                        case 3:
                            ModelState.AddModelError("Alias", HttpContext.GetGlobalResourceObject(null, "Account.Disabled").As<string>());
                            break;
                        default:

                            break;
                    }
                    context.SaveChanges();
                    
                }
                else
                {
                    ModelState.AddModelError("Alias", HttpContext.GetGlobalResourceObject(null, "Account.WrongLoginOrPassword").As<string>());
                }
            }
            return View(acc);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignOut(string returnUrl)
        {
            foreach (var cookieName in Request.Cookies.AllKeys)
            {
                Response.Cookies[cookieName].Value = "";
                Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
            }
                
            var customCulture = Session["customCulture"] as CultureInfo;
            
            Session.RemoveAll();
            Session.Abandon();
            if(customCulture != null)
                Session["customCulture"] = customCulture;
            return RedirectToAction("SignIn", new { returnUrl = returnUrl });
        }

        [HttpGet]
        public ActionResult Overview()
        {
            var AccRepo = new AccountRepository(context);
            var acc = AccRepo.GetByKey(User.Identity.Name);
            return View(acc);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [RequiredParameters("cpvm")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel cpvm)
        {
            if (ModelState.IsValid)
            {
                int saltByteLength = HttpContext.Application["security.SaltByteLength"].As<int>();
                int accountSymKeySize = HttpContext.Application["security.AccountKeySize"].As<int>();
                int hashIterations = HttpContext.Application["security.HashIterationCount"].As<int>();

                var cryptoProperties = new CryptoConfigHandler.CryptoConfigProperties
                {
                    SaltByteLength = saltByteLength,
                    SymmetricKeySize = accountSymKeySize,
                    HashIterations = hashIterations
                };

                var accRepo = new AccountRepository(context);
                var curAcc = accRepo.GetByKey(User.Identity.Name);
                var accHandler = new AccountHandler(curAcc);
                if (accHandler.ChangePassword(cpvm.CurrentPassword, cpvm.NewPassword, cryptoProperties))
                {
                    context.SaveChanges();
                    return RedirectToAction("PasswordChanged");
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", HttpContext.GetGlobalResourceObject(null, "Account.CurrentPasswordWrong").As<string>());
                } 
            }
            return View(cpvm);
        }

        [HttpGet]
        public ActionResult PasswordChanged()
        {
            return View();
        }
    }
}
