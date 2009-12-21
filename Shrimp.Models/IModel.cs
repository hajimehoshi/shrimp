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
    public interface IModel
    {
        void Clear();

        event EventHandler<UpdatedEventArgs> Updated;

        JToken ToJson();
        void LoadJson(JToken json);
    }

    public class UpdatedEventArgs : EventArgs
    {
        public UpdatedEventArgs()
            : this(null)
        {
        }

        public UpdatedEventArgs(string property)
        {
            this.Property = property;
        }

        public UpdatedEventArgs(string property, UpdatedEventArgs innerUpdatedEventArgs)
        {
            this.Property = property;
            this.InnerUpdatedEventArgs = innerUpdatedEventArgs;
        }

        // TODO: Remove this method
        public UpdatedEventArgs(string property, object bounds)
        {
            this.Property = property;
            this.Bounds = bounds;
        }

        public string Property { get; private set; }
        public object Bounds { get; private set; }
        public UpdatedEventArgs InnerUpdatedEventArgs { get; private set; }
    }

    public static class IModelExtensions
    {
        public static string GetProperty<T, TResult>(this T obj, Expression<Func<T, TResult>> expr) where T : IModel
        {
            return ((MemberExpression)(expr.Body)).Member.Name;
        }
    }
}
