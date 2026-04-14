using UnityEngine;

public class FiondaEvento : Eventi
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NotificaPersonalizzata(notificaMessaggio[0]);
            StartCoroutine(other.GetComponent<MovimentoNavicella>().AddBoost());
            ManagerHandler.ManagerInstance.SpeedManager.UpdateSpeedDisplay(other.GetComponent<MovimentoNavicella>());
        }
    }
}