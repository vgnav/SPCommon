using System.Collections.Generic;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCommon.CAML;
using SPCommon.Entity;
using SPCommon.Infrastructure.Common;
using SPCommon.Infrastructure.Factory;
using SPCommon.Infrastructure.Repository;
using SPCommon.Interface;

namespace SPCommon.Tests.IntegrationTests
{
    [TestClass]
    public class ListRepositoryTests
    {
        private const string ListName = "Test";
        private const string ListUrl = "http://spdev/lists/Test";
        private readonly IListRepository<TestEntity> _listRepository;
        private readonly SPWeb _web;
        private readonly SPSite _site;
        
        public ListRepositoryTests()
        {
            _site = new SPSite(ListUrl);
            _web = _site.OpenWeb();
            _listRepository = (new ClientFactory(_web, ListName)).CreateListRepository<TestEntity>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _web.Dispose();
            _site.Dispose();
        }

        #region Test Methods

        [TestMethod]
        public void ListRepository_GetAllItems()
        {
            var items = _listRepository.FindAll();
            var initialCount = items.Count;
            var testEntity = GetTestEntity();
            _listRepository.Create(testEntity);
            Assert.IsTrue(_listRepository.FindAll().Count == (initialCount + 1));
            Assert.IsTrue(_listRepository.Delete(testEntity));
            Assert.IsTrue(_listRepository.FindAll().Count == initialCount);
        }

        [TestMethod]
        public void ListRepository_AddItem()
        {
            var items = _listRepository.FindAll();
            var initialCount = items.Count;
            var newItem = GetTestEntity();
            Assert.IsTrue(_listRepository.Create(newItem));
            Assert.IsTrue(_listRepository.FindAll().Count == (initialCount + 1));
            var createdItem = _listRepository.Read(newItem.Id);
            Assert.IsTrue(createdItem.Id == newItem.Id);
            Assert.IsTrue(_listRepository.Delete(createdItem));
        }

        [TestMethod]
        public void ListRepository_UpdateItem()
        {
            var items = _listRepository.FindAll();
            var initialCount = items.Count;
            var newItem = GetTestEntity();
            Assert.IsTrue(_listRepository.Create(newItem));
            Assert.IsTrue(_listRepository.FindAll().Count == (initialCount + 1));
            newItem.Title = "Test Item Updated";
            Assert.IsTrue(_listRepository.Update(newItem));
            var updatedItem = _listRepository.Read(newItem.Id);
            Assert.IsTrue(updatedItem.Id == newItem.Id);
            Assert.IsTrue(updatedItem.Title == newItem.Title);
            Assert.IsTrue(_listRepository.Delete(updatedItem));
        }

        [TestMethod]
        public void ListRepository_DeleteItem()
        {
            var items = _listRepository.FindAll();
            var initialCount = items.Count;

            var newItem = new TestEntity
            {
                Title = "Test For Delete"
            };
            var success = _listRepository.Create(newItem);
            Assert.IsTrue(success);
            Assert.IsTrue(_listRepository.FindAll().Count == (initialCount + 1));

            success = _listRepository.Delete(newItem);
            Assert.IsTrue(success);
            Assert.IsTrue(_listRepository.FindAll().Count == initialCount);
        }

        [TestMethod]
        public void ListRepository_FindItemsByQuery()
        {
            ResetList();

            var item1 = GetTestEntity();
            var item2 = GetTestEntity();
            var item3 = GetTestEntity();
            var item4 = GetTestEntity();
            var item5 = GetTestEntity();

            _listRepository.Create(item1);
            _listRepository.Create(item2);
            _listRepository.Create(item3);
            _listRepository.Create(item4);
            _listRepository.Create(item5);

            var spquery = new SPQuery
            {
                Query = "<Where><Eq><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + item1.Title +"</Value></Eq></Where>"
            };

            var items = _listRepository.FindByQuery(spquery);
            var count = items.Count;

            Assert.IsTrue(count == 5);

            ResetList();
        }

        [TestMethod]
        public void ListRepository_FindItemsByCAML_SingleExpression()
        {
            ResetList();

            var item1 = GetTestEntity();
            var item2 = GetTestEntity();
            var item3 = GetTestEntity();
            var item4 = GetTestEntity();
            var item5 = GetTestEntity();

            _listRepository.Create(item1);
            _listRepository.Create(item2);
            _listRepository.Create(item3);
            _listRepository.Create(item4);
            _listRepository.Create(item5);

            var caml = new CAMLExpression
            {
                Column = "Title",
                Operator = CAMLOperator.Eq,
                Type = "Text",
                Value = item1.Title
            };

            var items = _listRepository.FindByCAML(caml);
            var count = items.Count;

            Assert.IsTrue(count == 5);

            ResetList();
        }

        #endregion

        #region helpers

        private static TestEntity GetTestEntity()
        {
            return new TestEntity { Title = "GetItemTest" };
        }

        // Delete everything
        private static void ResetList()
        {
            using (var site = new SPSite(ListUrl))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists.TryGetList(ListName);
                    var items = list.Items;
                    foreach (SPListItem item in items)
                    {
                        list.Items.DeleteItemById(item.ID);
                    }
                }
            }
        }

        #endregion
    }

    public class TestEntity : BaseItem
    {
        public string TextColumn { get; set; }
        public bool YesNoColumn { get; set; }
    }

    public class TestRepository : GenericListRepository<TestEntity>
    {
        public TestRepository(SPWeb web)
            : base(web, "Test")
        {}

        public override void MapEntityItemToSPListItem(TestEntity item, SPListItem spListItem)
        {
            base.MapEntityItemToSPListItem(item, spListItem);
            spListItem["YesNoColumn"] = item.YesNoColumn ? "1" : "0";
        }
    }

    public class ClientFactory : RepositoryFactory
    {
        public ClientFactory(SPWeb web, string listName) : base(web, listName)
        {}

        protected override Dictionary<string, IRepository<T>> ProvideRepositories<T>()
        {
            var repositories = base.ProvideRepositories<T>();
            repositories.Add("Test", new TestRepository(Web) as IRepository<T>);
            return repositories;
        }
    }

    public class TestWSRepo : GenericListWSRepository<TestEntity>
    {
        public TestWSRepo(ICredentials credentials, string url, string listName) : base(credentials, url, listName)
        {
        }

        protected override Dictionary<string, string> ProvideItemFieldMap(TestEntity item)
        {
            var map = base.ProvideItemFieldMap(item);
            return map;
        }
    }

}
