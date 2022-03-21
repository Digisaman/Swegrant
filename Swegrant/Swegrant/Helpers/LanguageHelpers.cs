using Swegrant.Resources;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Swegrant.Helpers
{
    public class LanguageHelper
    {
        public static void ChangeLanguage(Language language)
        {
            string languageName = "";
            if (language != Language.None)
                languageName = (language == Language.Svenska ? "Swedish" : "Persian");
            else
                languageName = "English";

            var lang = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList().First(element => element.EnglishName.Contains(languageName));
            Thread.CurrentThread.CurrentUICulture = lang;
            AppResources.Culture = lang;
        }
    }
}
