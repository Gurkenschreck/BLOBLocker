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
            ViewBag.IsRegistrationRestricted = bool.Parse(HttpContext.Application["system.RestrictRegistration"] as string);
            return View();
        }

        [RequiredParameters("acc")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SignUp(AccountViewModel acc)
        {
            bool enableRegistration = bool.Parse(HttpContext.Application["system.EnableRegistration"] as string);
            if(!enableRegistration)
            {
                ModelState.AddModelError("RegistrationDisabled", Resources.Account.Strings.RegistrationDisabled);
            }
            bool isRegistrationRestricted = bool.Parse(HttpContext.Application["system.RestrictRegistration"] as string);
            if (isRegistrationRestricted)
            {
                if(string.IsNullOrEmpty(acc.RegistrationCode))
                {
                    ModelState.AddModelError("InvalidRegistrationCode", Resources.Account.Strings.InvalidRegistrationCode);

                }
                string expectedRegistrationCode = HttpContext.Application["system.RestrictedRegistrationCode"] as string;
                if (acc.RegistrationCode != expectedRegistrationCode)
                {
                    ModelState.AddModelError("RegistrationRestricted", Resources.Account.Strings.RestrictedRegistrationMessage);
                }
            }
            int accLimit = Convert.ToInt32(HttpContext.Application["system.AccountLimit"]);
            int actual = context.Accounts.Count();
            if(actual >= accLimit)
            {
                ModelState.AddModelError("AccountLimitReached", Resources.Account.Strings.AccountLimitReached);
            }
            // Check if info is correct
            if(ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account newAcc = accRepo.GetAccount(acc.Alias);

                if(newAcc == null)
                {
                    int basicMemoryPoolSize = Convert.ToInt32(HttpContext.Application["account.InitialMemoryPoolSize"].ToString());
                    newAcc = accRepo.CreateNew(acc.Alias, acc.Password, acc.ContactEmail, basicMemoryPoolSize, new CryptoConfigRepository.Config
                    {
                        Password = acc.Password,
                        SaltByteLength = Convert.ToInt32(HttpContext.Application["security.SaltByteLength"]),
                        SymKeySize = Convert.ToInt32(HttpContext.Application["security.AccountKeySize"]),
                        RSAKeySize = Convert.ToInt32(HttpContext.Application["security.AccountRSAKeySize"]),
                        HashIterations = Convert.ToInt32(HttpContext.Application["security.HashIterationCount"])
                    });
                    bool createPersistentCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());
                    int cookieKeySize = Convert.ToInt32(HttpContext.Application["security.CookieCryptoKeySize"]);

                    string basicRoleName = HttpContext.Application["account.DefaultRole"] as string;
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

                        using(var credHandler = new CredentialHandler(cookieKeySize, Session))
                        {
                            credHandler.Inject(symC.Decrypt(newAcc.Config.PrivateKey), Session, out keyPartCookie);
                            Response.Cookies.Add(keyPartCookie);
                        }
                    }
                    FormsAuthentication.SetAuthCookie(newAcc.Alias, createPersistentCookie);
                    return RedirectToAction("Index", "Panel");
                }
                ModelState.AddModelError("AliasAlreadyExists", Resources.Account.Strings.AliasAlreadyExists);
            }
            ViewBag.IsRegistrationRestricted = bool.Parse(HttpContext.Application["system.RestrictRegistration"] as string);
            
            return View(acc);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [RequiredParameters("acc")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(AccountViewModel acc)
        {
            bool loginEnabled = bool.Parse(HttpContext.Application["system.EnableLogin"] as string);
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
                        bool createPersistentAuthCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());
                        int cookieKeySize = Convert.ToInt32(HttpContext.Application["security.CookieCryptoKeySize"]);

                        using (var symC = new SymmetricCipher<AesManaged>(acc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV, iterations:correspondingAcc.Config.IterationCount))
                        {
                            try
                            {
                                correspondingAcc.Addition.LastLogin = DateTime.Now;
                                context.SaveChanges();

                                byte[] plainPrivKey = symC.Decrypt(correspondingAcc.Config.PrivateKey);
                                string pPriKey = symC.DecryptToString(correspondingAcc.Config.PrivateKey);
                                HttpCookie cryptoCookie = null;
                                
                                using(var credHandler = new CredentialHandler(cookieKeySize, Session))
                                {
                                    HttpCookie keypartCookie;
                                    credHandler.Inject(plainPrivKey, Session, out keypartCookie);
                                    cryptoCookie = keypartCookie;
                                    Response.Cookies.Add(keypartCookie);
                                }
                                
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
