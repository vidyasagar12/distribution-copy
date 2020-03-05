using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace distribution_copy.Models
{
    public class Fields
    {
        [JsonProperty(PropertyName = "System.TeamProject")]
        public string TeamProject { get; set; }


        [JsonProperty(PropertyName = "System.WorkItemType")]
        public string WorkItemType { get; set; }


        [JsonProperty(PropertyName = "System.State")]
        public string State { get; set; }


        [JsonProperty(PropertyName = "Custom.PlannedHours")]
        public float PlannedHours { get; set; }


        [JsonProperty(PropertyName = "Custom.ActualHours")]
        public float ActualHours { get; set; }


        [JsonProperty(PropertyName = "System.IterationPath")]
        public string Sprint { get; set; }


        [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.OriginalEstimate")]
        public float OriginalEstimate { get; set; }


        [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.CompletedWork")]
        public float CompletedWork { get; set; }


        [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.RemainingWork")]
        public float RemainingWork { get; set; }


        [JsonProperty(PropertyName = "System.Title")]
        public string Title { get; set; }


        [JsonProperty(PropertyName = "System.CreatedDate")]
        public DateTime CreatedDate { get; set; }


        [JsonProperty(PropertyName = "System.Description")]
        public string Description { get; set; }


        [JsonProperty(PropertyName = "System.CreatedBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty(PropertyName = "System.AssignedTo")]
        public CreatedBy AssignedTo { get; set; }


        [JsonProperty(PropertyName = "System.ChangedBy")]
        public CreatedBy ChangedBy { get; set; }


    }
    public class CreatedBy
    {
        public string displayName { get; set; }
        public string uniqueName { get; set; }
    }
    public class Value
    {
        public int id { get; set; }
        public string url { get; set; }
        public Fields fields { get; set; }
    }
    public class ResponseWI
    {
        public int count { get; set; }
        public List<Value> value { get; set; }
        public IList<Value> workItems { get; set; }

    }
}