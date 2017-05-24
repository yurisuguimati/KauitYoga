using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaiutYoga.ViewModels
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ClassJson
    {
        [JsonProperty(PropertyName = "ClassId")]
        public long ClassId { get; set; }

        [JsonProperty(PropertyName = "Start")]
        public DateTime Start {get; set;}

        [JsonProperty(PropertyName = "Weekly")]
        public long AmountWeeklyStudents { get; set; }

        [JsonProperty(PropertyName = "Trial")]
        public long AmountTrialStudents { get; set; }

        [JsonProperty(PropertyName = "Replacement")]
        public long AmountReplacementStudents { get; set; }

        [JsonProperty(PropertyName = "Index")]
        public long Index { get; set; }
    }
}