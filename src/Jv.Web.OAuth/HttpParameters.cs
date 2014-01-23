using Jv.Web.OAuth.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        IDictionary<string, T> OfType<T>()
        {
            return (from param in _parameters
                    where param.Value is T
                    select new KeyValuePair<string, T>(param.Key, (T)param.Value))
                   .ToDictionary();
        }

        public IDictionary<string, string> Fields
        {
            get { return OfType<string>(); }
        }

        public IDictionary<string, File> Files
        {
            get { return OfType<File>(); }
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

        public HttpParameters NotNullParameters
        {
            get
            {
                var notNullParams = new HttpParameters();
                notNullParams._parameters.AddRange(_parameters.Where(p => p.Value != null));
                return notNullParams;
            }
        }

        public void Add(string name, string value)
        {
            _parameters.Add(name, value);
        }

        public void Add(string name, File value)
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

        public void AddRange(IEnumerable<KeyValuePair<string, File>> keyValues)
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

        public string GetField(string name)
        {
            return (string)_parameters.Single(p => p.Key == name).Value;
        }

        public File GetFile(string name)
        {
            return (File)_parameters.Single(p => p.Key == name).Value;
        }

        public void Set(string name, string value)
        {
            Set(name, (object)value);
        }

        public void Set(string name, File value)
        {
            Set(name, (object)value);
        }

        void Set(string name, object value)
        {
            var ind = _parameters.FindIndex(p => p.Key == name);
            if (ind >= 0)
                _parameters[ind] = new KeyValuePair<string, object>(name, value);
            else
                _parameters.Add(name, value);
        }

        public void Sort()
        {
            _parameters.Sort((a, b) => a.Key.CompareTo(b.Key));
        }
    }
}
