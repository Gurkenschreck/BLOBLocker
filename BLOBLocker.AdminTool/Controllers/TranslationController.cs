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

        // GET: Translation
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
                ModelState.AddModelError("key", "Translation with key " + key + " not found.");
            }

            if (ModelState.IsValid)
            {
                var etvm = new EditTranslationViewModel();
                etvm.Key = key;
                etvm.Translation = translation;
                return View(etvm);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [RequiredParameters("etvm")]
        [HttpPost]
        public ActionResult EditTranslation(EditTranslationViewModel etvm)
        {
            Translation translation = context.Translations.FirstOrDefault(p => p.Key == etvm.Key);
            if (translation == null)
            {

            }
            if (ModelState.IsValid)
            {

            }

            return View();
        }

        [RequiredParameters("ntvm")]
        [PreserveModelState]
        [HttpPost]
        public ActionResult AddTranslation(NewTranslationViewModel ntvm) // Not binding to Key correctly
        {
            if (ModelState.IsValid)
            {
                Translation transl = ntvm.Parse();
                context.Translations.Add(transl);
                context.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

    }
}