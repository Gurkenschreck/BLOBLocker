using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.Code.Helper
{
    public static class VerticalMenu
    {
        public static HtmlString CreateMenu(this HtmlHelper htmlHelper, ICollection<Pool> visiblePools)
        {
            string htmlMenu = "";
            
            return new HtmlString(htmlMenu);
        }
    }
}
