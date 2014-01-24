using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Jv.Web.OAuth
{
    public class SafeObject : DynamicObject
    {
        IDictionary<string, object> _obj;
        public SafeObject(IDictionary<string, object> dict)
        {
            _obj = dict;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_obj.ContainsKey(binder.Name))
            {
                result = _obj[binder.Name];
                var resultAsDict = result as IDictionary<string, object>;
                if (resultAsDict != null)
                    result = new SafeObject(resultAsDict);
            }
            else
                result = null;

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _obj.Keys;
        }

        public static dynamic Create(object obj)
        {
            if (obj is IList<object>)
                return new SafeList((IList<object>)obj);
            if (obj is IDictionary<string, object>)
                return new SafeObject((IDictionary<string, object>)obj);
            return obj;
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
