using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.ModelHelper;
using BLOBLocker.Code.ViewModels.AdminTool;
using BLOBLocker.Entities.Models.AdminTool;
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
        public ActionResult Index()
        {
            var translationViewModel = new TranslationIndexViewModel();

            var curAcc = atcontext.Accounts.FirstOrDefault(p => User.Identity.Name == p.Alias);
            translationViewModel.IsModerator = curAcc.Roles.Any(p => p.Role.Definition == "Moderator");
            translationViewModel.IsTranslator = curAcc.Roles.Any(p => p.Role.Definition == "Translator");
            translationViewModel.Translations = context.Translations.ToList();
            return View(translationViewModel);
        }

        [PreserveModelState]
        [HttpGet]
        public ActionResult EditTranslation(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var translation = context.Translations.FirstOrDefault(p => p.Key == key);
            if (translation == null)
            {
                ModelState.AddModelError("key", "StringResource with key " + key + " not found.");
            }

            if (ModelState.IsValid)
            {
                var etvm = new EditTranslationViewModel();
                etvm.Key = key;
                etvm.StringResource = translation;
                return View(etvm);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("etvm")]
        [HttpPost]
        public ActionResult EditTranslation(EditTranslationViewModel etvm)
        {
            StringResource translation = context.Translations.FirstOrDefault(p => p.Key == etvm.Key);
            
            if (ModelState.IsValid)
            {
                translation.Key = etvm.StringResource.Key;
                translation.Comment = etvm.StringResource.Comment;
                translation.Type = etvm.StringResource.Type;

                foreach (var lstr in translation.LocalizedStrings)
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

            return View(etvm);
        }

        [RequiredParameters("ntvm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult AddTranslation(NewTranslationViewModel ntvm) 
        {
            if (ModelState.IsValid)
            {
                StringResource transl = ntvm.Parse();
                context.Translations.Add(transl);
                context.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

    }
}