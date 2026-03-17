using UnityEngine;
using System.Collections;
using TMPro;

public class ProgramTestScript : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !inputField.isFocused) //il focus controlla se sto scrivendo nel campo di input
        {
            inputField.gameObject.SetActive(!inputField.gameObject.activeSelf);
            if (inputField.gameObject.activeSelf)
            {
                inputField.ActivateInputField(); // Attiva il campo di input quando viene mostrato
            }
            else
            {
                inputField.DeactivateInputField(); // Disattiva il campo di input quando viene nascosto
            }
        }
    }

    public void checkComandFromInput()
    {
        {
            string comandoOttenuto = inputField.text;
            string[] substring = comandoOttenuto.Split();
            string comand = substring[0];//comando utilizzato
            string argomento = "";
            if (substring.Length > 1)//evito cos� che vada in eccezione in caso non venga inserito l'argomento
                argomento = substring[1];//argomento del comando (una parola)

            switch (comand)
            {
                case "ShowDialogue":
                    //avvio evento
                    Debug.Log("ShowDialogue");
                    DelegateClass.DialogueBoxEventsHandler.Invoke(argomento);
                    break;

                case "ShowNotification":
                    //avvio evento
                    Debug.Log("ShowNotification");
                    DelegateClass.NotificationEventsHandler.Invoke(argomento);
                    break;
                case "CodexUpdate":
                    Debug.Log("Codex Update");
                    ManagerHandler.ManagerInstance.NotificationManager.ShowNotificationCodexUpdate(argomento);
                    break;
                default:
                    Debug.LogWarning("Comando sconosciuto");
                    break;
            }
        }
    }
}
