using CryptoPool.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoPool.Entities.Models.WebApp;

namespace CryptoPool.Code.ModelHelper
{
    public static class NotificationHelper
    {
        public static Notification SendNotification(Account reciever, string message)
        {
            return SendNotification(reciever, message);
        }
        public static Notification SendNotification(string formatMessage, params object[] args)
        {
            var notification = new Notification();
            notification.Description = string.Format(formatMessage, args);
            return notification;
        }
        public static Notification SendNotification(Account reciever, string formatMessage, params object[] args)
        {
            var notification = new Notification();
            notification.Description = string.Format(formatMessage, args);
            reciever.Addition.Notifications.Add(notification);
            return notification;
        }
    }
}
