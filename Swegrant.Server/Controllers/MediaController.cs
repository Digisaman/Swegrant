using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Swegrant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
      
        [HttpGet]
        [Route(nameof(GetCurrentTime))]
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        [HttpGet]
        [Route(nameof(GetMediaInfo))]
        public MediaInfo GetMediaInfo()
        {
            DirectoryInfo mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA");

            FileInfo[] files = mediaDirectory.GetFiles("*.*", SearchOption.AllDirectories);

            MediaInfo mediaInfo = null;
            string mediaInfoFilePath = $"{mediaDirectory}\\MediaInfo.json";
            using (StreamReader streamReader = new StreamReader(mediaInfoFilePath))
            {
                string content = streamReader.ReadToEnd();
                mediaInfo = JsonConvert.DeserializeObject<MediaInfo>(content);
            }

            string localIP = Helpers.AppConfigHelpers.LoadConfig("ServerIP").ToString();
            string port = ChatSettings.DefaultPort;
            string protocol = ChatSettings.DefaultProtocol;

            List<string> urls = new List<string>();
            foreach (MediaFile mediafile in mediaInfo.AUDIO)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/AUDIO/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            foreach (MediaFile mediafile in mediaInfo.THSUB)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/THSUB/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            foreach (MediaFile mediafile in mediaInfo.VDSUB)
            {
                FileInfo file = files.FirstOrDefault(c => c.Name == mediafile.FileName);
                if (file != null)
                {
                    mediafile.Url = $"{protocol}://{localIP}:{port}/MEDIA/VDSUB/{file.Name}";
                    mediafile.IsAvailable = true;
                    mediafile.FileSize = file.Length;
                }
            }

            mediaInfo.AUDIO = mediaInfo.AUDIO.Where(c => c.IsAvailable).ToList();
            mediaInfo.THSUB = mediaInfo.THSUB.Where(c => c.IsAvailable).ToList();
            mediaInfo.VDSUB = mediaInfo.VDSUB.Where(c => c.IsAvailable).ToList();


            return mediaInfo;



        }

        [HttpGet]
        [Route(nameof(GetQuestionnaire))]
        public Questionnaire GetQuestionnaire()
        {
            try
            {
                DirectoryInfo mediaDirectory = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\wwwroot\\MEDIA");

                Questionnaire questionnaire = null;
                string questionnaireFilePath = $"{mediaDirectory}\\Questionnaire.json";
                using (StreamReader streamReader = new StreamReader(questionnaireFilePath))
                {
                    string content = streamReader.ReadToEnd();
                    questionnaire = JsonConvert.DeserializeObject<Questionnaire>(content);
                }

                return questionnaire;
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        [HttpPost]
        [Route(nameof(SubmitQuestion))]
        public void SubmitQuestion([FromBody] SubmitQuestion question)
        {
            try
            {
                //question.Id = Questions.Count + 1;
                //if (Questionnaire == null )
                //{
                //    Questionnaire = GetQuestionnaire();
                //}
                //Question selectedQuestion = Questionnaire.Questions.FirstOrDefault(c => c.Id == question.Id);
                //if (selectedQuestion != null)
                //{
                //    question.Title = selectedQuestion.Title;
                //    question.Value = (question.Type == QuestionType.MultiAnswer 
                //        ? selectedQuestion.Answers.FirstOrDefault(c => c.Id == question.AnswerId).Value 
                //        : question.CommentValue);
                //}
               MainWindow.Singleton.AddQuestion(question);
            }
            catch (Exception ex)
            {

            }
        }

        [HttpPost]
        [Route(nameof(SubmitUserStatus))]
        public void SubmitUserStatus([FromBody] SubmitUserStatus userStatus)
        {
            try
            {
                //userStatus.Id = UserStatuses.Count + 1;
                userStatus.Time = DateTime.Now;
                //UserStatuses.Add(userStatus);
                
                MainWindow.Singleton.AddUserStatus(userStatus);
            }
            catch (Exception ex)
            {

            }
        }

        [HttpGet]
        [Route(nameof(AutoAssignCharacter))]
        public Character AutoAssignCharacter()
        {
            try
            {

                return MainWindow.Singleton.AutoAssignCharacter();
            }
            catch (Exception ex)
            {
                return Character.Tara;
            }
        }


    }
}
