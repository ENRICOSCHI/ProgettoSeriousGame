using UnityEngine;

public abstract class Eventi : MonoBehaviour
{
    [TextArea(3, 10)] [SerializeField] string descrizione;

    [ContextMenu("TestaEvento")]
    public void Descrizione()
    {
        DelegateClass.DialogueBoxEventsHandler.Invoke(descrizione);
    }
}
