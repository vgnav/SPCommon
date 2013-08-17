using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCommon.Entity;
using SPCommon.Serializers;

namespace SPCommon.Tests.UnitTests
{
    [TestClass]
    public class JSONTests
    {
        [TestMethod]
        public void JSON_SerializeObjectToJSON()
        {
            var baseItem = new BaseItem
            {
                Author = "Navid"
            };
            var json = baseItem.ToJSON();
            Assert.IsTrue(json.ToString().Contains("Navid"));
        }

        [TestMethod]
        public void JSON_BuildJSONTree()
        {
            var jsonList = new JSONList
            {
                new BaseItem {Author = "Navid"},
                new BaseItem {Author = "Preety"},
                new BaseItem {Author = "One"},
                new BaseItem {Author = "Two"}
            };

            var list = new List<BaseItem>
            {
                new BaseItem {Author = "Navid"},
                new BaseItem {Author = "Preety"},
            };

            var jsonList2 = JSONList.ToJSONList(list);
            var jsonString2 = jsonList2.ToJSONString();
            Assert.IsTrue(jsonString2.Contains("Navid"));
            var jsonString = jsonList.ToJSONString();
            Assert.IsTrue(jsonString.Contains("Navid"));
        }
    }
}
