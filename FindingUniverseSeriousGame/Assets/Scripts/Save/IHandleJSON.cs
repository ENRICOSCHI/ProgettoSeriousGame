using System.Collections.Generic;
using UnityEngine;

public interface IHandleJSON
{
    void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data);

    void SaveGame(string id, bool isActive, bool isCompleted);

    void Save();


    //Da fare il Load
}
