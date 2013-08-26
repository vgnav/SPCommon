using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using SPCommon.Entity;
using SPCommon.Infrastructure.SPWebService;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Repository
{
    public abstract class GenericListWSRepository<T> : IListRepository<T> where T : BaseItem, new()
    {
        private readonly Lists _listWebservice;
        private readonly string _listName;

        protected GenericListWSRepository(ICredentials credentials, string url, string listName)
        {
            _listName = listName;
            _listWebservice = new SPWebService.Lists
            {
                Credentials = credentials ?? CredentialCache.DefaultNetworkCredentials,
                Url = url
            };
        }

        public bool Create(T t)
        {
            var command = GetNewBatchString(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public T Read(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(T t)
        {
            var command = GetUpdateBatchString(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public bool Delete(T t)
        {
            var command = GetDeleteBatchString(t);
            _listWebservice.UpdateListItems(_listName, command);
            return true;
        }

        public IList<T> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<T> FindByQuery(object query)
        {
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
                {
                    "Title", item.Title
                }
            };
        }

        #endregion

        #region Helper methods

        enum WebServiceListCommand
        {
            Update, New, Delete
        }

        private XmlElement GetNewBatchString(T item)
        {
            return GetBatchSingleItemString(item, WebServiceListCommand.New);
        }

        private XmlElement GetUpdateBatchString(T item)
        {
            return GetBatchSingleItemString(item, WebServiceListCommand.Update);
        }

        private XmlElement GetDeleteBatchString(T item)
        {
            return GetBatchSingleItemString(item, WebServiceListCommand.Delete);
        }

        private XmlElement GetBatchSingleItemString(T item, WebServiceListCommand command)
        {
            var fieldMap = ProvideItemFieldMap(item);
            var xmlDoc = new XmlDocument();
            var elBatch = xmlDoc.CreateElement("Batch");
            elBatch.SetAttribute("OnError", "Continue");
            elBatch.SetAttribute("ListVersion", "1");
            elBatch.SetAttribute("ViewName","");
            elBatch.InnerXml = GetMethodString(fieldMap, command, 1);
            return elBatch;
        }

        private static string GetMethodString(Dictionary<string, string> fieldMap, WebServiceListCommand command, int commandId)
        {
            var method = new StringBuilder();
            method.AppendFormat(@"<Method ID=""{0}"" Cmd=""{1}"">", commandId, command);
            if(command == WebServiceListCommand.New)
                method.AppendFormat(GetFieldNameString("ID", "New")); // need to set this for NEW items
            foreach (var key in fieldMap.Keys)
                method.AppendFormat(GetFieldNameString(key, fieldMap[key]));
            method.AppendFormat(@"</Method>");
            return method.ToString();
        }

        private static string GetFieldNameString(string fieldName, string fieldValue)
        {
            return string.Format(@"<Field Name=""{0}"">{1}</Field>", fieldName, fieldValue);
        }

        #endregion
    }
}
