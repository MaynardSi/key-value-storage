using System.Collections.Generic;

namespace Server
{
    /// <summary>
    /// Repository class for the Key Value Pair.
    /// </summary>
    public sealed class KeyValuePairRepository
    {
        private static readonly KeyValuePairRepository instance = new KeyValuePairRepository();

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

        public static KeyValuePairRepository GetInstance()
        {
            return instance;
        }

        public void AddKeyValuePair(string key, string value)
        {
            KeyValuePairs.Add(key, value);
        }

        #endregion Methods
    }
}