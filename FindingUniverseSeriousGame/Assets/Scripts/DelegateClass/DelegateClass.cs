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
    public delegate void NotificationEvents(string message,Color notificationColor);
    public static NotificationEvents NotificationEventsHandler;

    /*Gestione evento vento solare*/
    public delegate void VentoSolareEvents(bool isActive);
    public static VentoSolareEvents VentoSolareEventsHandler;

    /*Gestione evento salvataggio*/
    public delegate void SaveEvent(bool isChangingLevel);
    public static SaveEvent SaveEventHandler;

    /* Gestione Caricamento */
    public delegate void LoadEvent(bool isChangingLevel);
    public static LoadEvent LoadEventHandler;

    /* Gestione Update Quest Data */
    /*Il seguente evento è pensato in maniera specifica
    per essere catturato dai vari script CheckPotenziamenti*/
    public delegate void UpdateQuestDataEvent(CategoryMission[] categoryMissions);
    public static UpdateQuestDataEvent UpdateQuestDataEventHandler;
}
