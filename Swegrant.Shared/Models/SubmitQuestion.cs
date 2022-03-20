using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Shared.Models
{
    public class SubmitQuestion
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public string Title { get; set; }

        [JsonIgnore]
        public string Value { get; set; }

        public string CommentValue { get; set; }

        public int AnswerId { get; set; }

        public QuestionType Type { get; set; }
        public string Username { get; set; }

        
    }
}
