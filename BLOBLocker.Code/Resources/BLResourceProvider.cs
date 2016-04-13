using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace BLOBLocker.Code.Resources
{
    public class BLResourceProvider : IResourceProvider
    {
        BLWAContext context;
        public BLResourceProvider(string classKey)
        {
            context = new BLWAContext();
        }

        public object GetObject(string resourceKey, System.Globalization.CultureInfo culture)
        {
            if(culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }

            var translation = context.Translations.FirstOrDefault(p => p.Key == resourceKey);
            if (translation == null)
            {
                return "unknown";
            }
            else
            {
                var localized = translation.Translations.FirstOrDefault(p => p.UICulture == culture.Name);
                if(localized == null)
                {
                    return translation.Base;
                }
                else
                {
                    return localized.Translation;
                }
            }
        }

        public System.Resources.IResourceReader ResourceReader
        {
            get { throw new NotImplementedException(); }
        }
    }
}
