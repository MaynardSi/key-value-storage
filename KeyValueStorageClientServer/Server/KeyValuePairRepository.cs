using System.Collections.Generic;

public class KeyValuePairRepository
{
    public KeyValuePairRepository()
    {
        this.KeyValuePairs = new Dictionary<string, string>();
    }

    public Dictionary<string, string> KeyValuePairs { get; set; }

    public void AddKeyValuePair(string key, string value)
    {
        this.KeyValuePairs.Add(key, value);
    }
}