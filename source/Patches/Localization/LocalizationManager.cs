using System.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TownOfUs.Patches.Localization
{
    public class LocalizationManager
    {
        public static Dictionary<string, string> languageOptions = new Dictionary<string, string>
        {
            { "English", "" },
            { "French", "fr" },
        };
        private ResourceManager resourceManager;
        private CultureInfo currentCulture;
        private CultureInfo defaultCulture;
        public string missingTextString = "Localized text not found";
        public string currentLanguage = "en";
        private static LocalizationManager _instance;
        private const string ASSETS_LOCALIZATION_NAMESPACE = "TownOfUs.Properties.Resources";
        private static Assembly Assembly => typeof(TownOfUs).Assembly;

        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }

        public LocalizationManager()
        {
            resourceManager = new ResourceManager(ASSETS_LOCALIZATION_NAMESPACE, typeof(LocalizationManager).Assembly);
            defaultCulture = new CultureInfo("");
            loadSettings();
            currentCulture = new CultureInfo(this.currentLanguage);
        }

        public void SetLanguage(string languageCode)
        {
            currentCulture = new CultureInfo(languageCode);
        }

        public string GetString(string key)
        {
            string localizedString = resourceManager.GetString(key, currentCulture);

            if (localizedString == null)
            {
                localizedString = resourceManager.GetString(key, defaultCulture);
            }

            return localizedString ?? "Key not found";
        }

        private void loadSettings()
        {
            System.Console.WriteLine("Init Language");
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string localLowFolder = Path.Combine(Directory.GetParent(localAppData).FullName, "LocalLow");
            string path = Path.Combine(
            localLowFolder,
                "Innersloth",
                "Among Us",
                "settings.amogus"
            );
            System.Console.WriteLine($"Path : {path}");
            if (File.Exists(path))
            {
                string settingsContent = File.ReadAllText(path);
                JObject jsonSettings = JObject.Parse(settingsContent);
                string language = jsonSettings["language"]["currentLanguage"].ToString();
                string value;
                System.Console.WriteLine($"Language found in file : {language}");
                if (languageOptions.TryGetValue(language, out value))
                {
                    this.currentLanguage = value;
                }
                System.Console.WriteLine($"Language found : {currentLanguage}");
            }
            else
            {
                System.Console.WriteLine("File not found.");
            }
        }
    }

}
