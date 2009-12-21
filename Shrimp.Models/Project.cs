using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public class Project : Model
    {
        public Project()
        {
            this.Clear();
        }

        public string GameTitle
        {
            get { return this.gameTitle; }
            set
            {
                if (this.gameTitle != value)
                {
                    this.gameTitle = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.GameTitle)));
                }
            }
        }
        private string gameTitle;

        public override void Clear()
        {
            this.GameTitle = "";
        }

        public override JToken ToJson()
        {
            return new JObject(new JProperty("GameTitle", this.GameTitle));
        }

        public override void LoadJson(JToken json)
        {
            this.Clear();
            this.GameTitle = json.Value<string>("GameTitle");
        }
    }
}
