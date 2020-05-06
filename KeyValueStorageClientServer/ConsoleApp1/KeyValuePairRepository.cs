using System.Collections.Generic;

public class KeyValuePairRepository
{
    public KeyValuePairRepository()
    {
        this.KeyValuePairs = new Dictionary<object, object>();
    }

    public Dictionary<object, object> KeyValuePairs { get; set; }

    public void AddKeyValuePair(object key, object value)
    {
        this.KeyValuePairs.Add(key, value);
    }
}