using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateClass
{
    /*Gestione eventi DialogueBox*/
    public delegate void DialogueBoxEvents(string message);
    public static DialogueBoxEvents DialogueBoxEventsHandler;
}
