using System;
using System.Collections.Generic;
using SPCommon.Interface;
using SPCommon.Serializers;

namespace SPCommon.Entity
{
    public class BaseItem : IJSONSerializable
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string ContentTypeName { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string IconUrl { get; set; }
        public byte[] FileData { get; set; }
        public string FileUrl { get; set; }

        #region JSON stuff. TODO: change JSON implementation
        public JSON ToJSON()
        {
            return new JSON(ProvideJSONProperties());
        }

        public virtual Dictionary<string, string> ProvideJSONProperties()
        {
            var properties = new Dictionary<string, string>
            {
                {"Title", Title},
                {"Id", Convert.ToString(Id)},
                {"Author", Author},
                {"ContentTypeName", ContentTypeName}
            };
            return properties;
        }
        #endregion
    }
    
}
