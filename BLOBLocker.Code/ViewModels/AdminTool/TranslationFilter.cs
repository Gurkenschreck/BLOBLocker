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
                leftOver = leftOver.Where(p => p.LocalizedStrings
                                                .Any(q => q.Status == TranslationStatus));
            }

            return leftOver.ToList();
        }
    }
}
