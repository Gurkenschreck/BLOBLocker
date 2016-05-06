
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
using BLOBLocker.Entities.Models.Models.WebApp;

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
            ViewBag.IsRegistrationRestricted = HttpContext.Application["system.RestrictRegistration"].As<bool>();
            return View();
        }

        [RequiredParameters("acc")]
        [CustomValidate(typeof(AccountViewModelValidation))]
        [ValidateAntiForgeryToken, AllowAnonymous, HttpPost]
        public ActionResult SignUp(AccountViewModel acc)
        {
            bool isRegRestricted = HttpContext.Application["system.RestrictRegistration"].As<bool>();
            ViewBag.IsRegistrationRestricted = isRegRestricted;
            if (isRegRestricted)
                ModelState.AddModelError(null, HttpContext.GetGlobalResourceObject(null, "Account.RegistrationRestricted").As<string>());
            // Check if info is correct
            if(ModelState.IsValid)
            {
                AccountHandler accHandler = new AccountHandler();
                AccountRepository accRepo = new AccountRepository(context);
                Account newAcc = accRepo.GetByKey(acc.Alias);

                if(newAcc == null)
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

                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, newAcc.Salt, newAcc.Config.IV))
                    {
                        HttpCookie keyPartCookie = null;

                        using(var credHandler = new CredentialHandler(cookieKeySize))
                        {
                            byte[] sessionCookieKey;
                            byte[] sessionCookieIV;
                            byte[] sessionStoredKeyPart;
                            credHandler.Inject(symC.Decrypt(newAcc.Config.PrivateKey),
                                out keyPartCookie,
                                out sessionCookieKey,
                                out sessionCookieIV,
                                out sessionStoredKeyPart);
                            Response.Cookies.Add(keyPartCookie);
                            Session["AccPriKeyCookieKey"] = sessionCookieKey;
                            Session["AccPriKeyCookieIV"] = sessionCookieIV;
                            Session["AccPriKeySessionStoredKeyPart"] = sessionStoredKeyPart;
                        }
                    }
                    FormsAuthentication.SetAuthCookie(newAcc.Alias, createPersistentCookie);
                    return RedirectToAction("Index", "Panel");
                }
                ModelState.AddModelError("AliasAlreadyExists", HttpContext.GetGlobalResourceObject(null, "Account.AccountAlreadyExists").As<string>());
            }
            
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
        public ActionResult SignIn(AccountViewModel acc)
        {
            bool loginEnabled = HttpContext.Application["system.EnableLogin"].As<bool>();
            if (!loginEnabled)
            {
                ModelState.AddModelError("LoginClosed", HttpContext.GetGlobalResourceObject(null, "Account.LoginDisabled").As<string>());
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

                            HttpCookie cryptoCookie = null;

                            byte[] sessionCookieKey;
                            byte[] sessionCookieIV;
                            byte[] sessionStoredKeyPart;
                            using(var credHandler = new CredentialHandler(cookieKeySize))
                            {
                                credHandler.Inject(priRSAKey, out cryptoCookie, out sessionCookieKey, out sessionCookieIV, out sessionStoredKeyPart);
                                Response.Cookies.Add(cryptoCookie);
                            }
                            Session["AccPriKeyCookieKey"] = sessionCookieKey;
                            Session["AccPriKeyCookieIV"] = sessionCookieIV;
                            Session["AccPriKeySessionStoredKeyPart"] = sessionStoredKeyPart;

                            FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, createPersistentAuthCookie);
                            if (Request.QueryString["ReturnUrl"] == null)
                                return RedirectToAction("Index", "Panel");
                            else
                                Response.Redirect(Request.QueryString["ReturnUrl"]);
                            break;
                        case 2:
                            ModelState.AddModelError("WrongAliasOrPassword", HttpContext.GetGlobalResourceObject(null, "Account.WrongLoginOrPassword").As<string>());
                            break;
                        case 3:
                            ModelState.AddModelError("AccountIsDisabledError", HttpContext.GetGlobalResourceObject(null, "Account.Disabled").As<string>());
                            break;
                        default:

                            break;
                    }
                    context.SaveChanges();
                    
                }
                else
                {
                    ModelState.AddModelError("WrongAliasOrPassword", HttpContext.GetGlobalResourceObject(null, "Account.WrongLoginOrPassword").As<string>());
                }
            }
            return View(acc);
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Secret"].Expires = DateTime.Now.AddDays(-1);

            var customCulture = Session["customCulture"] as CultureInfo;

            Session.Abandon();
            if(customCulture != null)
                Session["customCulture"] = customCulture;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Overview()
        {
            var AccRepo = new AccountRepository(context);
            var acc = AccRepo.GetByKey(User.Identity.Name);
            return View(acc);
        }
    }
}
