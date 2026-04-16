using System.Collections.Generic;
using UnityEngine;

public interface IHandleJSON
{
    /* SAVE */
    void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data);
    void Save();

    /* LOAD */
    bool CheckJsonFile();

    Dictionary<TKey, TValue> LoadJson<TKey, TValue>();

}
