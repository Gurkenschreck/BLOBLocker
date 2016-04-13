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
using BLOBLocker.Code.ModelHelper;
using BLOBLocker.Code.Attributes;
using BLOBLocker.Entities.Models.WebApp;
using System.Text;
using BLOBLocker.Code.ViewModels.WebApp;
using BLOBLocker.Code.Extention;
using BLOBLocker.Code.ViewModels.Validation;

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
            // Check if info is correct
            if(ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account newAcc = accRepo.GetAccount(acc.Alias);

                if(newAcc == null)
                {
                    int basicMemoryPoolSize = HttpContext.Application["account.InitialMemoryPoolSize"].As<int>();
                    newAcc = accRepo.CreateNew(acc.Alias, acc.Password, acc.ContactEmail, basicMemoryPoolSize, new CryptoConfigRepository.Config
                    {
                        Password = acc.Password,
                        SaltByteLength = HttpContext.Application["security.SaltByteLength"].As<int>(),
                        SymKeySize = HttpContext.Application["security.AccountKeySize"].As<int>(),
                        RSAKeySize = HttpContext.Application["security.AccountRSAKeySize"].As<int>(),
                        HashIterations = HttpContext.Application["security.HashIterationCount"].As<int>()
                    });
                    bool createPersistentCookie = HttpContext.Application["security.CreatePersistentAuthCookie"].As<bool>();
                    int cookieKeySize = HttpContext.Application["security.CookieCryptoKeySize"].As<int>();

                    string basicRoleName = HttpContext.Application["account.DefaultRole"].As<string>();
                    accRepo.AddRole(newAcc, basicRoleName);

                    NotificationHelper.SendNotification(newAcc,
                        Resources.Notifications.WelcomeMessage,
                        newAcc.Alias);

                    context.Accounts.Add(newAcc);
                    context.Additions.Add(newAcc.Addition);
                    context.SaveChanges();

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
                ModelState.AddModelError("AliasAlreadyExists", Resources.Account.Strings.AliasAlreadyExists);
            }
            ViewBag.IsRegistrationRestricted = HttpContext.Application["system.RestrictRegistration"].As<bool>();
            return View(acc);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [RequiredParameters("acc")]
        [ValidateAntiForgeryToken, AllowAnonymous, HttpPost]
        public ActionResult Login(AccountViewModel acc)
        {
            bool loginEnabled = HttpContext.Application["system.EnableLogin"].As<bool>();
            if (!loginEnabled)
            {
                ModelState.AddModelError("LoginClosed", Resources.Account.Strings.LoginDisabled);
            }
            if(ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account correspondingAcc = accRepo.GetAccount(acc.Alias);
                // validate password
                if (correspondingAcc != null)
                {
                    if (correspondingAcc.IsEnabled)
                    {
                        bool createPersistentAuthCookie = HttpContext.Application["security.CreatePersistentAuthCookie"].As<bool>();
                        int cookieKeySize = HttpContext.Application["security.CookieCryptoKeySize"].As<int>(); ;

                        using (var symC = new SymmetricCipher<AesManaged>(acc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV, iterations:correspondingAcc.Config.IterationCount))
                        {
                            try
                            {
                                correspondingAcc.Addition.LastLogin = DateTime.Now;
                                context.SaveChanges();

                                byte[] plainPrivKey = symC.Decrypt(correspondingAcc.Config.PrivateKey);
                                string pPriKey = symC.DecryptToString(correspondingAcc.Config.PrivateKey);
                                HttpCookie cryptoCookie = null;


                                byte[] sessionCookieKey;
                                byte[] sessionCookieIV;
                                byte[] sessionStoredKeyPart;
                                using(var credHandler = new CredentialHandler(cookieKeySize))
                                {
                                    credHandler.Inject(symC.Decrypt(correspondingAcc.Config.PrivateKey), out cryptoCookie, out sessionCookieKey, out sessionCookieIV, out sessionStoredKeyPart);
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
                            }
                            catch (CryptographicException)
                            {
                                correspondingAcc.Addition.LastFailedLogin = DateTime.Now;
                                NotificationHelper.SendNotification(correspondingAcc, 
                                    Resources.Notifications.FailedLogin,
                                    DateTime.Now);
                                context.SaveChanges();
                                // Log wrong password try?
                                ModelState.AddModelError("WrongAliasOrPassword", Resources.Account.Strings.WrongAliasOrPassword);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("AccountIsDisabledError", Resources.Account.Strings.AccountIsDisabledError);
                    }
                }
                else
                {
                    ModelState.AddModelError("WrongAliasOrPassword", Resources.Account.Strings.WrongAliasOrPassword);
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
            var acc = AccRepo.GetAccount(User.Identity.Name);
            return View(acc);
        }
    }
}
