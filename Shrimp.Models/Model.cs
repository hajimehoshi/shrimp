using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public abstract class Model : IModel
    {
        public abstract void Clear();

        public event EventHandler<UpdatedEventArgs> Updated;
        protected virtual void OnUpdated(UpdatedEventArgs e)
        {
            if (this.Updated != null) { this.Updated(this, e); }
        }

        public abstract JToken ToJson();

        public abstract void LoadJson(JToken json);
    }
}
