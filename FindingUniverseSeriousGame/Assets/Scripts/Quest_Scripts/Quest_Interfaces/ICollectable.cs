using UnityEngine;

public interface ICollectable
{
    /// <summary>
    /// OnCollect è il metodo che viene chiamato quando un oggetto 
    /// che implementa l'interfaccia ICollectable viene raccolto.
    /// </summary>
    void OnCollect();

}
