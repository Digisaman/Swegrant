using Newtonsoft.Json;
using Swegrant.Helpers;
using Swegrant.Models;
using Swegrant.Resources;
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

        private string commentValue = null;
        public string CommentValue
        {
            get { return this.commentValue; }
            set { SetProperty(ref this.commentValue, value); }
        }


        private bool isQuestionVisibile = false;
        public bool IsQuestionVisibile
        {
            get { return this.isQuestionVisibile; }
            set { SetProperty(ref this.isQuestionVisibile, value); }
        }


        private bool isCommentVisibile = false;
        public bool IsCommentVisibile
        {
            get { return this.isCommentVisibile; }
            set { SetProperty(ref this.isCommentVisibile, value); }
        }

        public int CurentIndex { get; set; }
        #endregion
        #region Commands
        public MvvmHelpers.Commands.Command LoadQuestionsCommand { get; }

        public MvvmHelpers.Commands.Command SubmitQuestionCommand { get; }
        #endregion

        public QuestionnaireViewModel()
        {
            Title = Resources.MenuTitles.Questionnaire;
            IsQuestionVisibile = true;
            IsCommentVisibile = false;
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
                if (CurrentQuestion.Id != 0)
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
                        if (CurentIndex < Helpers.Settings.Questionnaire.Questions.Length - 1)
                        {
                            CurentIndex++;
                            await LoadQuestions();
                        }
                        else
                        {
                            //await DialogService.DisplayAlert("Information", "Thank you for takeing the time to fill out the questionnaire.", "");



                            //await App.Current.MainPage.DisplayAlert(AppResources.Information, 
                            //    AppResources.QuestionnaireFinalMessage,
                            //    AppResources.OK, 
                            //    AppResources.Cancel);
                            //CurrentQuestion = new ObservableQuestion();
                            ServerHelper.SubmitStatus(UserEvent.QuestionnaireCompleted, "");

                            LoadComment();
                        }
                    }


                }
                else
                {
                    SubmitQuestion submitQuestion = new SubmitQuestion
                    {
                        Title = CurrentQuestion.Title,
                        Value = CommentValue,
                        Type = QuestionType.Comment,
                        Username = Helpers.Settings.UserName
                    };

                    bool result = await ServerHelper.SubmitQuestion(submitQuestion);
                    if (result)
                    {

                        await App.Current.MainPage.DisplayAlert(AppResources.Information,
                                  AppResources.QuestionnaireFinalMessage,
                                  AppResources.OK,
                                  AppResources.Cancel);
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
                Title = ( Helpers.Settings.CurrentLanguage == Language.English ? 
                     question.Title : (Helpers.Settings.CurrentLanguage == Language.Svenska 
                     ? question.TitleSV  : 
                     question.TitleFA))
            };
            foreach (var item in question.Answers)
            {
                this.CurrentQuestion.Answers.Add(new ObservableAnswer
                {
                    Id = item.Id,
                    Value = (Helpers.Settings.CurrentLanguage == Language.English ?
                     item.Value : (Helpers.Settings.CurrentLanguage == Language.Svenska
                     ? item.ValueSV :
                     item.ValueFA))
                });
            }

        }

        private async Task LoadComment()
        {
            IsQuestionVisibile = false;
            IsCommentVisibile = true;
            Comment comment= Helpers.Settings.Questionnaire.Comment;
            this.CurrentQuestion = new ObservableQuestion
            {
                Id = 0,
                Title = (Helpers.Settings.CurrentLanguage == Language.English ?
                     comment.Title : (Helpers.Settings.CurrentLanguage == Language.Svenska
                     ? comment.TitleSV :
                     comment.TitleFA))
            };
        }
    }
}
