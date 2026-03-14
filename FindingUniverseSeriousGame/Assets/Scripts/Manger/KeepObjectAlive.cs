using UnityEngine;

public class KeepObjectAlive : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); 
    }
}
