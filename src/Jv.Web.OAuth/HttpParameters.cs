using Jv.Web.OAuth.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Jv.Web.OAuth
{
    public class HttpParameters : IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, StorageFile>>
    {
        List<KeyValuePair<string, object>> _parameters = new List<KeyValuePair<string, object>>();

        public HttpParameters(HttpParameters parameters = null)
        {
            if (parameters != null)
                _parameters.AddRange(parameters._parameters);
        }

        public void Add(string name, string value)
        {
            _parameters.Add(name, value);
        }

        public void Add(string name, StorageFile value)
        {
            _parameters.Add(name, value);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            _parameters.AddRange(keyValues.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
        }

        public void AddRange(IEnumerable<KeyValuePair<string, StorageFile>> keyValues)
        {
            _parameters.AddRange(keyValues.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return _parameters.OfType<KeyValuePair<string, string>>().GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, StorageFile>> IEnumerable<KeyValuePair<string, StorageFile>>.GetEnumerator()
        {
            return _parameters.OfType<KeyValuePair<string, StorageFile>>().GetEnumerator();
        }
    }
}
