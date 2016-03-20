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
using CryptoPool.Code.Membership;
using System.Text;

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

        [ChildActionOnly]
        [HttpGet]
        public ActionResult Chat(string puid)
        {
            if(string.IsNullOrWhiteSpace(puid))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            accRepo = new AccountRepository(context);

            Account curAcc = accRepo.GetAccount(User.Identity.Name);
            PoolShare curPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == puid);
            Pool curPool = curPoolShare.Pool;
            ChatViewModel cvm = new ChatViewModel();

            if (ModelState.IsValid)
            {
                cvm.PUID = puid;
                
                //Decrypt messages //take last x messages // show only since share date
                List<Message> encryptedMessageList = null; //= curPool.Messages.Where(p => p.IsVisible).ToList();
                int showAmountMessages = Request.QueryString["smc"] != null 
                    ? Convert.ToInt32(Request.QueryString["smc"]) 
                    : Convert.ToInt32(HttpContext.Application["pool.ShowLastMessageCount"]);
                int incrementShowAmountMessages = Convert.ToInt32(HttpContext.Application["pool.IncrementShowMessageCount"]);
                cvm.NextAmountShowLastMessageCount = showAmountMessages + incrementShowAmountMessages;

                if(curPoolShare.ShowSince != null)
                {
                    DateTime showSince = (DateTime)curPoolShare.ShowSince;
                    encryptedMessageList = curPool.Messages
                                                  .Skip(curPool.Messages.Count - showAmountMessages)
                                                  .Where(p => DateTime.Compare(showSince, (DateTime)p.Sent) < 0)
                                                  .OrderByDescending(p => p.Sent)
                                                  .ToList();

                }
                else
                {
                    encryptedMessageList = curPool.Messages
                                                  .Skip(curPool.Messages.Count - showAmountMessages)
                                                  .OrderByDescending(p => p.Sent)
                                                  .ToList();
                }

                cvm.NewMessage = new MessageViewModel();
                cvm.NewMessage.PUID = puid;
                if (encryptedMessageList.Count > 0)
                {
                    List<Message> decryptedMessageList = new List<Message>();

                    using (CredentialHandler credHandler = new CredentialHandler(Session))
                    {
                        HttpCookie keypartCookie = Request.Cookies["Secret"];
                        byte[] privKey = credHandler.Extract(keypartCookie, Session);

                        PoolShareHandler psHandler = new PoolShareHandler(Session, HttpContext);
                        byte[] key = psHandler.GetPoolKey(Encoding.UTF8.GetString(privKey), curPoolShare);
                        byte[] iv = curPool.Config.IV;

                        Message curMsg;
                        using (var poolCipher = new SymmetricCipher<AesManaged>(key, iv))
                        {
                            foreach (var encMsg in encryptedMessageList)
                            {
                                curMsg = encMsg;
                                curMsg.Text = poolCipher.DecryptToString(curMsg.Text);
                                decryptedMessageList.Add(curMsg);
                            }
                        }
                        Utilities.SetArrayValuesZero(privKey);
                        Utilities.SetArrayValuesZero(key);
                        Utilities.SetArrayValuesZero(iv);
                    }
                    cvm.Messages = decryptedMessageList;
                }
                else
                {
                    cvm.Messages = new List<Message>();
                }
            }

            return View(cvm);
        }

        [PreserveModelState]
        [HttpPost]
        public ActionResult SendMessage(MessageViewModel cvm)
        {
            if(cvm == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(ModelState.IsValid)
            {
                accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetAccount(User.Identity.Name);

                PoolShare curPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == cvm.PUID);
                Pool curPool = curPoolShare.Pool;

                Message msg = new Message();
                msg.Pool = curPool;
                msg.Sender = curAcc;
                
                using(CredentialHandler credHandler = new CredentialHandler(Session))
                {
                    HttpCookie keypartCookie = Request.Cookies["Secret"];
                    byte[] privKey = credHandler.Extract(keypartCookie, Session);

                    PoolShareHandler psHandler = new PoolShareHandler(Session, HttpContext);
                    byte[] curAccPoolSharePriKey;
                    byte[] key = psHandler.GetPoolKey(Encoding.UTF8.GetString(privKey), curPoolShare, out curAccPoolSharePriKey);
                    byte[] iv = curPool.Config.IV;

                    using(var poolCipher = new SymmetricCipher<AesManaged>(key, iv))
                    {
                        msg.Text = poolCipher.EncryptToString(cvm.MessageText);
                    }
                    using(var curAccPSRSACipher = new RSACipher<RSACryptoServiceProvider>(Encoding.UTF8.GetString(curAccPoolSharePriKey)))
                    {
                        msg.TextSignature = curAccPSRSACipher.SignStringToString<SHA256Cng>(msg.Text);
                    }
                    curPool.Messages.Add(msg);
                    context.SaveChanges();
                    Utilities.SetArrayValuesZero(privKey);
                    Utilities.SetArrayValuesZero(key);
                    Utilities.SetArrayValuesZero(iv);
                    Utilities.SetArrayValuesZero(curAccPoolSharePriKey);
                }
            }

            return RedirectToAction("Pool", new { puid = cvm.PUID });
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

        [RestoreModelState]
        [ChildActionOnly]
        [HttpGet]
        public ActionResult InviteUser(string puid)
        {
            InvitationViewModel ivm = new InvitationViewModel();
            ivm.PoolUID = puid;
            ivm.ShowSince = DateTime.Now;
            return View(ivm);
        }
        [PreserveModelState]
        [HttpPost]
        public ActionResult InviteUser(InvitationViewModel ivm)
        {
            var x = HttpContext.Cache["d"];
            accRepo = new AccountRepository(context);
            var corAcc = accRepo.GetAccount(ivm.InviteAlias);
            if (corAcc == null)
                ModelState.AddModelError("InviteAlias", "Account with this alias does not exist.");
            if (ivm.InviteAlias == User.Identity.Name)
                ModelState.AddModelError("InviteAlias", "You are already in this group");

            if(ModelState.IsValid)
            {
                var pool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == ivm.PoolUID);
                if (pool != null)
                {
                    if (pool.Participants.All(p => p.SharedWith.Alias != ivm.InviteAlias))
                    {
                        var curAcc = accRepo.GetAccount(User.Identity.Name);

                        PoolShare curAccPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.IsActive && p.PoolID == pool.ID);
                        if (curAccPoolShare != null)
                        {
                            int poolShareKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareKeySize"]);

                            using (var handler = new CredentialHandler(Session))
                            {
                                HttpCookie keypart = Request.Cookies["Secret"];
                                byte[] privKey = handler.Extract(keypart, Session);

                                PoolShareHandler poolHandler = new PoolShareHandler(Session, HttpContext);
                                PoolShare ps = poolHandler.Connect(curAccPoolShare, corAcc, pool, privKey);
                                if (!ivm.ShowAll)
                                {
                                    ps.ShowSince = ivm.ShowSince;
                                }
                            }
                            context.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("InviteAlias", "Cannot invite user. You shouldn't even be here.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("InviteAlias", "Account is already part of this curPool.");
                    }
                }
            }
            return RedirectToAction("PoolConfig", new { puid = ivm.PoolUID });
        }

        [RestoreModelState]
        [ChildActionOnly]
        [HttpGet]
        public ActionResult AssignMemory(string puid)
        {
            if(string.IsNullOrWhiteSpace(puid))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bool isvalid = ModelState.IsValid;
            Pool curPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetAccount(User.Identity.Name);
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
        [PreserveModelState]
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
                    pool = curAcc.PoolShares.Select(p => p.Pool).FirstOrDefault(p => p.UniqueIdentifier == mvm.PoolUniqueIdentifier);
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
                        ModelState.AddModelError("AdditionalMemoryToAdd", "Not enough additional space");
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
            
            return RedirectToAction("JoinPool", new { puid = corPool.UniqueIdentifier });
        }

        [RestoreModelState]
        [HttpGet]
        public ActionResult PoolConfig(string puid)
        {
            if (string.IsNullOrWhiteSpace(puid))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Pool corPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            if (corPool == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            
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
            else
            {
                return RedirectToAction("JoinPool", corPool.UniqueIdentifier);
            }
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
                int saltByteLength = Convert.ToInt32(HttpContext.Application["security.SaltByteLength"]);

                
                PoolRepository repo = new PoolRepository(context);
                accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetAccount(User.Identity.Name);
                
                Pool pool = poolViewModel.Generate();
                pool.UniqueIdentifier = Convert.ToBase64String(Utilities.GenerateBytes(9));
                pool.OwnerID = curAcc.ID;
                pool.Salt = Cipha.Security.Cryptography.Utilities.GenerateBytes(saltByteLength);

                int poolSymKeySize = Convert.ToInt32(HttpContext.Application["security.PoolKeySize"]);
                int poolRSAKeySize = Convert.ToInt32(HttpContext.Application["security.PoolRSAKeySize"]);
                int poolShareKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareKeySize"]);
                int poolShareRSAKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareRSAKeySize"]);

                CryptoConfiguration poolConfig = new CryptoConfiguration();
                PoolShare poolShare = new PoolShare();
                poolShare.Pool = pool;
                poolShare.SharedWith = curAcc;

                CryptoConfiguration poolShareConfig = new CryptoConfiguration();
                poolShareConfig.RSAKeySize = poolShareRSAKeySize;
                poolShareConfig.KeySize = poolShareKeySize;

                using (var accRSACipher = new RSACipher<RSACryptoServiceProvider>(curAcc.Config.PublicKey))
                {
                    using (var poolShareCipher = new SymmetricCipher<AesManaged>(poolShareKeySize))
                    {
                        // 1. PoolShare Key|IV generation
                        poolShareConfig.Key = accRSACipher.Encrypt(poolShareCipher.Key);
                        poolShareConfig.IV = accRSACipher.Encrypt(poolShareCipher.IV);
                        using (var poolRSACipher = new RSACipher<RSACryptoServiceProvider>(poolRSAKeySize))
                        {
                            using (var poolSymCipher = new SymmetricCipher<AesManaged>(poolSymKeySize))
                            {
                                poolConfig.PublicKey = poolRSACipher.ToXmlString(false);
                                poolConfig.IV = poolSymCipher.IV;

                                poolShareConfig.PrivateKey = poolShareCipher.Encrypt(poolRSACipher.ToXmlString(true));
                                poolShare.PoolKey = poolRSACipher.Encrypt(poolSymCipher.Key);

                                poolConfig.PublicKeySignature = poolRSACipher.SignStringToString<SHA256Cng>(poolConfig.PublicKey);
                            }
                        }
                    }
                }


                NotificationHelper.SendNotification(curAcc, "Pool {0} creation was a success!", pool.Description);
                
                pool.Config = poolConfig;
                poolShare.Config = poolShareConfig;
                curAcc.PoolShares.Add(poolShare);
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
