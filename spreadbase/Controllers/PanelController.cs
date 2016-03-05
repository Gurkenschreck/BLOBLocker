using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using SpreadBase.App_Code.Validation;
using SpreadBase.Controllers;
using SpreadBase.Models;
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

namespace SpreadBase.Controllers
{
    public class PanelController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var acc = GetAccount();
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

                Account curAcc = GetAccount();
                Pool pool = new Pool();
                pool.Description = newPool.Description;
                pool.OwnerID = curAcc.ID;
                SpreadBase.Models.CryptoConfig config = new SpreadBase.Models.CryptoConfig();
                
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
            return View(GetAccount());
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
            Account curAcc = GetAccount();
            if(ModelState.IsValid)
            {
                var correspondingAccount = GetAccount(addAlias);
                if(correspondingAccount != null)
                {
                    if(curAcc.Addition.Contacts.All(p => p.AccountID != correspondingAccount.ID))
                    {
                        curAcc.Addition.Contacts.Add(new Contact
                        {
                            AccountID = correspondingAccount.ID
                        });

                        Notification not1 = new Notification();
                        not1.Description = string.Format(Resources.Notifications.AddAccount, correspondingAccount.Alias);
                        curAcc.Addition.Notifications.Add(not1);

                        Notification not2 = new Notification();
                        not2.Description = string.Format(Resources.Notifications.GetAdded, curAcc.Alias);
                        correspondingAccount.Addition.Notifications.Add(not2);

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
                var curAcc = GetAccount();
                var correspondingAccount = GetAccount(shareWithAlias);

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
