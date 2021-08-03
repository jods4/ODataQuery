using System;
using System.Collections;
using System.Collections.Generic;

namespace ODataQuery
{
  // Most of this is not implemented and is not meant to be used as a real dictionary.
  // This is a very lightweight dictionary (in fact just a plain list of KeyValuePair structs)
  // that is returned by $select for JSON serialization.
  // It works fine for this job and is more efficient than a real dictionary (either sorted list or hashtable).
  class SelectDictionary : IDictionary<string, object>
  {
    private List<KeyValuePair<string, object>> values = new();

    public int Count => values.Count;
    public bool IsReadOnly => false;

    public void Add(string key, object value) => Add(new(key, value));

    public void Add(KeyValuePair<string, object> item) => values.Add(item);

    public void Clear() => values.Clear();

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)values).GetEnumerator();

    public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ICollection<string> Keys => throw new NotImplementedException();

    public ICollection<object> Values => throw new NotImplementedException();

    public bool Contains(KeyValuePair<string, object> item) => throw new NotImplementedException();

    public bool ContainsKey(string key) => throw new NotImplementedException();

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(string key) => throw new NotImplementedException();

    public bool Remove(KeyValuePair<string, object> item) => throw new NotImplementedException();

    public bool TryGetValue(string key, out object value) => throw new NotImplementedException();
  }
}
