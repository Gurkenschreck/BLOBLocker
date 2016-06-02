using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
{
    [Authorize]
    [AjaxOnly]
    public class AjaxController : BaseController
    {
        [RequiredParameters("id")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DisableNotificationAsync(int id)
        {
            var accRepo = new AccountRepository(context);
            var curAcc = accRepo.GetByKey(User.Identity.Name);
            var notification = curAcc.Addition.Notifications.Where(p => p.ID == id).FirstOrDefault();
            if (notification != null)
            {
                notification.IsVisible = false;
                context.SaveChanges();
            }
            else{
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(
                new { result = "ok"}, JsonRequestBehavior.AllowGet
            );
        }
    }
}
