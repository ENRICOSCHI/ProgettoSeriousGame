using UnityEngine;

public class FiondaEvento : Eventi
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NotificaPersonalizzata(notificaMessaggio[0]);
            //se l'evento non è ancora stato scoperto...
            if (!PersistentSceneData.Instance.isDescriptionFiondaHappened)
            {
                ActiveSubtitlesWithAudio(); //attivo descrizione audio con sottotitoli
                UnlockOnCodexMenu(); //sblocco argomento nel codex
                PersistentSceneData.Instance.isDescriptionFiondaHappened = true;
            }
            StartCoroutine(other.GetComponent<MovimentoNavicella>().AddBoost());//attivo il boost di velocità per la navicella
            //aggiorno la velocità nella UI
            ManagerHandler.ManagerInstance.SpeedManager.UpdateSpeedDisplay(other.GetComponent<MovimentoNavicella>());
        }
    }
}