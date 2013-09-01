using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.SharePoint;
using SPCommon.CAML;
using SPCommon.Entity;
using SPCommon.Infrastructure.SPWebService;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    public abstract class BaseWebServiceListRepository<T> : IListRepository<T> where T : BaseItem, new()
    {
        private readonly Lists _listWebservice;
        private readonly string _listName;

        protected BaseWebServiceListRepository(ICredentials credentials, string url, string listName)
        {
            _listName = listName;
            _listWebservice = new Lists
            {
                Credentials = credentials ?? CredentialCache.DefaultNetworkCredentials,
                Url = url
            };
        }

        public bool Create(T t)
        {
            var command = GetNewItemXml(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public T Read(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(T t)
        {
            var command = GetUpdateXml(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public bool Delete(T t)
        {
            var command = GetDeleteXml(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public IList<T> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindByQuery(object query)
        {
            var itemsXml = _listWebservice.GetListItems(
                _listName,
                string.Empty, 
                GetQueryXml(query as SPQuery),
                GetViewFields(),
                "200", 
                GetQueryOptions(), 
                string.Empty);
            throw new System.NotImplementedException();
        }

        public IList<T> FindByCAML(ICAMLExpression camlExpression)
        {
            throw new System.NotImplementedException();
        }

        #region Overridable regions

        protected virtual Dictionary<string, string> ProvideItemFieldMap(T item)
        {
            return new Dictionary<string, string>
            {
                {"Title", item.Title},
                {"ID", item.Id.ToString(CultureInfo.InvariantCulture)}
            };
        }
        
        protected virtual IList<string> ProvideViewFields()
        {
            return new List<string> {"Title"};
        }

        #endregion

        #region Helper methods

        enum WebServiceListCommand
        {
            Update, New, Delete
        }

        private XmlElement GetQueryXml(ICAMLExpression query)
        {
            return GetQueryXml(new SPQuery {Query = (new CAMLBuilder(query)).ToString()});
        }

        private static XmlElement GetQueryXml(SPQuery spQuery)
        {
            var xmlDoc = new XmlDocument();
            var query = xmlDoc.CreateElement("Query");
            query.InnerXml = spQuery.Query;
            return query;
        }

        private XmlElement GetViewFields()
        {
            var xmlDoc = new XmlDocument();
            var viewFields = xmlDoc.CreateElement("ViewFields");
            viewFields.InnerXml = GetViewFieldsForQuery();
            return viewFields;
        }

        private static XmlElement GetQueryOptions()
        {
            var xmlDoc = new XmlDocument();
            var queryOptions = xmlDoc.CreateElement("QueryOptions");
            queryOptions.InnerXml = "";
            return queryOptions;
        }

        private string GetViewFieldsForQuery()
        {
            var viewFields = ProvideViewFields();
            // TODO
            return string.Empty;
        }


        private XmlElement GetNewItemXml(T item)
        {
            return GetBatchSingleItemXml(item, WebServiceListCommand.New);
        }

        private XmlElement GetUpdateXml(T item)
        {
            return GetBatchSingleItemXml(item, WebServiceListCommand.Update);
        }

        private XmlElement GetDeleteXml(T item)
        {
            return GetBatchSingleItemXml(item, WebServiceListCommand.Delete);
        }

        private XmlElement GetBatchSingleItemXml(T item, WebServiceListCommand command)
        {
            var fieldMap = ProvideItemFieldMap(item);
            var xmlDoc = new XmlDocument();
            var batchElement = xmlDoc.CreateElement("Batch");
            batchElement.SetAttribute("OnError", "Continue");
            batchElement.SetAttribute("ListVersion", "1");
            batchElement.SetAttribute("ViewName","");
            batchElement.InnerXml = GetMethodString(fieldMap, command, 1);
            return batchElement;
        }

        private static string GetMethodString(Dictionary<string, string> fieldMap, WebServiceListCommand command, int commandId)
        {
            var method = new StringBuilder();
            method.AppendFormat(@"<Method ID=""{0}"" Cmd=""{1}"">", commandId, command);
            // ID for 'new' items is 'New'
            if (command == WebServiceListCommand.New)
            {
                if (fieldMap.ContainsKey("ID"))
                    fieldMap["ID"] = "New";
                else
                    fieldMap.Add("ID", "New");
            }
            foreach (var key in fieldMap.Keys)
                method.AppendFormat(GetFieldNameString(key, fieldMap[key]));
            method.AppendFormat(@"</Method>");
            return method.ToString();
        }

        private static string GetFieldNameString(string fieldName, string fieldValue)
        {
            // It's okay for fieldValue to be empty, but fieldName cannot be empty
            if (string.IsNullOrEmpty(fieldName) || fieldValue == null) return string.Empty;
            return string.Format(@"<Field Name=""{0}"">{1}</Field>", fieldName, fieldValue);
        }

        #endregion
    }
}
