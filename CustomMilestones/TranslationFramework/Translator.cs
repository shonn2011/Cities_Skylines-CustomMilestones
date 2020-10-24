using ColossalFramework.Globalization;
using CustomMilestones.Helpers;
using CustomMilestones.Models;
using System.Collections.Generic;
using System.IO;

namespace CustomMilestones.TranslationFramework
{
    public class Translator
    {
        private readonly string _localeVirtualPath = "Resources\\Locales";
        private readonly string _defaultLanguage = "en";
        private readonly SortedList<string, LanguageModel> _languages;
        private LanguageModel _currentLanguage = null;

        public Translator()
        {
            _languages = new SortedList<string, LanguageModel>();
            LoadLanguages();
            LocaleManager.eventLocaleChanged += SetCurrentLanguage;
        }

        public string Translate(string key, string standby)
        {
            if (_currentLanguage == null)
            {
                SetCurrentLanguage();
            }
            if (_currentLanguage != null)
            {
                if (_currentLanguage.KeyValuePairs.ContainsKey(key))
                {
                    return _currentLanguage.KeyValuePairs[key];
                }
            }
            return standby;
        }

        private void LoadLanguages()
        {
            _languages.Clear();
            if (!string.IsNullOrEmpty(ModHelper.GetPath()))
            {
                string localePath = Path.Combine(ModHelper.GetPath(), _localeVirtualPath);
                if (Directory.Exists(localePath))
                {
                    string[] languageFiles = Directory.GetFiles(localePath);
                    foreach (string languageFile in languageFiles)
                    {
                        LanguageModel language = JsonHelper.FromJsonFile<LanguageModel>(languageFile);
                        if (language != null)
                        {
                            _languages.Add(language.UniqueName, language);
                        }
                    }
                }
            }
        }

        private void SetCurrentLanguage()
        {
            if (LocaleManager.exists)
            {
                string localeLanguage = LocaleManager.instance.language;
                if (!_languages.ContainsKey(localeLanguage))
                {
                    localeLanguage = _defaultLanguage;
                }
                if (_languages.ContainsKey(localeLanguage) && (_currentLanguage == null || _currentLanguage.UniqueName != localeLanguage))
                {
                    _currentLanguage = _languages[localeLanguage];
                }
            }
        }
    }
}
