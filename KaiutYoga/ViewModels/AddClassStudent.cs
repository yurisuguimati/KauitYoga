using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AddClassStudent
    {
        [JsonProperty(PropertyName = "HavePresenceList")]
        public bool HavePresenceList { get; set; }

        [JsonProperty(PropertyName = "WeeklyAdded")]
        public bool WeeklyAdded { get; set; }

        [JsonProperty(PropertyName = "WeeklyMsg")]
        public string WeeklyMsg { get; set; }

        [JsonProperty(PropertyName = "SpareAdded")]
        public bool SpareAdded { get; set; }

        [JsonProperty(PropertyName = "SpareMsg")]
        public string SpareMsg { get; set; }
        
    }
}