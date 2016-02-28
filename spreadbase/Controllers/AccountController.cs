using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using SpreadBase.Controllers;
using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SignUp(Account acc)
        {
            if(acc == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(acc.Alias))
            {
                ModelState.AddModelError("Name", "Please enter a name");
            }
            if (string.IsNullOrWhiteSpace(acc.Password))
            {
                ModelState.AddModelError("Password", "Please enter a password");
            }
            // Check if info is correct
            if(ModelState.IsValid)
            {
                Account newAcc = context.Accounts.FirstOrDefault(x => x.Alias == acc.Alias);

                if(newAcc == null)
                {
                    int saltLenght = Convert.ToInt32(HttpContext.Application["security.SaltByteLength"]);
                    int keySize = Convert.ToInt32(HttpContext.Application["security.RSAKeySize"]);
                    bool createPersistentCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());

                    newAcc = new Account();
                    newAcc.Alias = acc.Alias;
                    newAcc.Addition = new AccountAddition();
                    newAcc.Addition.ContactEmail = acc.Addition.ContactEmail;

                    string pwHash;
                    string usrHash;
                    using (var hasher = new Hasher<SHA1Cng>())
                    {
                        usrHash = hasher.HashToString(newAcc.Alias);
                        pwHash = hasher.HashToString(acc.Password);

                    }

                    var cryptoConfig = new SpreadBase.Models.CryptoConfig();
                    byte[] salt;
                    byte[] iv;

                    
                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, out salt, out iv))
                    {
                        newAcc.Salt = salt;
                        cryptoConfig.IV = iv;

                        using (var rsaC = new RSACipher<RSACryptoServiceProvider>(keySize))
                        {
                            cryptoConfig.PublicKey = rsaC.ToXmlString(false);
                            cryptoConfig.PrivateKey = Convert.FromBase64String(rsaC.ToEncryptedXmlString<AesManaged>(true, acc.Password, salt, iv));
                            cryptoConfig.PublicKeySignature = rsaC.SignStringToString<SHA256Cng>(cryptoConfig.PublicKey);
                        }

                        newAcc.Password = symC.EncryptToString(pwHash);
                    }
                    newAcc.Config = cryptoConfig;
                    newAcc.Addition.LastLogin = DateTime.Now;
                    newAcc.Roles = new List<AccountRoleLink>();
                    newAcc.Roles.Add(new AccountRoleLink()
                    {
                        Account = newAcc,
                        Role = context.AccountRoles.FirstOrDefault(x => x.RoleName == HttpContext.Application["account.DefaultRole"].ToString())
                    });

                    context.Accounts.Add(newAcc);

                    context.SaveChanges();
                    
                    FormsAuthentication.SetAuthCookie(newAcc.Alias, createPersistentCookie);
                    
                    return RedirectToAction("Index", "Panel");
                }
                
                ModelState.AddModelError("AliasExists", "There is already an account using this shareWithAliasias");
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
                ModelState.AddModelError("Alias", "Invalid Alias");
            }
            if(string.IsNullOrWhiteSpace(acc.Password))
            {
                ModelState.AddModelError("Password", "Insufficient Password");
            }
            if(ModelState.IsValid)
            {
                // Find corresponsing existing account
                Account correspondingAcc = context.Accounts.FirstOrDefault(p => p.Alias == acc.Alias);

                // validate password
                if (correspondingAcc != null)
                {
                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV))
                    {
                        try
                        {
                            string decr = symC.DecryptToString(correspondingAcc.Password);
                            bool createPersistentAuthCookie = bool.Parse(HttpContext.Application["security.CreatePersistentAuthCookie"].ToString());

                            FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, createPersistentAuthCookie);
                            correspondingAcc.Addition.LastLogin = DateTime.Now;
                            context.SaveChangesAsync();

                            return RedirectToAction("Index", "Panel");
                        }
                        catch (CryptographicException)
                        {
                            correspondingAcc.Addition.LastFailedLogin = DateTime.Now;
                            context.SaveChangesAsync();
                            // Log wrong password try?
                        }
                    }
                }
                ModelState.AddModelError("AliasOrPassword", "Alias and/or password wrong");
            }
            return View(acc);
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Overview()
        {
            return View();
        }
    }
}
