using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Swegrant.Models
{
    public class ObservableQuestion : ObservableObject
    {
        private int id;

        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private string title = "";

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ObservableCollection<ObservableAnswer> Answers { get; set; }

        public ObservableQuestion()
        {
            Answers = new ObservableCollection<ObservableAnswer>(); 
        }

        public bool IsAnswerSelected
        {
            get
            {
                return Answers.Any(c => c.IsSelected);
            }
        }

    }

    public class ObservableAnswer : ObservableObject
    {
        private int id;

        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private string value = "";

        public string Value
        {
            get => value;
            set => base.SetProperty(ref this.value, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
    }
}
