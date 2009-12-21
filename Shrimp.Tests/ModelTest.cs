using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shrimp;
using Shrimp.Models;

namespace Shrimp.Tests
{
    [TestFixture]
    public class ModelTest
    {
        private class FooModel : Model
        {
            public FooModel(string foo)
            {
                this.Foo = foo;
            }

            public override void Clear()
            {
                throw new NotImplementedException();
            }

            public override JToken ToJson()
            {
                throw new NotImplementedException();
            }

            public override void LoadJson(JToken json)
            {
                throw new NotImplementedException();
            }

            public object Foo { get; private set; }
        }

        [Test]
        public void TestGetProperty()
        {
            FooModel model1 = new FooModel("foo1");
            FooModel model2 = new FooModel("foo2");
            Assert.IsTrue(model1.GetProperty(_ => _.Foo) == model2.GetProperty(_ => _.Foo));
        }
    }
}
