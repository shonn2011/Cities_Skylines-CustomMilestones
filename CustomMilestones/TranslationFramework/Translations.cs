namespace CustomMilestones.TranslationFramework
{
    public class Translations
    {
        private static Translator _translator;

        public static string Translate(string key, string standby) => Instance.Translate(key, standby);

        private static Translator Instance
        {
            get
            {
                if (_translator == null)
                {
                    _translator = new Translator();
                }

                return _translator;
            }
        }
    }
}
