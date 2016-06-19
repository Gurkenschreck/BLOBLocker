using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BLOBLocker.Code.Web.Helper
{
    public static class Hidden
    {
        public static HtmlString HiddenFromQueryString(this HtmlHelper helper, string queryKey)
        {
            HttpContext context = HttpContext.Current;
            var qkey = context.Request.QueryString.AllKeys.FirstOrDefault(p => p.ToUpper() == queryKey.ToUpper());
            if (string.IsNullOrWhiteSpace(qkey))
            {
                return new HtmlString("");
            }
            else
            {
                return helper.Hidden(qkey, context.Request.QueryString[qkey].ToString());
            }
        }
    }
}
