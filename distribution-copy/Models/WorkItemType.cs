using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace distribution_copy.Models
{
    public partial class WorkItemType
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public Value[] Value { get; set; }
    }

    public partial class Values
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("icon")]
        public Icon Icon { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("xmlForm")]
        public string XmlForm { get; set; }

        [JsonProperty("fields")]
        public Field[] Fields { get; set; }

        [JsonProperty("fieldInstances")]
        public Field[] FieldInstances { get; set; }

        [JsonProperty("transitions")]
        public Dictionary<string, Transition[]> Transitions { get; set; }

        [JsonProperty("states")]
        public State[] States { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Field
    {
        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("alwaysRequired")]
        public bool AlwaysRequired { get; set; }

        [JsonProperty("referenceName")]
        public string ReferenceName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("helpText", NullValueHandling = NullValueHandling.Ignore)]
        public string HelpText { get; set; }
    }

    public partial class Icon
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class State
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }

    public partial class Transition
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("actions")]
        public string[] Actions { get; set; }
    }
}