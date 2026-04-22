using UnityEngine;

public class FiondaEvento : Eventi
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NotificaPersonalizzata(notificaMessaggio[0]);
            if (!PersistentSceneData.Instance.isDescriptionFiondaHappened)
            {
                ActiveSubtitlesWithAudio();
                UnlockOnCodexMenu();
                PersistentSceneData.Instance.isDescriptionFiondaHappened = true;
            }
            StartCoroutine(other.GetComponent<MovimentoNavicella>().AddBoost());
            ManagerHandler.ManagerInstance.SpeedManager.UpdateSpeedDisplay(other.GetComponent<MovimentoNavicella>());
        }
    }
}