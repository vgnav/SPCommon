using System;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPCommon.Tests.IntegrationTests
{
    [TestClass]
    public class MiscTests
    {
        private const string ListName = "CTListTest";
        private const string ListUrl = "http://spdev/Lists/CTTestList";
        private const string TestCTOne = "TestCTOne";
        private const string ItemCT = "Item";

        [TestMethod]
        public void Misc_GetContentTypeOfListItem()
        {
            using (var site = new SPSite(ListUrl))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists.TryGetList(ListName);
                    var items = list.GetItems();
                    foreach (SPListItem item in items)
                    {
                        var contentType = item.ContentType.Name;
                        Assert.IsTrue(contentType.Equals(TestCTOne) || contentType.Equals(ItemCT));
                    }
                }
            }
        }

        #region Helpers

        #endregion
    }
}
