using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using CryptoPool.WebApp.Controllers;
using CryptoPool.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using CryptoPool.WebApp.Resources;
using CryptoPool.Code.Attributes;
using CryptoPool.Code.Controllers;
using CryptoPool.Code.ModelHelper;
using CryptoPool.Entities.Models.WebApp;

namespace CryptoPool.WebApp.Controllers
{
    public class PanelController : BaseController
    {
        AccountRepository accRepo;

        [HttpGet]
        public ActionResult Index()
        {
            accRepo = new AccountRepository(context);
            var acc = accRepo.GetAccount(HttpContext.User.Identity.Name);
            return View(acc);
        }

        [HttpGet]
        public ActionResult Build()
        {
            Pool p = new Pool();
            return View(p);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Build(Pool newPool)
        {
            if(newPool == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(ModelState.IsValid)
            {
                int symKeySize = Convert.ToInt32(HttpContext.Application["security.PoolKeySize"]);
                int rsaKeySize = Convert.ToInt32(HttpContext.Application["security.PoolRSAKeySize"]);
                int poolSize = Convert.ToInt32(HttpContext.Application["security.SaltByteLength"]);
                int hashIterations = Convert.ToInt32(HttpContext.Application["security.HashIterationCount"]);

                PoolRepository repo = new PoolRepository(context);
                accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetAccount(User.Identity.Name);
                Pool pool = new Pool();
                pool.Description = newPool.Description;
                pool.OwnerID = curAcc.ID;
                CryptoConfiguration config = new CryptoConfiguration();
                
                pool.Salt = Cipha.Security.Cryptography.Utilities.GenerateBytes(poolSize);

                using (var symCipher = new SymmetricCipher<AesManaged>(symKeySize))
                {

                    config.IV = symCipher.IV;
                    config.Key = symCipher.Key;
                    config.KeySize = symCipher.KeySize;
                    config.IterationCount = hashIterations;
                    using (var asymCipher = new RSACipher<RSACryptoServiceProvider>())
                    {
                        config.RSAKeySize = asymCipher.KeySize;
                        config.PublicKey = asymCipher.ToXmlString(false);
                        config.PrivateKey = Convert.FromBase64String(asymCipher.ToEncryptedXmlString<AesManaged>(true, symCipher.Key, symCipher.IV));
                        config.PublicKeySignature = asymCipher.SignStringToString<SHA256Cng>(config.PublicKey);
                    }
                }
                pool.Config = config;
                context.Pools.Add(pool);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newPool);
        }

        [HttpGet]
        public ActionResult AddContact()
        {
            accRepo = new AccountRepository(context);
            return View(accRepo.GetAccount(User.Identity.Name));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddContact(string addAlias)
        {
            if (addAlias == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(addAlias))
            {
                ModelState.AddModelError("addAlias", "Invalid addAlias");
            }
            accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetAccount(User.Identity.Name);
            if(ModelState.IsValid)
            {
                var correspondingAccount = accRepo.GetAccount(addAlias);
                if(correspondingAccount != null)
                {
                    if(curAcc.Addition.Contacts.All(p => p.AccountID != correspondingAccount.ID))
                    {
                        curAcc.Addition.Contacts.Add(new Contact
                        {
                            AccountID = correspondingAccount.ID
                        });

                        curAcc.Addition.Notifications.Add(NotificationHelper.SendNotification(Resources.Notifications.AddAccount,
                            correspondingAccount.Alias));
                        correspondingAccount.Addition.Notifications.Add(NotificationHelper.SendNotification(Resources.Notifications.GetAdded,
                            curAcc.Alias));

                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("addModel", "User already in your list.");
                    }
                }
                else
                {
                    ModelState.AddModelError("addAlias", "No such user found");
                }
            }
            return View(curAcc);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShareContacts(string shareWithAlias)
        {
            if (string.IsNullOrWhiteSpace(shareWithAlias))
            {
                ModelState.AddModelError("addAlias", "Invalid addAlias");
            }
            if (ModelState.IsValid)
            {
                accRepo = new AccountRepository(context);
                var curAcc = accRepo.GetAccount(User.Identity.Name);
                var correspondingAccount = accRepo.GetAccount(shareWithAlias);

                if (correspondingAccount != null)
                {
                    if (curAcc.ID != correspondingAccount.ID)
                    {
                        ICollection<Contact> corAccContacts = correspondingAccount.Addition.Contacts;
                        foreach (Contact contact in curAcc.Addition.Contacts)
                        {
                            if (!corAccContacts.Any(c => c.AccountID == contact.AccountID))
                            {
                                corAccContacts.Add(new Contact
                                {
                                    AccountID = contact.AccountID
                                });
                            }
                        }

                        var corNotification = new Notification();
                        corNotification.Description = string.Format("{0} hash shared all his contacts with out.", curAcc.Alias);
                        correspondingAccount.Addition.Notifications.Add(corNotification);
                        
                        var curNotification = new Notification();
                        curNotification.Description = string.Format("You shared all your contacts with {0}.", correspondingAccount.Alias);
                        curAcc.Addition.Notifications.Add(curNotification);

                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("addAlias", "You cannot share with yourself");
                    }
                }
                else
                {
                    ModelState.AddModelError("addAlias", "No such user found");
                }
            }
            return RedirectToAction("AddContact");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DisableNotification(int id)
        {
            var notification = context.Notifications.Where(p => p.ID == id).FirstOrDefault();
            if (notification != null)
            {
                notification.IsVisible = false;
                context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HttpPost]
        public string DisableNotificationAsync(int id)
        {
            if (id < 0)
                return "id<0";
            var notification = context.Notifications.Where(p => p.ID == id).FirstOrDefault();
            if (notification == null)
                return "notificationNotExistent";
            else
            {
                notification.IsVisible = false;
                try
                {
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }
            }
            return "ok";
        }
    }
}
