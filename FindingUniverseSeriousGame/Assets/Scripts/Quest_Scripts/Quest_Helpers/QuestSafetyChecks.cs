using UnityEngine;

/// <summary>
/// Classe statica preposta a fornire controlli di sicurezza logici
/// e ai riferimenti per i componenti delle Quest.
/// </summary>
public static class QuestSafetyChecks
{
    #region Controlli per Liste Item Raccoglibili per Quest 3

    /// <summary>
    /// Metodo che controlla se l'array di GameObject in ingresso non ha
    /// riferimenti disponiibli o è vuoto.
    /// </summary>
    /// <remarks>
    /// Durante l'esecuzione è anche controllato che ogni riferimento interno ai singoli
    /// elementi dell'array non sia nullo.
    /// </remarks>
    /// <returns>
    /// Ritorna "False" se e solo se il Check effettuato è fallito.
    /// "True" altrimenti.
    /// </returns>
    public static bool CheckGameObjectArray(GameObject[] list)
    {
        if (list == null || list.Length <= 0
            || System.Array.Exists(list, item => item == null))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Controlla se ogni elemento della lista di GameObject in entrata ha 
    /// un componente che implementa ICollectable.
    /// Pensato originariamente per verificare la presenza di Collectable_Item_Quest3 
    /// in ogni entrata della lista questItem in Quest_3_Script.
    /// </summary>
    /// <returns>
    /// Ritorna "False" se e solo se il Check effettuato è fallito.
    /// "True" altrimenti.
    /// </returns>
    public static bool CheckICollectable(GameObject[] list)
    {
        foreach (GameObject item in list)
        {
            if (item == null || item.GetComponent<ICollectable>() == null)
            {
                return false;
            }
        }
        return true;
    }

    #endregion
}
