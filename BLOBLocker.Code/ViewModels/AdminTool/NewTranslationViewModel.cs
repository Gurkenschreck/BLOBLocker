using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class NewResourceViewModel
    {
        public NewResourceViewModel()
        {
            Base = "nt";
            Comment = "-";
            Type = TranslationType.Global;
            Languages = "en,de,pl,es";
        }

        [Required]
        public string Key { get; set; }
        public string Base { get; set; }
        public string Comment { get; set; }
        public TranslationType Type { get; set; }
        public string Languages { get; set; }

        public StringResource Parse()
        {
            StringResource translation = new StringResource();
            translation.Key = Key;
            translation.Type = Type;
            translation.Base = Base;
            translation.Comment = Comment;

            var langs = Languages.Split(',');

            foreach (var lang in langs)
            {
                if (translation.LocalizedStrings.All(p => p.UICulture != lang))
                {
                    LocalizedString lstr = new LocalizedString();
                    lstr.BaseResource = translation;
                    lstr.UICulture = lang;
                    translation.LocalizedStrings.Add(lstr);
                }
            }

            return translation;
        }
    }
}
