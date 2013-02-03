using Jv.Web.OAuth.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Jv.Web.OAuth
{
    public class HttpParameters : IEnumerable<KeyValuePair<string, object>>
    {
        public HttpParameters()
            : this(null)
        {
        }

        public HttpParameters(HttpParameters parameters)
        {
            _parameters = new List<KeyValuePair<string, object>>();
            if (parameters != null)
                AddRange(parameters);
        }

        List<KeyValuePair<string, object>> _parameters;

        IEnumerable<KeyValuePair<string, T>> OfType<T>()
        {
            return from param in _parameters
                   where param.Value is T
                   select new KeyValuePair<string, T>(param.Key, (T)param.Value);
        }

        public IEnumerable<KeyValuePair<string, string>> Fields
        {
            get { return OfType<string>(); }
        }

        public IEnumerable<KeyValuePair<string, StorageFile>> Files
        {
            get { return OfType<StorageFile>(); }
        }

        public HttpParameters FieldParameters
        {
            get
            {
                var fieldParams = new HttpParameters();
                fieldParams.AddRange(Fields);
                return fieldParams;
            }
        }

        public HttpParameters FileParameters
        {
            get
            {
                var fileParams = new HttpParameters();
                fileParams.AddRange(Files);
                return fileParams;
            }
        }

        public void Add(string name, string value)
        {
            _parameters.Add(name, value);
        }

        public void Add(string name, StorageFile value)
        {
            _parameters.Add(name, value);
        }

        public void AddRange(HttpParameters parameters)
        {
            _parameters.AddRange(parameters._parameters);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            _parameters.AddRange(keyValues.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
        }

        public void AddRange(IEnumerable<KeyValuePair<string, StorageFile>> keyValues)
        {
            _parameters.AddRange(keyValues.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
