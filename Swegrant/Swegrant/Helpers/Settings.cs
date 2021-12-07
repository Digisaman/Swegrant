﻿using System;
using Xamarin.Essentials;

namespace Swegrant.Helpers
{
    public static class Settings
    {
        public static string AppCenterAndroid = "AC_ANDROID";

#if DEBUG
        static readonly string defaultIP = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
#else
                static readonly string defaultIP = "Swegrantr.azurewebsites.net";
#endif



        //static readonly string defaultIP = "localhost";



        public static bool UseHttps
        {
            get => (defaultIP != "localhost" && defaultIP != "10.0.2.2");
        }

        public static string ServerIP
        {
            get => Preferences.Get(nameof(ServerIP), defaultIP);
            set => Preferences.Set(nameof(ServerIP), value);
        }

        static Random random = new Random();
        static readonly string defaultName = $"{DeviceInfo.Platform} User {random.Next(1, 100)}";
        public static string UserName
        {
            //get => Preferences.Get(nameof(UserName), defaultName);
            get => "User1";
            set => Preferences.Set(nameof(UserName), value);
        }

        
        public static string Group
        {
            //get => Preferences.Get(nameof(Group), string.Empty);
            get => "Xamarin";

            set => Preferences.Set(nameof(Group), value);
        }
    }
}
