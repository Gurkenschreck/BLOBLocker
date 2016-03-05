using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using SpreadBase.App_Code.ModelFactory;
using SpreadBase.Controllers;
using SpreadBase.Models;
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

namespace SpreadBase.Controllers
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

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SignUp(Account acc, string registrationCode)
        {
            if(acc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(acc.Alias))
            {
                ModelState.AddModelError("InvalidAlias", Resources.Account.Strings.InavlidAlias);
            }
            if (string.IsNullOrWhiteSpace(acc.Password))
            {
                ModelState.AddModelError("InvalidPassword", Resources.Account.Strings.InvalidPassword);
            }
            bool enableRegistration = bool.Parse(HttpContext.Application["system.EnableRegistration"] as string);
            if(!enableRegistration)
            {
                ModelState.AddModelError("RegistrationDisabled", Resources.Account.Strings.RegistrationDisabled);
            }
            bool isRegistrationRestricted = bool.Parse(HttpContext.Application["system.RestrictRegistration"] as string);
            if (isRegistrationRestricted)
            {
                string expectedRegistrationCode = HttpContext.Application["system.RestrictedRegistrationCode"] as string;
                if (registrationCode != expectedRegistrationCode)
                {
                    ModelState.AddModelError("RegistrationDisabled", Resources.Account.Strings.RegistrationDisabled);
                }
            }
            // Check if info is correct
            if(ModelState.IsValid)
            {
                Account newAcc = context.Accounts.FirstOrDefault(x => x.Alias == acc.Alias);

                if(newAcc == null)
                {
                    AccountFactory fact = new AccountFactory();
                    newAcc = fact.CreateNewAccount(this.HttpContext, acc);
                    bool createPersistentCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());
                    int cookieKeySize = Convert.ToInt32(HttpContext.Application["security.CookieCryptoKeySize"]);

                    string basicRoleName = HttpContext.Application["account.DefaultRole"] as string;
                    newAcc.Roles.Add(new AccountRoleLink
                    {
                        Account = newAcc,
                        Role = context.AccountRoles.FirstOrDefault(x => x.Definition == basicRoleName)
                    });

                    var welcomeNotification = new Notification();
                    string notificationMessage = Resources.Notifications.WelcomeMessage;
                    welcomeNotification.Description = string.Format(notificationMessage, newAcc.Alias);
                    newAcc.Addition.Notifications.Add(welcomeNotification);

                    context.Accounts.Add(newAcc);
                    context.Additions.Add(newAcc.Addition);
                    context.SaveChanges();

                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, newAcc.Salt, newAcc.Config.IV))
                    {
                        using (var cookieCipher = new SymmetricCipher<AesManaged>(cookieKeySize))
                        {
                            Session["CookieKey"] = cookieCipher.Key;
                            Session["CookieIV"] = cookieCipher.IV; // security.CookieCryptoKeySize
                            string key = symC.DecryptToString(newAcc.Config.PrivateKey);
                            HttpCookie cookie = new HttpCookie("CookiePassword",
                                cookieCipher.EncryptToString(key));
                            cookie.Secure = true;
                            
                            Response.Cookies.Add(cookie);
                        }
                    }
                    FormsAuthentication.SetAuthCookie(newAcc.Alias, createPersistentCookie);
                    return RedirectToAction("Index", "Panel");
                }
                
                ModelState.AddModelError("AliasAlreadyExists", Resources.Account.Strings.AliasAlreadyExists);
            }
            
            return View(acc);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(Account acc)
        {
            if (acc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(acc.Alias))
            {
                ModelState.AddModelError("InvalidAlias", Resources.Account.Strings.InavlidAlias);
            }
            if(string.IsNullOrWhiteSpace(acc.Password))
            {
                ModelState.AddModelError("InvalidPassword", Resources.Account.Strings.InvalidPassword);
            }
            bool loginEnabled = bool.Parse(HttpContext.Application["system.EnableLogin"] as string);
            if (!loginEnabled)
            {
                ModelState.AddModelError("LoginClosed", Resources.Account.Strings.LoginDisabled);
            }
            if(ModelState.IsValid)
            {
                // Find corresponsing existing account
                Account correspondingAcc = GetAccount(acc.Alias);

                // validate password
                if (correspondingAcc != null)
                {
                    
                    if (correspondingAcc.IsEnabled)
                    {
                        bool createPersistentAuthCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());
                        int cookieKeySize = Convert.ToInt32(HttpContext.Application["security.CookieCryptoKeySize"]);

                        using (var symC = new SymmetricCipher<AesManaged>(acc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV))
                        {
                            try
                            {
                                FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, createPersistentAuthCookie);
                                correspondingAcc.Addition.LastLogin = DateTime.Now;
                                context.SaveChangesAsync();
                                
                                using(var cookieCipher = new SymmetricCipher<AesManaged>(cookieKeySize))
                                {
                                    Session["CookieKey"] = cookieCipher.Key;
                                    Session["CookieIV"] = cookieCipher.IV; // security.CookieCryptoKeySize
                                    HttpCookie cookie = new HttpCookie("CookiePassword",
                                        cookieCipher.EncryptToString(symC.Decrypt(correspondingAcc.Config.PrivateKey)));
                                    cookie.Secure = true;
                                    Response.Cookies.Add(cookie);
                                }
                                return RedirectToAction("Index", "Panel");
                            }
                            catch (CryptographicException)
                            {
                                correspondingAcc.Addition.LastFailedLogin = DateTime.Now;
                                correspondingAcc.Addition.Notifications.Add(new Notification
                                {
                                    Description = string.Format(Resources.Notifications.FailedLogin, DateTime.Now)
                                });
                                context.SaveChangesAsync();
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
            var customCulture = Session["customCulture"] as CultureInfo;
            FormsAuthentication.SignOut();
            Session.Clear();
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            if(customCulture != null)
                Session["customCulture"] = customCulture;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Overview()
        {
            return View();
        }
    }
}
