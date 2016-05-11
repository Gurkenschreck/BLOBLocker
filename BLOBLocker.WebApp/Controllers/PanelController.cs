using Cipha.Security.Cryptography.Asymmetric;
using BLOBLocker.Code.Extention;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.WebApp.Controllers;
using BLOBLocker.Entities.Models;
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
using BLOBLocker.WebApp.Resources;
using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography;
using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Code.Membership;
using System.Text;
using BLOBLocker.Code.ViewModels.WebApp;
using BLOBLocker.Code.Security.Cryptography;
using BLOBLocker.Code.Data;

namespace BLOBLocker.WebApp.Controllers
{
    public class PanelController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var accRepo = new AccountRepository(context);
            var acc = accRepo.GetByKey(HttpContext.User.Identity.Name);

            PanelIndexViewModel pivm = new PanelIndexViewModel();
            pivm.Addition = acc.Addition;
            pivm.Pools = acc.Pools.Where(p => p.IsActive);
            pivm.PoolShares = acc.PoolShares.Where(p => p.IsActive);
            pivm.Notifications = acc.Addition.Notifications.Where(p => p.IsVisible);
            return View(pivm);
        }

        [RequiredParameters("puid")]
        [HttpGet]
        public ActionResult Pool(string puid)
        {
            Pool corPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            if (corPool == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            using (var poolHandler = new PoolHandler(curAcc, corPool))
            {
                if (poolHandler.CanAccessPool)
                {
                    PoolOverviewViewModel povm = new PoolOverviewViewModel();
                    povm.Populate(corPool);

                    PoolShare curPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == puid);
                    povm.CurrentPoolShare = curPoolShare;

                    if (!string.IsNullOrWhiteSpace(corPool.Description))
                    {
                        byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                        byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                        byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                        HttpCookie keypartCookie = Request.Cookies["Secret"];

                        poolHandler.Initialize(Convert.FromBase64String(keypartCookie.Value),
                            sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);
                        using (var poolCipher = poolHandler.GetPoolCipher())
                        {
                            povm.Description = poolCipher.DecryptToString(corPool.Description);
                        }
                    }

                    ViewBag.Rights = curPoolShare.Rights;
                    return View(povm);
                }
            }

            return RedirectToAction("JoinPool", new { puid = corPool.UniqueIdentifier });
        }

        [RequiredParameters("puid")]
        [RestoreModelState]
        [HttpGet]
        public ActionResult PoolConfig(string puid)
        {
            PoolRepository poolRepo = new PoolRepository(context);

            Pool corPool = poolRepo.GetByKey(puid);
            if (corPool == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);
            using (PoolHandler poolHandler = new PoolHandler(curAcc, corPool))
            {
                if (poolHandler.CanAccessPool)
                {
                    PoolConfigModel configModel = new PoolConfigModel();
                    configModel.Populate(corPool, curAcc);
                    configModel.TitleDescriptionViewModel.Rights = poolHandler.CorrespondingPoolShare.Rights;

                    byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                    byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                    byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                    HttpCookie keypartCookie = Request.Cookies["Secret"];

                    poolHandler.Initialize(Convert.FromBase64String(keypartCookie.Value),
                        sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);

                    if (!string.IsNullOrWhiteSpace(corPool.Description))
                    {
                        using (var cipher = poolHandler.GetPoolCipher())
                        {
                            configModel.TitleDescriptionViewModel.Description = cipher.DecryptToString(corPool.Description);
                        }
                    }

                    return View(configModel);
                }
                else
                {
                    return RedirectToAction("JoinPool", corPool.UniqueIdentifier);
                }
            }
        }

        [ValidateAntiForgeryToken]
        [RequiredParameters("tdvm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult ChangeTitleAndDescription(TitleDescriptionViewModel tdvm)
        {
            if (ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);
                PoolRepository poolRepo = new PoolRepository(context);
                Pool curPool = poolRepo.GetByKey(tdvm.PUID);

                curPool.Title = tdvm.Title;

                if (string.IsNullOrWhiteSpace(tdvm.Description))
                    tdvm.Description = string.Empty;
                
                using (PoolHandler poolHandler = new PoolHandler(curAcc, curPool))
                {
                    byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                    byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                    byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                    HttpCookie keypartCookie = Request.Cookies["Secret"];

                    poolHandler.Initialize(Convert.FromBase64String(keypartCookie.Value),
                        sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);
                    using (var cipher = poolHandler.GetPoolCipher())
                    {
                        curPool.Description = cipher.EncryptToString(tdvm.Description);
                    }
                }
                
                context.SaveChanges();
            }

            return RedirectToAction("PoolConfig", new { puid = tdvm.PUID });
        }

        [ValidateAntiForgeryToken]
        [RequiredParameters("mmvm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult ManageModules(ManageModulesViewModel mmvm)
        {
            if (ModelState.IsValid)
            {
                PoolRepository poolRepo = new PoolRepository(context);
                Pool curPool = poolRepo.GetByKey(mmvm.PUID);

                curPool.ChatEnabled = mmvm.EnableChat;
                curPool.FileStorageEnabled = mmvm.EnableFileStorage;
                curPool.LinkRepositoryEnabled = mmvm.EnableLinkRepository;

                context.SaveChanges();
            }
            return RedirectToAction("PoolConfig", new { puid = mmvm.PUID });
        }

        [RequiredParameters("revm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreserveModelState]
        public ActionResult EditDefaultRights(RightsEditViewModel revm)
        {
            var curAcc = context.Accounts.FirstOrDefault(p => p.Alias == User.Identity.Name);
            PoolShare ps = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == revm.PoolUID);
            if(ModelState.IsValid)
            {
                ps.Pool.DefaultRights = PoolRightHelper.CalculateRights(revm.Rights);
                context.SaveChanges();
            }
            return RedirectToAction("PoolConfig", new { puid = revm.PoolUID });
        }

        [RequiredParameters("puid")]
        [ChildActionOnly]
        [HttpGet]
        public ActionResult Chat(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            PoolShare curPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == puid);
            Pool curPool = curPoolShare.Pool;
            ChatViewModel cvm = new ChatViewModel();
            if (ModelState.IsValid)
            {
                cvm.PoolShare = curPoolShare;
                ViewBag.Rights = curPoolShare.Rights;
                cvm.PUID = puid;
                
                //Decrypt messages //take last x messages // show only since share date
                int showAmountMessages = Request.QueryString["smc"] != null 
                    ? Convert.ToInt32(Request.QueryString["smc"]) 
                    : Convert.ToInt32(HttpContext.Application["pool.ShowLastMessageCount"]);

                int incrementShowAmountMessages = Convert.ToInt32(HttpContext.Application["pool.IncrementShowMessageCount"]);
                cvm.NextAmountShowLastMessageCount = showAmountMessages + incrementShowAmountMessages;

                cvm.NewMessage = new MessageViewModel();
                cvm.NewMessage.PUID = puid;

                byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                HttpCookie keypartCookie = Request.Cookies["Secret"];

                using (PoolHandler poolHandler = new PoolHandler(curAcc, curPool))
                {
                    poolHandler.Initialize(Convert.FromBase64String(keypartCookie.Value), sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);

                    ICollection<Message> plainMessages;
                    poolHandler.GetChat(cvm.NextAmountShowLastMessageCount, out plainMessages);
                    cvm.Messages = plainMessages ?? new List<Message>();
                }
            }

            return View(cvm);
        }

        [RequiredParameters("cvm")]
        [PreserveModelState]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SendMessage(MessageViewModel cvm)
        {
            if(ModelState.IsValid)
            {
                var accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);

                PoolShare curPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == cvm.PUID);
                Pool curPool = curPoolShare.Pool;

                Message msg = new Message();
                msg.Pool = curPool;
                msg.Sender = curAcc;

                byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                HttpCookie keypartCookie = Request.Cookies["Secret"];

                using (PoolHandler poolHandler = new PoolHandler(curAcc, curPool))
                {
                    poolHandler.Initialize(Convert.FromBase64String(keypartCookie.Value),
                        sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);
                    byte[] poolPrivateRSAKey;
                    using (var poolCipher = poolHandler.GetPoolCipher(out poolPrivateRSAKey))
                    {
                        msg.Text = poolCipher.EncryptToString(cvm.MessageText);

                        using (var rsaCipher = new RSACipher<RSACryptoServiceProvider>(Encoding.UTF8.GetString(poolPrivateRSAKey)))
                        {
                            msg.TextSignature = rsaCipher.SignStringToString<SHA256Cng>(msg.Text);
                        }
                    }
                }
                curPool.Messages.Add(msg);
                context.SaveChanges();
            }

            return RedirectToAction("Pool", new { puid = cvm.PUID });
        }

        [RequiredParameters("assid", "puid")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveAssigned(int assid, string puid)
        {
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

        [RequiredParameters("ivm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult InviteUser(InvitationViewModel ivm)
        {
            var accRepo = new AccountRepository(context);
            var corAcc = accRepo.GetByKey(ivm.InviteAlias);
            if (corAcc == null)
                ModelState.AddModelError("InviteAlias", HttpContext.GetGlobalResourceObject(null, "Account.AccountNonexistent").As<string>());
            if (ivm.InviteAlias == User.Identity.Name)
                ModelState.AddModelError("InviteAlias", HttpContext.GetGlobalResourceObject(null, "Account.SelfIsInGroup").As<string>());

            if(ModelState.IsValid)
            {
                var pool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == ivm.PoolUID);
                if (pool != null)
                {
                    if (pool.Participants.All(p => p.SharedWith.Alias != ivm.InviteAlias))
                    {
                        var curAcc = accRepo.GetByKey(User.Identity.Name);
                        PoolShare curAccPoolShare = curAcc.PoolShares.FirstOrDefault(p => p.IsActive && p.PoolID == pool.ID);

                        HttpCookie keypartCookie = Request.Cookies["Secret"];
                        byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                        byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                        byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                        int poolShareSymKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareKeySize"]);

                        using (PoolHandler ph = new PoolHandler(curAcc, pool))
                        {
                            ph.Initialize(Convert.FromBase64String(keypartCookie.Value),
                                sessionCookieKey, sessionCookieIV, sessionStoredKeyPart);
                            PoolShare ps = ph.AddToPool(corAcc, poolShareSymKeySize);

                            if (!ivm.ShowAll)
                            {
                                ps.ShowSince = ivm.ShowSince;
                            }
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("InviteAlias",
                            string.Format(HttpContext.GetGlobalResourceObject(null, "Pool.AccountAlreadyInPool").As<string>(), ivm.InviteAlias));
                    }
                }
            }
            return RedirectToAction("PoolConfig", new { puid = ivm.PoolUID });
        }

        [RequiredParameters("puid")]
        [RestoreModelState]
        [ChildActionOnly, HttpGet]
        public ActionResult AssignMemory(string puid)
        {
            bool isvalid = ModelState.IsValid;
            Pool curPool = context.Pools.FirstOrDefault(p => p.UniqueIdentifier == puid);
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);
            MemoryViewModel mvm = new MemoryViewModel();
            mvm.Populate(curAcc, curPool);
            return View(mvm);
        }

        [RequiredParameters("mvm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AssignMemory(MemoryViewModel mvm)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            if(ModelState.IsValid)
            {
                Pool pool = curAcc.PoolShares.Select(p => p.Pool).FirstOrDefault(p => p.UniqueIdentifier == mvm.PoolUniqueIdentifier);
                MemoryPoolHandler memPoolHandler = new MemoryPoolHandler(curAcc);
                switch(memPoolHandler.AssignMemoryToPool(pool, mvm.BasicMemoryToAdd, true))
                {
                    case 2:
                        ModelState.AddModelError("BasicMemoryToAdd",
                            HttpContext.GetGlobalResourceObject(null, "MemoryPool.NotEnoughBasicSpace").As<string>());
                    break;
                }

                switch (memPoolHandler.AssignMemoryToPool(pool, mvm.AdditionalMemoryToAdd, false))
                {
                    case 2:
                        ModelState.AddModelError("AdditionalMemoryToAdd",
                            HttpContext.GetGlobalResourceObject(null, "MemoryPool.NotEnoughAdditionalSpace").As<string>());
                        break;
                }

                context.SaveChanges();
            }
            return RedirectToAction("PoolConfig", new { puid = mvm.PoolUniqueIdentifier });
        }

        [HttpGet]
        public ActionResult JoinPool(string puid)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Build()
        {
            BuildPoolViewModel p = new BuildPoolViewModel();
            p.Rights = PoolRightHelper.GetRights(PoolRightHelper.GetMaxRights()).ToArray();
            return View(p);
        }

        [RequiredParameters("poolViewModel")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Build(BuildPoolViewModel poolViewModel)
        {
            if(ModelState.IsValid)
            {
                int saltByteLength = HttpContext.Application["security.SaltByteLength"].As<int>();
                int poolSymKeySize = HttpContext.Application["security.PoolKeySize"].As<int>();
                int poolRSAKeySize = HttpContext.Application["security.PoolRSAKeySize"].As<int>();
                int poolShareKeySize = HttpContext.Application["security.PoolShareKeySize"].As<int>();
                int poolShareRSAKeySize = HttpContext.Application["security.PoolShareRSAKeySize"].As<int>();

                PoolRepository poolRepo = new PoolRepository(context);
                var accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);
                Pool pool = poolViewModel.Generate();
                using (PoolHandler ph = new PoolHandler(curAcc, pool))
                {
                    PoolShare poolShare = ph.SetupNew(9,
                        PoolRightHelper.CalculateRights(poolViewModel.Rights),
                        saltByteLength,
                        poolShareKeySize,
                        poolShareRSAKeySize,
                        poolRSAKeySize,
                        poolSymKeySize);

                    if (!string.IsNullOrWhiteSpace(poolViewModel.Description))
                    {
                        byte[] sessionCookieKey = Session["AccPriKeyCookieKey"] as byte[];
                        byte[] sessionCookieIV = Session["AccPriKeyCookieIV"] as byte[];
                        byte[] sessionStoredKeyPart = Session["AccPriKeySessionStoredKeyPart"] as byte[];
                        HttpCookie keypartCookie = Request.Cookies["Secret"];

                        ph.Initialize(Convert.FromBase64String(keypartCookie.Value), sessionCookieKey,
                            sessionCookieIV, sessionStoredKeyPart);
                        using (var poolCipher = ph.GetPoolCipher())
                        {
                            pool.Description = poolCipher.EncryptToString(poolViewModel.Description);
                        }
                    }
                }

                BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(curAcc,
                    HttpContext.GetGlobalResourceObject(null, "Notification.PoolCreationSuccess").As<string>(),
                    pool.Title);

                poolRepo.Add(pool);
                return RedirectToAction("Index");
            }
            return View(poolViewModel);
        }

        [HttpGet]
        public ActionResult AddContact()
        {
            var accRepo = new AccountRepository(context);
            return View(accRepo.GetByKey(User.Identity.Name));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddContact(AddContactViewModel acvm)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            if (acvm.AddAlias == curAcc.Alias)
                ModelState.AddModelError("AddAlias", HttpContext.GetGlobalResourceObject(null, "Contact.CannotAddYourself").As<string>());

            if(ModelState.IsValid)
            {
                var correspondingAccount = accRepo.GetByKey(acvm.AddAlias);
                if(correspondingAccount != null)
                {
                    AccountHandler accountHandler = new AccountHandler(curAcc);
                    Contact contact = accountHandler.AddContact(correspondingAccount);
                    if (contact == null)
                    {
                        ModelState.AddModelError("AddModel", HttpContext.GetGlobalResourceObject(null, "Contact.AccountAlreadyAdded").As<string>());
                    }
                    else{
                        context.SaveChanges();                        
                    }
                }
                else
                {
                    ModelState.AddModelError("AddAlias", HttpContext.GetGlobalResourceObject(null, "Contact.AccountNonExistent").As<string>());
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
                ModelState.AddModelError("shareWithAlias", HttpContext.GetGlobalResourceObject(null, "Contact.BadAddAlias").As<string>());
            }
            if (ModelState.IsValid)
            {
                var accRepo = new AccountRepository(context);
                var curAcc = accRepo.GetByKey(User.Identity.Name);
                var correspondingAccount = accRepo.GetByKey(shareWithAlias);

                if (correspondingAccount != null)
                {
                    if (curAcc.ID != correspondingAccount.ID)
                    {
                        AccountHandler accHandler = new AccountHandler(curAcc);

                        accHandler.ShareContactsWith(correspondingAccount);
                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("shareWithAlias", HttpContext.GetGlobalResourceObject(null, "Contact.CannotShareWithYourself").As<string>());
                    }
                }
                else
                {
                    ModelState.AddModelError("shareWithAlias", HttpContext.GetGlobalResourceObject(null, "Contact.AccountNonexistent").As<string>());
                }
            }
            return RedirectToAction("AddContact");
        }

        [RequiredParameters("id")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public void DisableNotification(int id)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);
            var notification = curAcc.Addition.Notifications.Where(p => p.ID == id).FirstOrDefault();
            if (notification != null)
            {
                notification.IsVisible = false;
                context.SaveChanges();
            }
            Response.Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}
