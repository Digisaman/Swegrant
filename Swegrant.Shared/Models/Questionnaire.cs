using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Shared.Models
{
    public class Questionnaire
    {

        public Question[] Questions { get; set; }

        public Comment Comment { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string TitleFA { get; set; }

        public string TileSV { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string TitleFA { get; set; }

        public string TileSV { get; set; }

        public Answer[] Answers { get; set; }

        //public int AnswerId { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string ValueFA { get; set; }

        public string ValueSV { get; set; }

    }
}
