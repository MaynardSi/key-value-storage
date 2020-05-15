using System.Collections.Generic;

/// <summary>
/// Repository class for the Key Value Pair
/// </summary>
public class KeyValuePairRepository
{
    #region Constructor

    public KeyValuePairRepository()
    {
        KeyValuePairs = new Dictionary<string, string>()
        {
            { "ExampleKey", "ExampleValue" }
        };
    }

    #endregion Constructor

    #region Properties

    public Dictionary<string, string> KeyValuePairs { get; set; }

    #endregion Properties

    #region Methods

    public void AddKeyValuePair(string key, string value)
    {
        KeyValuePairs.Add(key, value);
    }

    #endregion Methods
}