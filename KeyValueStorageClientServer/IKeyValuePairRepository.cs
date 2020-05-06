using System;
using System.Collections.Generic;

public interface IKeyValuePairRepository
{
    IDictionary<TKey, TValue> KeyValuePair { get; }
}
