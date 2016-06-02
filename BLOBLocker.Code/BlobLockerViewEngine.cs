using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code
{
    public class BlobLockerViewEngine : RazorViewEngine
    {
        public BlobLockerViewEngine()
        {
            var newLocationFormat = new[]
                                        {
                                            "~/Views/{1}/Partial/{0}.cshtml",
                                            "~/Views/Shared/Partial/{0}.cshtml"
                                        };

            PartialViewLocationFormats = PartialViewLocationFormats.Union(newLocationFormat).ToArray();
        }
    }
}
