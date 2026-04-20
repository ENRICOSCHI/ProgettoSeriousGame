using System.Collections.Generic;
using UnityEngine;

public interface IHandleJSON
{
    /* SAVE */
    void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data);
    void Save(bool isChangingLevel);

    /* LOAD */
    bool CheckJsonFile();
    void Load(bool isChangingLevel);

    Dictionary<TKey, TValue> LoadJson<TKey, TValue>();

}
