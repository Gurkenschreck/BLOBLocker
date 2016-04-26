using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class TranslationFilter
    {
        public string SearchKey { get; set; }
        public string SearchCultures { get; set; }
        public TranslationType? TranslationType { get; set; }
        public TranslationStatus? TranslationStatus { get; set; }

        public IList<StringResource> ApplyFilter(IList<StringResource> resources)
        {
            IEnumerable<StringResource> leftOver = resources;

            if (SearchKey != null)
            {
                leftOver = leftOver.Where(p => p.Key.Contains(SearchKey));
            }

            if (TranslationType != null)
            {
                leftOver = leftOver.Where(p => p.Type == TranslationType);
            }

            if (TranslationStatus != null)
            {
                if (SearchCultures != null)
                {
                    foreach (var culture in SearchCultures.Replace(" ", "").Split(','))
                    {
                        leftOver = leftOver.Where(p => p.LocalizedStrings.Any(q => q.UICulture == culture && q.Status == TranslationStatus));
                    }
                }
                else
                {
                    leftOver = leftOver.Where(p => p.LocalizedStrings
                                                .Any(q => q.Status == TranslationStatus));
                }
            }
            else
            {
                if (SearchCultures != null)
                {
                    foreach (var culture in SearchCultures.Replace(" ", "").Split(','))
                    {
                        leftOver = leftOver.Where(p => p.LocalizedStrings.Any(q => q.UICulture == SearchCultures));
                    }
                }
            }

            return leftOver.ToList();
        }
    }
}
