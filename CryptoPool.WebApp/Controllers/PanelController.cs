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
using Cipha.Security.Cryptography;
using CryptoPool.WebApp.Models;
using CryptoPool.Entities.Models.Models.WebApp;

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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveAssigned(int assid, string puid)
        {
            if (assid < 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var not = context.AssignedMemory.FirstOrDefault(p => p.ID == assid);
            if(not !=  null)
            {
                not.IsEnabled = false;
                context.SaveChanges();
            }
            return RedirectToAction("PoolConfig", new { puid=puid });
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult AssignMemory(string puid)
        {
            if(string.IsNullOrWhiteSpace(puid))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pool curPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            Account curAcc = curPool.Owner;
            MemoryViewModel mvm = new MemoryViewModel();
            mvm.TotalPoolMemory = curPool.AssignedMemory.Where(p => p.IsEnabled).Select(p => p.Space).Sum();
            mvm.AssignedMemory = curPool.AssignedMemory.Where(p => p.IsEnabled).ToList();
            mvm.FreeBasicMemory = curAcc.MemoryPool.BasicSpace - curAcc
                .MemoryPool.AssignedMemory
                .Where(p => p.IsBasic && p.IsEnabled)
                .Select(p => p.Space).Sum();
            mvm.FreeAdditionalMemory = curAcc.MemoryPool.AdditionalSpace - curAcc
                .MemoryPool.AssignedMemory
                .Where(p => !p.IsBasic && p.IsEnabled)
                .Select(p => p.Space).Sum();
            mvm.PoolUniqueIdentifier = curPool.UniqueIdentifier;
            return View(mvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignMemory(MemoryViewModel mvm)
        {
            if (mvm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetAccount(User.Identity.Name);

            if(ModelState.IsValid)
            {
                Pool pool = curAcc.Pools.FirstOrDefault(p => p.UniqueIdentifier == mvm.PoolUniqueIdentifier);
                if (pool == null)
                    pool = curAcc.ForeignPools.Select(p => p.Pool).FirstOrDefault(p => p.UniqueIdentifier == mvm.PoolUniqueIdentifier);
                AssignedMemory assignBasicMemory = null;
                AssignedMemory assignAdditionalMemory = null;
                int basicMemLeft, additionalMemLeft;
                accRepo.MemoryLeft(curAcc, out basicMemLeft, out additionalMemLeft);

                if(mvm.BasicMemoryToAdd > 0)
                {
                    if (basicMemLeft >= mvm.BasicMemoryToAdd)
                    {
                        assignBasicMemory = new AssignedMemory();
                        assignBasicMemory.MemoryPool = curAcc.MemoryPool;
                        assignBasicMemory.Space = mvm.BasicMemoryToAdd;
                        pool.AssignedMemory.Add(assignBasicMemory);
                        curAcc.MemoryPool.AssignedMemory.Add(assignBasicMemory);
                    }
                    else
                    {
                        ModelState.AddModelError("BasicMemoryToAdd", "Not enough free basic space");
                    }
                }
                if (mvm.AdditionalMemoryToAdd > 0)
                {
                    if (additionalMemLeft >= mvm.AdditionalMemoryToAdd)
                    {
                        assignAdditionalMemory = new AssignedMemory();
                        assignAdditionalMemory.MemoryPool = curAcc.MemoryPool;
                        assignAdditionalMemory.Space = mvm.BasicMemoryToAdd;
                        pool.AssignedMemory.Add(assignAdditionalMemory);
                        curAcc.MemoryPool.AssignedMemory.Add(assignAdditionalMemory);
                    }
                    else
                    {
                        ModelState.AddModelError("AdditionalMemoryToAdd", "Not enough free basic space");
                    }
                }
                if (assignBasicMemory != null || assignAdditionalMemory != null)
                    context.SaveChanges();
            }
            return RedirectToAction("PoolConfig", new { puid = mvm.PoolUniqueIdentifier });
        }

        [HttpGet]
        public ActionResult JoinPool(string puid)
        {
            var tmodelstate = TempData["ModelState"] as ModelStateDictionary;
            ModelState.Merge(tmodelstate);

            Pool corPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            return View(corPool);
        }

        [HttpGet]
        public ActionResult Pool(string puid)
        {
            if (string.IsNullOrWhiteSpace(puid))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Pool corPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            if (corPool == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (ModelState.IsValid)
            {
                accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetAccount(User.Identity.Name);
                if (accRepo.HasPoolRights(curAcc, corPool))
                {
                    ViewBag.CurrentAccount = curAcc;
                    ViewBag.Pool = corPool;
                    ViewBag.AssignedPoolSpace = (corPool.AssignedMemory.Count != 0) ? corPool.AssignedMemory
                                                                                            .Where(p => p.IsEnabled)
                                                                                            .Select(p => p.Space)
                                                                                            .Sum() : 0;
                    return View();
                }
            }
            return RedirectToAction("JoinPool", corPool.UniqueIdentifier);
        }

        [HttpGet]
        public ActionResult PoolConfig(string puid)
        {
            if (string.IsNullOrWhiteSpace(puid))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Pool corPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            if (corPool == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (ModelState.IsValid)
            {
                accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetAccount(User.Identity.Name);
                if (accRepo.HasPoolRights(curAcc, corPool))
                {
                    PoolConfigModel configModel = new PoolConfigModel();
                    configModel.Pool = corPool;
                    configModel.Account = curAcc;
                    configModel.IsOwner = corPool.OwnerID == curAcc.ID;

                    ViewBag.AssignedPoolSpace = (corPool.AssignedMemory.Count != 0) ? corPool.AssignedMemory
                                                                                            .Where(p => p.IsEnabled)
                                                                                            .Select(p => p.Space)
                                                                                            .Sum() : 0;

                    return View();
                }
            }
            return RedirectToAction("JoinPool", corPool.UniqueIdentifier);
        }

        [HttpPost]
        public ActionResult PoolConfig(PoolConfigModel configModel)
        {
            if (configModel == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if(ModelState.IsValid)
            {
                PoolRepository poolRepo = new PoolRepository(context);
                Pool adw = context.Pools.FirstOrDefault(p => p.ID == configModel.Pool.ID); 
            }


            return View();
        }

        [HttpGet]
        public ActionResult Build()
        {
            BuildPoolViewModel p = new BuildPoolViewModel();
            return View(p);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Build(BuildPoolViewModel poolViewModel)
        {
            if(poolViewModel == null)
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
                Pool pool = poolViewModel.Generate();
                pool.UniqueIdentifier = Convert.ToBase64String(Utilities.GenerateBytes(9));
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
            return View(poolViewModel);
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

                        NotificationHelper.SendNotification(curAcc, Resources.Notifications.AddAccount, correspondingAccount.Alias);
                        NotificationHelper.SendNotification(correspondingAccount, Resources.Notifications.GetAdded, curAcc.Alias);

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

                        NotificationHelper.SendNotification(correspondingAccount, CryptoPool.WebApp.Resources.Notifications.ShareContactsReceived, curAcc.Alias);
                        NotificationHelper.SendNotification(curAcc, CryptoPool.WebApp.Resources.Notifications.ShareContactsShared, correspondingAccount.Alias);

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
