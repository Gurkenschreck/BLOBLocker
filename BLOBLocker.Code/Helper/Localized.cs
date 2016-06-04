using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BLOBLocker.Code.Helper
{
    public static class Localized
    {
        public static MvcHtmlString LocalizedActionLink(this HtmlHelper htmlHelper, string resourceKey, string action)
        {
            return htmlHelper.ActionLink(HttpContext.GetGlobalResourceObject(null, resourceKey).ToString(), action);
        }

        public static MvcHtmlString LocalizedActionLink(this HtmlHelper htmlHelper, string resourceKey, string action, string controller)
        {
            return htmlHelper.ActionLink(HttpContext.GetGlobalResourceObject(null, resourceKey).ToString(),
                action, controller);
        }

        public static MvcHtmlString LocalizedActionLink(this HtmlHelper htmlHelper, string resourceKey, string action, object routeValues)
        {
            return htmlHelper.ActionLink(HttpContext.GetGlobalResourceObject(null, resourceKey).ToString(),
                action, routeValues);
        }
        public static MvcHtmlString LocalizedActionLink(this HtmlHelper htmlHelper, string resourceKey, string action, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink(HttpContext.GetGlobalResourceObject(null, resourceKey).ToString(),
                action, routeValues, htmlAttributes);
        }

        public static MvcHtmlString LocalizedActionLink(this HtmlHelper htmlHelper, string resourceKey, string action, string controller, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLink(HttpContext.GetGlobalResourceObject(null, resourceKey).ToString(),
                action, controller, routeValues, htmlAttributes);
        }

        public static MvcHtmlString LocalizedLabel(this HtmlHelper htmlHelper, string key)
        {
            return htmlHelper.Label(HttpContext.GetGlobalResourceObject(null, key).ToString());
        }

        public static MvcHtmlString LocalizedLabel(this HtmlHelper htmlHelper, string key, object htmlAttributes)
        {
            return htmlHelper.Label(HttpContext.GetGlobalResourceObject(null, key).ToString(),
                htmlAttributes);
        }
    }
}