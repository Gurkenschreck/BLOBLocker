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
using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography;
using BLOBLocker.Code.Membership;
using System.Text;
using BLOBLocker.Code.ViewModels.WebApp;
using BLOBLocker.Code.Security.Cryptography;
using BLOBLocker.Code.Data;
using BLOBLocker.Code.Web;
using System.IO;
using System.Net.Mime;
using BLOBLocker.Code.Exception;
using System.Web.UI;
using System.Web.Security;

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

        [RestoreModelState, PreserveModelState]
        [RequiredParameters("puid")]
        [HttpGet]
        public ActionResult Pool(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);
            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    PoolOverviewViewModel povm = new PoolOverviewViewModel();
                    var corPool = poolHandler.Pool;
                    povm.Populate(corPool);

                    PoolShare curPoolShare = poolHandler.PoolShare;
                    povm.CurrentPoolShare = curPoolShare;
                    
                    if (!string.IsNullOrWhiteSpace(corPool.Description))
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                        {
                            byte[] cc = css["PrivRSAKey"];
                            poolHandler.Initialize(cc);
                        }
                        using (var poolCipher = poolHandler.GetPoolCipher())
                        {
                            povm.Description = poolCipher.DecryptToString(corPool.Description);
                        }
                    }
                    TempData["rights"] = curPoolShare.Rights;
                    return View(povm);
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [RequiredParameters("puid")]
        [HttpGet]
        public ActionResult Upload(string puid)
        {
            NewFileViewModel nfvm = new NewFileViewModel();
            nfvm.PUID = puid;
            return View(nfvm);
        }
        
        [RequiredParameters("nfvm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UploadMinimal(NewFileViewModel nfvm)
        {
            if (ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);

                bool canAccessPool;
                using (var poolHandler = new PoolHandler(curAcc, nfvm.PUID, out canAccessPool))
                {
                    if (canAccessPool)
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                                Session, Request, Response))
                        {
                            poolHandler.Initialize(css["PrivRSAKey"]);
                        }
                        string fileStorePath = HttpContext.Application["system.PoolBasePath"].ToString();

                        var vfiles = nfvm.Parse();
                        foreach (var file in vfiles)
                        {
                            file.FilePath = fileStorePath;
                            file.IPv4Address = Request.UserHostAddress;
                            StoringMode newStoringMode = HttpContext.Application["pool.NewStoringMode"].As<StoringMode>();
                            try
                            {
                                var storedFile = poolHandler.StoreFile(file, newStoringMode);
                            }
                            catch (NotEnoughPoolSpaceException ex)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                ModelState.AddModelError("Files", HttpContext.GetGlobalResourceObject(null, "Pool.NotEnoughFreeSpaceError").ToString());
                            }
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("JoinPool", new { puid = nfvm.PUID });
                    }
                }
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("puid")]
        [RestoreModelState]
        [HttpGet]
        public ActionResult Storage(string puid)
        {
            AccountRepository accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);
            bool canAccessPool;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccessPool))
            {
                if (canAccessPool)
                {
                    var svm = new StorageViewModel();
                    svm.PUID = puid;
                    svm.PoolTitle = poolHandler.Pool.Title;
                    svm.Files = poolHandler.GetFiles();
                    TempData["rights"] = poolHandler.PoolShare.Rights;
                    return View(svm);
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [RestoreModelState]
        [RequiredParameters("puid")]
        [HttpGet, ChildActionOnly]
        public ActionResult StorageMinimal(string puid)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                var svm = new StorageViewModel();
                svm.PUID = puid;
                svm.PoolTitle = poolHandler.Pool.Title;

                svm.Files = poolHandler.GetFiles();
                return PartialView("_Storage", svm);
            }
        }

        [RequiredParameters("puid", "fl")]
        [HttpGet]
        public ActionResult Download(string puid, string fl, bool dl)
        {
            AccountRepository accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                    {
                        poolHandler.Initialize(css["PrivRSAKey"]);
                    }
                    string fileStorePath = HttpContext.Application["system.PoolBasePath"].ToString();
                    var virtualFile = poolHandler.GetFile(fl, fileStorePath, true);
                    string completeFileName = string.Format("{0}.{1}", virtualFile.FileName, virtualFile.FileExtension);

                    if (dl)
                    {
                        return File(virtualFile.Content, virtualFile.MimeType, completeFileName);
                    }
                    else
                    {
                        var cd = new ContentDisposition
                        {
                            FileName = string.Format("{0} - {1} - BLOBLocker", completeFileName, poolHandler.PoolShare.Pool.Title),
                            Inline = true
                        };

                        Response.AddHeader("Content-Disposition", cd.ToString());
                        return File(virtualFile.Content, virtualFile.MimeType);
                    }
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [RequiredParameters("puid", "fl")]
        [HttpGet]
        public ActionResult Preview(string puid, string fl)
        {
            AccountRepository accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                    {
                        poolHandler.Initialize(css["PrivRSAKey"]);
                    }
                    string fileStorePath = HttpContext.Application["system.PoolBasePath"].ToString();


                    // get previewable from ApplicationContext
                    string previewAbleFullString = HttpContext.Application["pool.PreviewableExtensions"].ToString();
                    string[] previewAble = previewAbleFullString.Replace(" ", string.Empty).Split(',');

                    // get max preview size from ApplicationContext
                    int maxByteSize = HttpContext.Application["pool.MaxPreviewByteSize"].As<int>();

                    VirtualFile virtualFile = null;
                    try
                    {
                        virtualFile = poolHandler.GetFile(fl, fileStorePath,
                            true, previewAble, maxByteSize);
                    }
                    catch (FileNotFoundException ex)
                    {
                        var fileEntity = poolHandler.Pool.Files.FirstOrDefault(p => p.StoredFileName == fl);
                        if (fileEntity != null)
                        {
                            fileEntity.IsDeleted = true;
                            context.SaveChanges();
                        }
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                        return RedirectToAction("Pool", new { puid = puid });
                    }

                    ViewBag.Rights = poolHandler.PoolShare.Rights;

                    return View(virtualFile);
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [ValidateAntiForgeryToken]
        [RequiredParameters("puid", "fl")]
        [HttpPost]
        public ActionResult ToggleFile(string puid, string fl)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    try
                    {
                        poolHandler.ToggleFile(fl);
                        context.SaveChanges();
                    }
                    catch (PoolFileNotFoundException ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [ValidateAntiForgeryToken]
        [RequiredParameters("puid", "fl")]
        [HttpPost]
        public ActionResult DeleteFile(string puid, string fl)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    try
                    {
                        string baseFilePath = HttpContext.Application["system.PoolBasePath"].ToString();
                        poolHandler.DeleteFile(fl, baseFilePath);
                        context.SaveChanges();
                    }
                    catch (PoolFileNotFoundException ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
            return RedirectToAction("Pool", new { puid = puid });
        }

        [NoCache]
        [RequiredParameters("puid")]
        [RestoreModelState]
        [HttpGet]
        public ActionResult PoolConfig(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccessPool;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccessPool))
            {
                if (canAccessPool)
                {
                    var corPool = poolHandler.Pool;
                    PoolConfigModel configModel = new PoolConfigModel();
                    configModel.Populate(corPool, curAcc);
                    configModel.TitleDescriptionViewModel.Rights = poolHandler.PoolShare.Rights;

                    if (!string.IsNullOrWhiteSpace(corPool.Description))
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                        {
                            poolHandler.Initialize(css["PrivRSAKey"]);
                        }
                        using (var cipher = poolHandler.GetPoolCipher())
                        {
                            configModel.TitleDescriptionViewModel.Description = cipher.DecryptToString(corPool.Description);
                        }
                    }
                    return View(configModel);
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [RequiredParameters("tdvm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeTitleAndDescription(TitleDescriptionViewModel tdvm)
        {
            if (ModelState.IsValid)
            {
                AccountRepository accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);

                bool canAccessPool;
                using (var poolHandler = new PoolHandler(curAcc, tdvm.PUID, out canAccessPool))
                {
                    if (canAccessPool)
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                        {
                            poolHandler.Initialize(css["PrivRSAKey"]);
                        }
                        poolHandler.Pool.Title = tdvm.Title;
                        if (tdvm.Description != null)
                        {
                            using (var cipher = poolHandler.GetPoolCipher())
                            {
                                poolHandler.Pool.Description = cipher.EncryptToString(tdvm.Description);
                            }
                        }
                        else
                        {
                            poolHandler.Pool.Description = null;
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("JoinPool", new { puid = tdvm.PUID });
                    }
                }
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("mmvm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
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
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("revm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditDefaultRights(RightsEditViewModel revm)
        {
            var curAcc = context.Accounts.FirstOrDefault(p => p.Alias == User.Identity.Name);
            PoolShare ps = curAcc.PoolShares.FirstOrDefault(p => p.Pool.UniqueIdentifier == revm.PoolUID);
            if(ModelState.IsValid)
            {
                ps.Pool.DefaultRights = PoolRightHelper.CalculateRights(revm.Rights);
                context.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("puid")]
        [HttpGet]
        public ActionResult Chat(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    ChatViewModel cvm = new ChatViewModel();
                    cvm.PoolTitle = poolHandler.Pool.Title;
                    cvm.PoolShare = poolHandler.PoolShare;
                    ViewBag.Rights = cvm.PoolShare;
                    cvm.PUID = puid;

                    //Decrypt messages //take last x messages // show only since share date
                    int showAmountMessages = Request.QueryString["smc"] != null
                        ? Convert.ToInt32(Request.QueryString["smc"])
                        : Convert.ToInt32(HttpContext.Application["pool.ShowLastMessageCount"]);

                    int incrementShowAmountMessages = Convert.ToInt32(HttpContext.Application["pool.IncrementShowMessageCount"]);
                    cvm.NextAmountShowLastMessageCount = showAmountMessages + incrementShowAmountMessages;

                    cvm.NewMessage = new MessageViewModel();
                    cvm.NewMessage.PUID = puid;

                    using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                    {
                        poolHandler.Initialize(css["PrivRSAKey"]);
                    }
                    ICollection<Message> plainMessages;
                    poolHandler.GetChat(cvm.NextAmountShowLastMessageCount, out plainMessages);
                    cvm.Messages = plainMessages ?? new List<Message>();
                    return View(cvm);
                }
                else
                {
                    return RedirectToAction("JoinPool", new { puid = puid });
                }
            }
        }

        [RequiredParameters("puid")]
        [ChildActionOnly]
        [HttpGet]
        public ActionResult ChatMinimal(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);

            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    ChatViewModel cvm = new ChatViewModel();
                    cvm.PoolTitle = poolHandler.Pool.Title;
                    cvm.PoolShare = poolHandler.PoolShare;
                    ViewBag.Rights = cvm.PoolShare;
                    cvm.PUID = puid;

                    //Decrypt messages //take last x messages // show only since share date
                    int showAmountMessages = Request.QueryString["smc"] != null
                        ? Convert.ToInt32(Request.QueryString["smc"])
                        : Convert.ToInt32(HttpContext.Application["pool.ShowLastMessageCount"]);

                    int incrementShowAmountMessages = Convert.ToInt32(HttpContext.Application["pool.IncrementShowMessageCount"]);
                    cvm.NextAmountShowLastMessageCount = showAmountMessages + incrementShowAmountMessages;

                    cvm.NewMessage = new MessageViewModel();
                    cvm.NewMessage.PUID = puid;

                    using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                    {
                        poolHandler.Initialize(css["PrivRSAKey"]);
                    }
                    ICollection<Message> plainMessages;
                    poolHandler.GetChat(cvm.NextAmountShowLastMessageCount, out plainMessages);
                    cvm.Messages = plainMessages ?? new List<Message>();
                    return PartialView("_Chat", cvm);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
        }

        [RequiredParameters("cvm")]
        [PreserveModelState]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SendMessage(MessageViewModel cvm)
        {
            if(ModelState.IsValid)
            {
                var accRepo = new AccountRepository(context);
                Account curAcc = accRepo.GetByKey(User.Identity.Name);

                bool canAccess;
                using (PoolHandler poolHandler = new PoolHandler(curAcc, cvm.PUID, out canAccess))
                {
                    if (canAccess)
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                                Session, Request, Response))
                        {
                            // get css PrivRSAKey to bytearr
                            poolHandler.Initialize(css["PrivRSAKey"]);
                            // set extracted css to null.
                        }

                        Message msg = poolHandler.SendMessage(cvm.MessageText);
                        if (msg != null)
                        {
                            // Success
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("JoinPool", new { puid = cvm.PUID });
                    }
                }
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("assid", "puid")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RemoveAssigned(string puid, int assid)
        {
            var not = context.AssignedMemory.FirstOrDefault(p => p.ID == assid);
            if(not !=  null)
            {
                not.IsEnabled = false;
                context.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("puid")]
        [RestoreModelState]
        [ChildActionOnly]
        [HttpGet]
        public ActionResult InviteUserMinimal(string puid)
        {
            InvitationViewModel ivm = new InvitationViewModel();
            ivm.PoolUID = puid;
            ivm.ShowSince = DateTime.Now;
            return PartialView("_InviteUser", ivm);
        }

        [RequiredParameters("ivm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult InviteUserMinimal(InvitationViewModel ivm)
        {
            var accRepo = new AccountRepository(context);
            var corAcc = accRepo.GetByKey(ivm.InviteAlias);
            if (corAcc == null)
                ModelState.AddModelError("InviteAlias", HttpContext.GetGlobalResourceObject(null, "Account.AccountNonexistent").As<string>());
            if (ivm.InviteAlias == User.Identity.Name)
                ModelState.AddModelError("InviteAlias", HttpContext.GetGlobalResourceObject(null, "Account.SelfIsInGroup").As<string>());

            if (ModelState.IsValid)
            {
                var curAcc = accRepo.GetByKey(User.Identity.Name);

                bool canAccess;

                using (PoolHandler poolHandler = new PoolHandler(curAcc, ivm.PoolUID, out canAccess))
                {
                    if (canAccess)
                    {
                        if (!poolHandler.IsUserInPool(ivm.InviteAlias))
                        {
                            int poolShareSymKeySize = Convert.ToInt32(HttpContext.Application["security.PoolShareKeySize"]);

                            using (var css = new CryptoSessionStore("AccPriKey",
                                Session, Request, Response))
                            {
                                poolHandler.Initialize(css["PrivRSAKey"]);
                            }

                            PoolShare ps = poolHandler.AddToPool(corAcc, poolShareSymKeySize);
                            if (!ivm.ShowAll)
                            {
                                ps.ShowSince = ivm.ShowSince;
                            }
                            context.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("InviteAlias",
                                string.Format(HttpContext.GetGlobalResourceObject(null, "Pool.AccountAlreadyInPool").As<string>(), ivm.InviteAlias));
                        }
                    }
                    else
                    {
                        return RedirectToAction("JoinPool", new { puid = ivm.PoolUID });
                    }
                }
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("puid")]
        [RestoreModelState]
        [HttpGet, ChildActionOnly]
        public ActionResult AssignMemoryMinimal(string puid)
        {
            var accRepo = new AccountRepository(context);
            Account curAcc = accRepo.GetByKey(User.Identity.Name);
            bool canAccess;
            using (var poolHandler = new PoolHandler(curAcc, puid, out canAccess))
            {
                if (canAccess)
                {
                    MemoryViewModel mvm = new MemoryViewModel();
                    mvm.Populate(curAcc, poolHandler.Pool);
                    return PartialView("_AssignMemory", mvm);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
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
            return Redirect(Request.UrlReferrer.AbsoluteUri);
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
        [HttpPost, ValidateAntiForgeryToken]
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
                using (PoolHandler poolHandler = new PoolHandler(curAcc, pool))
                {
                    PoolShare poolShare = poolHandler.SetupNew(9,
                        PoolRightHelper.CalculateRights(poolViewModel.Rights),
                        saltByteLength,
                        poolShareKeySize,
                        poolShareRSAKeySize,
                        poolRSAKeySize,
                        poolSymKeySize);

                    if (!string.IsNullOrWhiteSpace(poolViewModel.Description))
                    {
                        using (var css = new CryptoSessionStore("AccPriKey",
                            Session, Request, Response))
                        {
                            poolHandler.Initialize(css["PrivRSAKey"]);
                        }
                        using (var poolCipher = poolHandler.GetPoolCipher())
                        {
                            pool.Description = poolCipher.EncryptToString(poolViewModel.Description);
                        }
                    }
                }

                BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(curAcc,
                    HttpContext.GetGlobalResourceObject(null, "Notification.PoolCreationSuccess").As<string>(),
                    pool.Title);

                poolRepo.Add(pool);
                return RedirectToAction("Pool", new { puid = pool.UniqueIdentifier });
            }
            return View(poolViewModel);
        }

        [HttpGet]
        public ActionResult AddContact()
        {
            var accRepo = new AccountRepository(context);
            return View(accRepo.GetByKey(User.Identity.Name));
        }

        [RequiredParameters("acvm")]
        [HttpPost, ValidateAntiForgeryToken]
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

        [RequiredParameters("shareWithAlias")]
        [HttpPost, ValidateAntiForgeryToken]
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
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("id")]
        [HttpPost, ValidateAntiForgeryToken]
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
