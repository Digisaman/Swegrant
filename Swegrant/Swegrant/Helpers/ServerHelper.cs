using Newtonsoft.Json;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swegrant.Helpers
{
    public class ServerHelper
    {
        #region Media Controller
        public static void SubmitStatus(UserEvent userEvent, string value)
        {
            SubmitStatusAsync(userEvent, value);
            return;
        }
        public async static Task<bool> SubmitStatusAsync(UserEvent userEvent, string value)
        {
            try
            {
                SubmitUserStatus submitStatus = new SubmitUserStatus
                {
                    Event = userEvent,
                    Value = value,
                    Username = Helpers.Settings.UserName
                };
                Uri uri = GetApiUri("submituserstatus");
                var json = JsonConvert.SerializeObject(submitStatus);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient httpClient = new HttpClient();
                HttpClient client = httpClient;
                HttpResponseMessage response = await client.PostAsync(uri, data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<bool> SubmitQuestion(SubmitQuestion submitQuestion)
        {
            Uri uri = GetApiUri("submitquestion");
            var json = JsonConvert.SerializeObject(submitQuestion);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, data);
            return response.IsSuccessStatusCode;
        }

        public async static Task<MediaInfo> GetMediaInfo()
        {
            MediaInfo info = null;
            try
            {
                Uri uri = ServerHelper.GetApiUri("GetMediaInfo");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    info = JsonConvert.DeserializeObject<MediaInfo>(content);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return info;
        }

        public async static Task<Questionnaire> GetQuestionnaire()
        {
            Questionnaire info = null;
            try
            {
                Uri uri = ServerHelper.GetApiUri("GetQuestionnaire");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    info = JsonConvert.DeserializeObject<Questionnaire>(content);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return info;
        }

        
        public async static Task<Character> AutoAssignCharacterAsync()
        {
            Character character = Character.None; ;
            try
            {
                Uri uri = ServerHelper.GetApiUri("AutoAssignCharacter");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    character = JsonConvert.DeserializeObject<Character>(content);
                }
            }
            catch (Exception ex)
            {
                return Character.None;
            }
            return character;
        }

        private static Uri GetApiUri(string method, string controller = "media")
        {
            Uri uri = new Uri($"{(Swegrant.Helpers.Settings.UseHttps ? "https" : "http")}://{Swegrant.Helpers.Settings.ServerIP}:{Swegrant.Helpers.Settings.ServerPort}/api/{controller}/{method}");
            return uri;
        }

        #endregion

        #region Subtitle Controller
        public async static Task<bool> HideSubtitle()
        {
            try
            {
                Uri uri = ServerHelper.GetApiUri("HideSubtitle", "subtitle");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<bool> ShowSubtitle()
        {
            try
            {
                Uri uri = ServerHelper.GetApiUri("ShowSubtitle", "subtitle");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<bool> ResumeAutoSub()
        {
            try
            {
                Uri uri = ServerHelper.GetApiUri("ResumeAutoSub", "subtitle");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<bool> PauseAutoSub()
        {
            try
            {
                Uri uri = ServerHelper.GetApiUri("PauseAutoSub", "subtitle");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async static Task<bool> NextMaunualSub()
        {
            try
            {
                Uri uri = ServerHelper.GetApiUri("NextMaunualSub", "subtitle");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion
    }
}
