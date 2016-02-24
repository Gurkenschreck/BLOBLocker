using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using spreadbase.Models;
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

namespace spreadbase.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AccountController : Controller
    {
        SpreadBaseContext context = new SpreadBaseContext();
        //
        // GET: /Account/
        [AllowAnonymous]
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
                Account newAcc = (from account in context.Accounts
                                     where account.Alias == acc.Alias
                                     select account).FirstOrDefault();

                if(newAcc == null)
                {
                    int saltLenght = Convert.ToInt32(ConfigurationManager.AppSettings["SaltByteLength"]);
                    int keySize = Convert.ToInt32(ConfigurationManager.AppSettings["RSAKeySize"]);

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

                    var cryptoConfig = new spreadbase.Models.CryptoConfig();
                    byte[] salt;
                    byte[] iv;

                    
                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, out salt, out iv))
                    {
                        newAcc.Salt = salt;
                        cryptoConfig.IV = iv;

                        using (var rsaC = new RSACipher<RSACryptoServiceProvider>(keySize))
                        {
                            cryptoConfig.PublicKey = rsaC.ToXmlString(false);
                            cryptoConfig.PrivateKey = rsaC.ToEncryptedXmlString<AesManaged>(true, acc.Password, salt, iv);
                            cryptoConfig.PublicKeySignature = rsaC.SignStringToString<SHA512Cng>(cryptoConfig.PublicKey);
                        }

                        newAcc.Password = symC.EncryptToString(pwHash);
                    }
                    newAcc.Config = cryptoConfig;
                    newAcc.Addition.LastLogin = DateTime.Now;


                    context.Accounts.Add(newAcc);

                    int x = context.SaveChanges();
                    
                    

                    FormsAuthentication.SetAuthCookie(newAcc.Alias, false);
                    
                    return RedirectToAction("Index", "Panel");
                }
                
                ModelState.AddModelError("AliasExists", "There is already an account using this alias");
            }
            
            return View(acc);
        }

        [AllowAnonymous]
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
                Account correspondingAcc = (from account in context.Accounts
                                  where account.Alias == acc.Alias
                                  select account).FirstOrDefault();

                // validate password
                if (correspondingAcc != null)
                {
                    using (var symC = new SymmetricCipher<AesManaged>(acc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV))
                    {
                        try
                        {
                            string decr = symC.DecryptToString(correspondingAcc.Password);

                            FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, false);
                            correspondingAcc.Addition.LastLogin = DateTime.Now;

                            int a = context.SaveChanges();

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

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Overview()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
