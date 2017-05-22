using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class RemoveClassStudent
    {
        [JsonProperty(PropertyName = "Removed")]
        public bool Removed { get; set; }

        [JsonProperty(PropertyName = "WeeklyMsg")]
        public string WeeklyMsg { get; set; }

        [JsonProperty(PropertyName = "SpareMsg")]
        public string SpareMsg { get; set; }
    }
}