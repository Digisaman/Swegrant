using Newtonsoft.Json;
using Swegrant.Shared.Models;
using System;
using Xamarin.Essentials;

namespace Swegrant.Helpers
{
    public static class Settings
    {
        
        public static string AppCenterAndroid = "AC_ANDROID";

#if DEBUG
        //static readonly string defaultIP = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        static readonly string defaultIP = "192.168.1.10";
#else
                static readonly string defaultIP = "192.168.0.10";
#endif

        static readonly string defaultPort = "5000";

        static readonly string defaultLanguage = Language.None.ToString();

        static readonly string defaultCharachter = Character.None.ToString();

        static readonly string questionnaire = "";

        public static bool UseHttps
        {
            get => false;
            //get => (defaultIP != "localhost" && defaultIP != "10.0.2.2");
        }

        public static string ServerIP
        {
            get => Preferences.Get(nameof(ServerIP), defaultIP);
            set => Preferences.Set(nameof(ServerIP), value);
        }

        public static string ServerPort
        {
            //get
            //{
            //    return "5000";
            //}
            get => Preferences.Get(nameof(ServerPort), defaultPort);
            set => Preferences.Set(nameof(ServerPort), value);
        }

        static Random random = new Random();
        static readonly string defaultName = $"{DeviceInfo.Platform} User {random.Next(1, 100)}";
        public static string UserName
        {
            get => Preferences.Get(nameof(UserName), defaultName);
            //get => "User1";
            set => Preferences.Set(nameof(UserName), value);
        }

        
        public static string Group
        {
            //get => Preferences.Get(nameof(Group), string.Empty);
            get => Swegrant.Shared.Models.ChatSettings.ChatGroup;

            set => Preferences.Set(nameof(Group), value);
        }

        public static Character CurrentCharachter
        {

            get
            {
                string val = Preferences.Get(nameof(CurrentCharachter), defaultCharachter);
                return (Character) Enum.Parse(typeof(Character), val);
            }

            set => Preferences.Set(nameof(CurrentCharachter), value.ToString());
        }

        public static Language CurrentLanguage
        {

            get
            {
                string val = Preferences.Get(nameof(CurrentLanguage), defaultLanguage);
                return (Language)Enum.Parse(typeof(Language), val);
            }

            set => Preferences.Set(nameof(CurrentLanguage), value.ToString());
        }

        public static Language CurrentAudioLanguage
        {

            get
            {
                string val = Preferences.Get(nameof(CurrentAudioLanguage), defaultLanguage);
                return (Language)Enum.Parse(typeof(Language), val);
            }

            set => Preferences.Set(nameof(CurrentAudioLanguage), value.ToString());
        }

        private static string mediaInfo = "";

        public static string MediaInfo
        {
            get => Preferences.Get(nameof(ServerIP), defaultIP);
            set => Preferences.Set(nameof(ServerIP), value);
        }


        public static Questionnaire Questionnaire
        {
            get
            {
                if (!string.IsNullOrEmpty(QuestionnaireJson))
                {
                    return JsonConvert.DeserializeObject<Questionnaire>(QuestionnaireJson);
                }
                return new Questionnaire();
            }
            set
            {
                if (value != null)
                {
                    QuestionnaireJson = JsonConvert.SerializeObject(value);
                }

            }
        }

        private static string QuestionnaireJson
        {
            get => Preferences.Get(nameof(Questionnaire), questionnaire);
            set => Preferences.Set(nameof(Questionnaire), value);
        }

    }
}
