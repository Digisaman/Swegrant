using Newtonsoft.Json;
using System;

namespace Swegrant.Shared.Models
{
    public class SubmitUserStatus
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime Time { get; set; }
        public UserEvent Event { get; set; }

        public string Value { get; set; }

        public string Username { get; set; }
    }
}
