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
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace spreadbase.Controllers
{
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
                    newAcc.ContactEmail = acc.ContactEmail;
                    newAcc.Type = AccountType.Standard;

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

                    context.Accounts.Add(newAcc);
                    context.SaveChangesAsync();

                    FormsAuthentication.SetAuthCookie(newAcc.Alias, false);
                    Session["usr"] = usrHash;
                    
                    RedirectToAction("Index", "Panel");
                }

                    // Set Session

                    // Redirect to Account Home
               // }
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
            if(ModelState.IsValid)
            {
                // Find corresponsing existing account
                Account correspondingAcc = (from account in context.Accounts
                                  where account.Alias == acc.Alias
                                  select account).FirstOrDefault();

                // validate password
                if (correspondingAcc != null)
                {
                    bool pwIsRight = false;
                    using (var symC = new SymmetricCipher<AesManaged>(correspondingAcc.Password, correspondingAcc.Salt, correspondingAcc.Config.IV))
                    {
                        try
                        {
                            symC.DecryptToString(correspondingAcc.Password);
                            pwIsRight = true;
                        }
                        catch (CryptographicException)
                        {
                            // Log wrong password try?
                        }
                    }
                    if (pwIsRight)
                    {
                        FormsAuthentication.SetAuthCookie(correspondingAcc.Alias, false);
                        using (var hasher = new Hasher<SHA1Cng>())
                        {
                            Session["usr"] = hasher.HashToString(correspondingAcc.Alias);
                        }

                        RedirectToAction("Index", "Panel");
                    }
                }
            }
            return View(acc);
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View();
        }

        public ActionResult Overview()
        {
            return View();
        }
    }
}
