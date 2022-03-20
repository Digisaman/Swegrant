using Newtonsoft.Json;
using Swegrant.Helpers;
using Swegrant.Models;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swegrant.ViewModels
{
    public class QuestionnaireViewModel : BaseViewModel
    {  

        #region Properties
        private ObservableQuestion currentQuestion = null;
        public ObservableQuestion CurrentQuestion 
        {
            get { return this.currentQuestion; }
            set { SetProperty(ref this.currentQuestion, value); }
        }

        public int CurentIndex { get; set; }
        #endregion
        #region Commands
        public MvvmHelpers.Commands.Command LoadQuestionsCommand { get; }

        public MvvmHelpers.Commands.Command SubmitQuestionCommand { get; }
        #endregion

        public QuestionnaireViewModel()
        {
            Title = Resources.Questionnaire.Title;
            LoadQuestionsCommand = new MvvmHelpers.Commands.Command(async () => await LoadQuestions());
            SubmitQuestionCommand = new MvvmHelpers.Commands.Command(async () => await SubmitQuestion());
        }

        private bool CanExecuteSubmitQuestion()
        {
            bool result =CurrentQuestion.Answers.Any(c => c.IsSelected);
            return result;
            //return CurrentQuestion.IsAnswerSelected;
        }

        private async Task SubmitQuestion()
        {
            try
            {
                ObservableAnswer answer = CurrentQuestion.Answers.FirstOrDefault(c => c.IsSelected);
                if (answer == null)
                {
                    return;

                }

                SubmitQuestion submitQuestion = new SubmitQuestion
                {
                    Title = CurrentQuestion.Title,
                    Value = answer.Value,
                    Type = QuestionType.MultiAnswer,
                    Username = Helpers.Settings.UserName
                };
                bool result = await ServerHelper.SubmitQuestion(submitQuestion);
                if (result)
                {
                    if ( CurentIndex < Helpers.Settings.Questionnaire.Questions.Length - 1)
                    {
                        CurentIndex++;
                        await LoadQuestions();
                    }
                    else
                    {
                        //await DialogService.DisplayAlert("Information", "Thank you for takeing the time to fill out the questionnaire.", "");
                        ServerHelper.SubmitStatus(UserEvent.QuestionnaireCompleted, "");
                        await App.Current.MainPage.DisplayAlert("Information", "Thank you for takeing the time to fill out the questionnaire.","Ok", "Cancel");
                        CurrentQuestion = new ObservableQuestion();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task LoadQuestions()
        {
            Question question = Helpers.Settings.Questionnaire.Questions[CurentIndex];
            this.CurrentQuestion = new ObservableQuestion
            {
                Id = question.Id,
                Title = question.Title
            };
            foreach (var item in question.Answers)
            {
                this.CurrentQuestion.Answers.Add(new ObservableAnswer
                {
                    Id = item.Id,
                    Value = item.Value,
                });
            }

        }
    }
}
