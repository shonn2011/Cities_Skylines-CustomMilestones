using System;
using System.Collections.Generic;

namespace CustomMilestones.Models
{
    [Serializable]
    public class LanguageModel
    {
        public string UniqueName { get; set; }

        public string ReadableName { get; set; }

        public Dictionary<string, string> KeyValuePairs { get; set; }
    }
}
