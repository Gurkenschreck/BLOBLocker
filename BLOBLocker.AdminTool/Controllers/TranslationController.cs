using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.ModelHelper;
using BLOBLocker.Code.ViewModels.AdminTool;
using BLOBLocker.Entities.Models.AdminTool;
using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.AdminTool.Controllers
{
    [Authorize(Roles = "Moderator,Translator")]
    public class TranslationController : BaseController
    {
        BLATContext atcontext = new BLATContext();

        // GET: StringResource
        [RestoreModelState]
        [HttpGet]
        public ActionResult Index(TranslationFilter tivm)
        {
            var translationViewModel = new TranslationIndexViewModel();

            var curAcc = atcontext.Accounts.FirstOrDefault(p => User.Identity.Name == p.Alias);

            translationViewModel.IsModerator = curAcc.Roles.Any(p => p.Role.Definition == "Moderator");
            translationViewModel.IsTranslator = curAcc.Roles.Any(p => p.Role.Definition == "Translator");

            if (tivm != null)
                translationViewModel.StringResources = tivm.ApplyFilter(context.StringResources.ToList());
            else
                translationViewModel.StringResources = context.StringResources.ToList();
            
            return View(translationViewModel);
        }

        [PreserveModelState]
        [HttpGet]
        public ActionResult EditResource(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var resource = context.StringResources.FirstOrDefault(p => p.Key == key);

            if (resource == null)
                ModelState.AddModelError("key", "StringResource with key " + key + " not found.");
            

            if (ModelState.IsValid)
            {
                var etvm = new EditResourceViewModel();
                etvm.ResourceID = resource.ID;
                etvm.StringResource = resource;

                var curAcc = atcontext.Accounts.FirstOrDefault(p => User.Identity.Name == p.Alias);
                etvm.IsModerator = curAcc.Roles.Any(p => p.Role.Definition == "Moderator");

                return View(etvm);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("etvm")]
        [HttpPost]
        public ActionResult EditResource(EditResourceViewModel etvm)
        {
            StringResource resource = context.StringResources.FirstOrDefault(p => p.ID == etvm.ResourceID);
            
            if (ModelState.IsValid)
            {
                if (etvm.IsModerator)
                {
                    resource.Key = etvm.StringResource.Key;
                    resource.Comment = etvm.StringResource.Comment;
                    resource.Type = etvm.StringResource.Type;
                    if (resource.Base != etvm.StringResource.Base)
                    {
                        resource.Base = etvm.StringResource.Base;
                        if (etvm.MajorBaseChange)
                        {
                            foreach (var lstr in resource.LocalizedStrings)
                            {
                                if(lstr.Status != TranslationStatus.New)
                                    lstr.Status = TranslationStatus.BaseModified;
                            }
                        }
                    }
                }

                foreach (var lstr in resource.LocalizedStrings)
                {
                    var modifiedLstr = etvm.StringResource.LocalizedStrings.FirstOrDefault(p => p.ID == lstr.ID);
                    if (lstr.Translation != modifiedLstr.Translation)
                    {
                        lstr.Status = TranslationStatus.Translated;
                        lstr.Translation = modifiedLstr.Translation;
                    }
                }
                context.SaveChanges();
            }

            return RedirectToAction("EditResource", new { key = resource.Key });
        }

        [RequiredParameters("ntvm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult AddResource(NewResourceViewModel ntvm) 
        {
            StringResource sres = context.StringResources.FirstOrDefault(p => p.Key == ntvm.Key);

            if (sres != null)
                ModelState.AddModelError("Key", "Key already registered.");
            

            if (ModelState.IsValid)
            {
                sres = ntvm.Parse();
                context.StringResources.Add(sres);
                context.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteResource(int removeID)
        {
            var resource = context.StringResources.FirstOrDefault(p => p.ID == removeID);

            if (resource == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            context.StringResources.Remove(resource);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SetTranslationLive(int resourceID, string cultures)
        {
            var resource = context.StringResources.FirstOrDefault(p => p.ID == resourceID);

            if (resource == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            foreach (var cult in cultures.Split(','))
            {
                if (resource.LocalizedStrings.Any(p => p.UICulture == cult))
                {
                    LocalizedString remLocStr = resource.LocalizedStrings.First(p => p.UICulture == cult);
                    remLocStr.LiveTranslation = remLocStr.Translation;
                    remLocStr.Status = TranslationStatus.Live;
                }
            }
            context.SaveChanges();

            return RedirectToAction("EditResource", new { key = resource.Key });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ModifyUICultures(int resourceID, string addCultures, string removeCultures)
        {
            StringResource sres = context.StringResources.FirstOrDefault(p => p.ID == resourceID);

            if (sres == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!string.IsNullOrWhiteSpace(addCultures))
            {
                foreach (var addCult in addCultures.Split(','))
                {
                    if (sres.LocalizedStrings.All(p => p.UICulture != addCult))
                    {
                        var addLocStr = new LocalizedString();
                        addLocStr.BaseResource = sres;
                        addLocStr.UICulture = addCult;
                        sres.LocalizedStrings.Add(addLocStr);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(removeCultures))
            {
                foreach (var removeCult in removeCultures.Split(','))
                {
                    if (sres.LocalizedStrings.Any(p => p.UICulture == removeCultures))
                    {
                        LocalizedString remLocStr = sres.LocalizedStrings.First(p => p.UICulture == removeCult);
                        sres.LocalizedStrings.Remove(remLocStr);
                    }
                }
            }
            context.SaveChanges();

            return RedirectToAction("EditResource", new { key = sres.Key });
        }
    }
}