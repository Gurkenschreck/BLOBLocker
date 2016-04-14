using BLOBLocker.Code.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using BLOBLocker.Code.Extention;
using BLOBLocker.Entities.Models.Models.WebApp;

namespace BLOBLocker.Code.Attributes
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        readonly IResourceProvider provider;

        public LocalizedDisplayNameAttribute(string displayName, Type resourceProvider = null)
            : base(displayName)
        {
            if (resourceProvider != null)
            {
                provider = (IResourceProvider)Activator.CreateInstance(resourceProvider);
            }
            else
            {
                provider = new BLResourceProvider(string.Empty);
            }
        }

        public override string DisplayName
        {
            get
            {
                return (provider.GetObject(base.DisplayName, null)).As<string>();
            }
        }
    }
}
