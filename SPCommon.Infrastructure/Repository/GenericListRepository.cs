using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint;
using SPCommon.CAML;
using SPCommon.CustomException;
using SPCommon.Entity;
using SPCommon.Infrastructure.Common;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    /// <summary>
    /// GenericListRepository needs to be used with a class whose type has been derived from SPCommon.Entity.BaseListItem
    /// ListRepositoy is designed to work with dependency injection
    /// Initialise/construct with your SPWeb value (usually coming from SPContext.Current.Web)
    /// Key is compulsory (otherwise it won't work, obviously)
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericListRepository<T> : IListRepository<T> where T : BaseItem, new()
    {
        #region Constructors, method-based initialisers and private/protected variables

        protected string ListName;
        protected SPWeb Web;

        public GenericListRepository(SPWeb web, string listName)
        {
            Web = web;
            ListName = listName;
        }


        #endregion

        #region Interface methods

        public bool Create(T t)
        {
            return CreateItem(Web, t);
        }

        public T Read(int id)
        {
            return GetSingleItem(Web, id);
        }

        public bool Update(T t)
        {
            return UpdateItem(Web, t);
        }

        public bool Delete(T t)
        {
            return DeleteItem(Web, t);
        }

        public IList<T> FindAll()
        {
            return FindByQuery(null);
        }

        public IList<T> FindByQuery(object query)
        {
            var returnedList = GetAllItems(Web, query as SPQuery);
            return returnedList;
        }

        public IList<T> FindByCAML(ICAMLExpression camlExpression)
        {
            var query = new SPQuery
            {
                Query = (new CAMLBuilder(camlExpression)).ToString()
            };
            return FindByQuery(query);
        }

        #endregion

        #region Overridable methods for extending the Repository

        /// <summary>
        /// Maps a class derived from SPCommon.Entity.BaseListItem to a corresponding SPListItem object
        /// Extending code needs to implement the rest of 'MapEntityItemToSPListItem' so item gets mapped properly
        /// TODO: can possiby use Reflection to fill in more values than just Title
        /// </summary>
        /// <param name="item"></param>
        /// <param name="spListItem"></param>
        public virtual void MapEntityItemToSPListItem(T item, SPListItem spListItem)
        {
            spListItem["Title"] = item.Title;
        }

        /// <summary>
        /// TODO: turn this into reflection based property mapper
        /// Maps an SPListItem into an item derived from base cass SPCommon.Entity.BaseListItem
        /// Method can be over-ridden to provide custom functionality 
        /// </summary>
        /// <param name="spItem"></param>
        /// <returns></returns>
        public virtual T MapSPListItemToEntityItem(SPListItem spItem)
        {
            var itemMapper = new SharePointItemMapper<T>();
            var t = itemMapper.BuildEntityFromItem(spItem);
            return t;
        }

        protected virtual bool CreateItem(SPWeb web, T item)
        {
            var list = GetList(web);
            var spListItem = list.Items.Add();
            MapEntityItemToSPListItem(item, spListItem);
            spListItem.Update();
            // Set these so item can be discovered by calling code
            item.Id = spListItem.ID;
            item.Guid = spListItem.UniqueId;
            return true;
        }

        protected virtual bool UpdateItem(SPWeb web, T item)
        {
            var list = GetList(web);
            var spListItem = list.GetItemById(item.Id);
            // Only for document libraries...
            if (spListItem.File != null)
            {
                try
                {
                    // File is checked-out already, skip update
                    if (spListItem.File.CheckOutType != SPFile.SPCheckOutType.None)
                        return false;

                    // Attempt to check out the file if necessary
                    if (spListItem.File.RequiresCheckout)
                        spListItem.File.CheckOut();
                }
                catch { return false; } // Can't check out, skip update
            }

            MapEntityItemToSPListItem(item, spListItem);
            spListItem.Update();

            // File doesn't require check out, don't have to do anything
            if (spListItem.File == null || !spListItem.File.RequiresCheckout) return true;

            // File is checked out, so check back in
            var userName = web.CurrentUser == null ? "System User" : web.CurrentUser.LoginName;
            spListItem.File.CheckIn("Checked in by system after update on behalf of " + userName,
                SPCheckinType.MajorCheckIn);
            return true;
        }

        protected virtual bool DeleteItem(SPWeb web, T item)
        {
            var list = GetList(web);
            var listItem = list.GetItemById(item.Id);
            list.Items.DeleteItemById(listItem.ID);
            return true;
        }

        #endregion

        #region Private methods

        private List<T> GetAllItems(SPWeb web, SPQuery query)
        {
            var list = GetList(web);
            var items = query == null
                ? list.GetItems()
                : list.GetItems(query);

            return (from SPListItem item in items select MapSPListItemToEntityItem(item)).ToList();
        }

        private T GetSingleItem(SPWeb web, int id)
        {
            var list = GetList(web);
            var spListItem = list.GetItemById(id);
            return MapSPListItemToEntityItem(spListItem);
        }

        protected SPList GetList(SPWeb web)
        {
            var list = web.Lists.TryGetList(ListName);
            if (list == null) throw new ListNotFoundException(ListName + " does not exist in web " + web.Url);
            return list;
        }

        #endregion

    }
}
