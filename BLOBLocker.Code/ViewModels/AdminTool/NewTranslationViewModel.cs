using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class NewTranslationViewModel
    {
        public NewTranslationViewModel()
        {
            Base = "nt";
            Comment = "-";
            Type = TranslationType.Global;
            Languages = "de,pl,es";
        }

        [Required]
        public string Key { get; set; }
        public string Base { get; set; }
        public string Comment { get; set; }
        public TranslationType Type { get; set; }
        public string Languages { get; set; }

        public Translation Parse()
        {
            Translation translation = new Translation();
            translation.Key = Key;
            translation.Type = Type;
            translation.Base = Base;
            translation.Comment = Comment;

            var langs = Languages.Split(',');

            foreach (var lang in langs)
            {
                LocalizedString lstr = new LocalizedString();
                lstr.TranslationOf = translation;
                lstr.UICulture = lang;
            }

            return translation;
        }
    }
}
