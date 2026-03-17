using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class DelegateClass
{
    /*Gestione eventi DialogueBox*/
    public delegate void DialogueBoxEvents(string message);
    public static DialogueBoxEvents DialogueBoxEventsHandler;


    /*Gestione eventi Notification*/
    public delegate void NotificationEvents(string message);
    public static NotificationEvents NotificationEventsHandler;
}
