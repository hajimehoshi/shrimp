using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public interface IIndexable<T> : IEnumerable<T>
    {
        T this[int index] { get; set; }
        int Length { get; }
    }

    public class ObservedArray<T> : Model, IIndexable<T> where T : IComparable
    {
        public ObservedArray(int size)
        {
            this.Items = new T[size];
        }

        private T[] Items;

        public T this[int index]
        {
            get
            {
                return this.Items[index];
            }
            set
            {
                if (this.Items[index].CompareTo(value) != 0)
                {
                    this.Items[index] = value;
                    string property = "Index" + index.ToString();
                    this.OnUpdated(new UpdatedEventArgs(property));
                }
            }
        }

        public int Length
        {
            get { return this.Items.Length; }
        }

        public override void Clear()
        {
            for (int i = 0; i < this.Items.Length; i++)
            {
                this.Items[i] = default(T);
            }
            this.OnUpdated(new UpdatedEventArgs());
        }

        public override JToken ToJson()
        {
            Type type = typeof(T);
            if (type.IsValueType)
            {
                return new JArray(this.Items.Cast<int>().ToArray());
            }
            else
            {
                return new JArray(this.Items.Cast<string>().ToArray());
            }
        }

        public override void LoadJson(JToken json)
        {
            T[] newItems = json.Values<int>().Cast<T>().ToArray();
            for (int i = 0; i < this.Items.Length; i++)
            {
                this.Items[i] = newItems[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
    }
}
