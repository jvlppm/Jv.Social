using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Jv.Web.OAuth
{
    public class SafeObject : DynamicObject, IDictionary<string, object>
    {
        static SafeObject Null = new SafeObject();

        public static bool operator ==(SafeObject a, object b)
        {
            if (Object.ReferenceEquals(a, null))
                a = Null;

            return a.Equals(b);
        }

        public static bool operator !=(SafeObject a, object b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            if (_instance != null)
                return _instance.GetHashCode();
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(this, obj))
                return true;

            if (obj is SafeObject)
                obj = ((SafeObject)obj)._instance;

            if (_instance != null)
                return _instance.Equals(obj);

            return _instance == obj;
        }

        object _instance;
        IDictionary<string, object> _obj;

        private SafeObject() { }
        public SafeObject(dynamic instance)
        {
            _instance = instance;
            _obj = instance as IDictionary<string, object>;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (_instance != null && binder.ReturnType.GetTypeInfo().IsAssignableFrom(_instance.GetType().GetTypeInfo()))
                result = _instance;
            else
                result = null;

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_obj != null && _obj.ContainsKey(binder.Name))
                result = SafeObject.Create(_obj[binder.Name]);
            else
                result = Null;

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            if (_obj == null)
                return Enumerable.Empty<string>();
            return _obj.Keys;
        }

        public static dynamic Create(object obj)
        {
            if (obj == null)
                return Null;
            if (obj is IList<object>)
                return new SafeList((IList<object>)obj);
            if (obj is IDictionary<string, object>)
                return new SafeObject((IDictionary<string, object>)obj);
            return obj;
        }

        public void Add(string key, object value)
        {
            _obj.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _obj.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _obj.Keys; }
        }

        public bool Remove(string key)
        {
            return _obj.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _obj.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _obj.Values; }
        }

        public object this[string key]
        {
            get
            {
                if (!_obj.ContainsKey(key))
                    return Null;
                return SafeObject.Create(_obj[key]);
            }
            set { _obj[key] = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _obj.Add(item);
        }

        public void Clear()
        {
            _obj.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _obj.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _obj.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _obj.Count; }
        }

        public bool IsReadOnly
        {
            get { return _obj.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _obj.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _obj.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _obj.GetEnumerator();
        }
    }

    public class SafeList : DynamicObject, IEnumerable
    {
        IList<object> _list;

        public SafeList(IList<dynamic> list)
        {
            _list = list;
        }

        public int IndexOf(dynamic item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, dynamic item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public dynamic this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Add(dynamic item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(dynamic item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(dynamic[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public bool Remove(dynamic item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<dynamic> GetEnumerator()
        {
            return _list.Select(SafeObject.Create).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
