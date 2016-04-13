using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
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
        [ValidateAntiForgeryToken]
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
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "ok";
        }
    }
}
